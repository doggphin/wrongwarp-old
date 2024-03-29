using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Linq;

namespace WrongWarp
{
    /// <summary> Attached to a player. Stores several inventories and has several functions to communicate between them. </summary>
    public class InventoryManager : NetworkBehaviour
    {
        [SerializeField] private UIManager uiManager;
        private InventoryDisplayManager inventoryDisplayManager;
        [SerializeField] private HotbarController hotbarEquipper;

        public List<ServerInventory> serverInventories = new();                     // [SERVER] A list of Inventory references this player has access to.
        public List<ClientInventory> clientInventories = new();                     // [CLIENT] A list of dummy Inventory references that the client uses to represent server inventories.
        public ClientInventory ClientHotbar { get { return clientInventories[1]; } }
        public ServerInventory ServerHotbar { get { return serverInventories[1]; } }

        /// <summary> Initialize this inventory as default and initializes client. </summary>
        [Server]
        public void ServerInit()
        {
            SetDefaultInventory();
        }

        /// <summary>
        /// Starts inventory initialization chain on the client and subscribes to relevant events.
        /// </summary>
        public void ClientInit()
        {
            inventoryDisplayManager = uiManager.inventoryDisplayManager;
            inventoryDisplayManager.Init(this);
            hotbarEquipper.Init();

            InventoryIcon.SlotPickedUp += PickUpSlot;
            InventoryIcon.SlotDropped += DropSlot;
            InventoryDisplayTab.DroppedOnTab += OpenSlot;
            InventoryDisplayManager.InventoryClosed += ClearSlotPickedUpCache;
            InventoryDisplayManager.InventoryClosed += RemoveAddedInventories;
        }

        // ======================
        // INVENTORY REFERENCING
        // ======================

        #region Inventory Referencing

        /// <summary>
        /// Reset inventories to default. Also erases current inventory. Use for a starting player.
        /// </summary>
        [Server]
        public void SetDefaultInventory()
        {
            foreach (ServerInventory inventory in serverInventories) { inventory.RemoveObserver(connectionToClient); }
            serverInventories.Clear();

            TargetClearInventories(connectionToClient);
            AddInventory(new ServerInventory(0, connectionToClient.identity));
            AddInventory(new ServerInventory(1, connectionToClient.identity));
            AddInventory(new ServerInventory(2, connectionToClient.identity));
            serverInventories[1].SetItem(0, new ServerSimpleItem(13, 1));
        }

        /// <summary>
        /// Updates a client that their inventories have been completely removed.
        /// </summary>
        /// <param name="target"></param>
        [TargetRpc]
        public void TargetClearInventories(NetworkConnection target)
        {
            clientInventories.Clear();
            inventoryDisplayManager.ClearTabs();
        }

        /// <summary>
        /// Removes all non-default inventories.
        /// </summary>
        [Command]
        private void RemoveAddedInventories()
        {
            while(serverInventories.Count > 3)
            {
                RemoveInventory(3);
            }
        }

        /// <summary>
        /// Removes an inventory reference at a given index on the server.
        /// </summary>
        /// <param name="index"> Index to remove. </param>
        [Server]
        public void RemoveInventory(ushort index)
        {
            // Can't remove inventory 0, 1 or 2.
            if (index <= 2)
            {
                Debug.Log("Can't remove base inventory, hotbar or equipment.");
                return;
            }

            serverInventories[index].RemoveObserver(connectionToClient);
            serverInventories.RemoveAt(index);
            TargetRemoveInventory(connectionToClient, index);
        }

        /// <summary>
        /// Removes an inventory reference at a given index on a client.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="index"> Index to remove. </param>
        [TargetRpc]
        public void TargetRemoveInventory(NetworkConnection target, ushort index)
        {
            clientInventories.RemoveAt(index);

            inventoryDisplayManager.RemoveTab(index);
        }

        /// <summary>
        /// Adds an inventory reference to this player's list of references and adds them as an observer to the inventory.
        /// </summary>
        /// <param name="serverInventory"> The inventory to give a reference to. </param>
        [Server]
        public void AddInventory(ServerInventory serverInventory)
        {
            foreach(ServerInventory curServerInventory in serverInventories)
            {
                if (curServerInventory == serverInventory) { Debug.Log("Inventory already opened."); return; }
            }

            // Find what the index of this inventory will be
            // (If inventories is 7 long, index 7 will be the next slot filled)
            ushort newInventoryIndex = (ushort)serverInventories.Count;

            // Next, add this Inventory to inventories and this connection to the inventory's observers
            serverInventories.Add(serverInventory);
            serverInventory.AddObserver(connectionToClient, newInventoryIndex);
            TargetAddInventory(connectionToClient, serverInventory.ConvertToNetwork(serverInventory.ownerIdentity == connectionToClient.identity));
        }

        /// <summary>
        /// Adds a new inventory to inventories on the client.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="networkInventory"> The converted serverInventory which the client will convert into a clientInventory. </param>
        [TargetRpc]
        private void TargetAddInventory(NetworkConnection target, NetworkInventory networkInventory)
        {
            // Safe to use this with KCP (uses FIFO)
            clientInventories.Add(new ClientInventory(networkInventory));

            // Update display manager
            ushort indexToAdd = (ushort)(clientInventories.Count - 1);
            inventoryDisplayManager.AddTab(indexToAdd, clientInventories[indexToAdd]);
        }

        /// <summary>
        /// When the player asks to open an item inventory, attempt to add it as a reference.
        /// </summary>
        /// <param name="invIndex"></param>
        /// <param name="index"></param>
        [Command]
        public void CmdOpenItemInventory(ushort invIndex, ushort index)
        {
            if (serverInventories[invIndex].items[index].itemInventory == null)
            {
                Debug.Log("Item doesn't have an inventory.");
                return;
            }

            ServerInventory itemInventory = serverInventories[invIndex].items[index].itemInventory;
            AddInventory(itemInventory);
        }

        #endregion Inventory Referencing

        // ======================
        // UI DROP
        // ======================

        #region Inventory Manipulation

        /// <summary>
        /// When the player wants to drag one slot onto another, compute the result of that drag.
        /// </summary>
        /// <param name="startInvIndex"> The starting inventory index of the drag. </param>
        /// <param name="startIndex"> The starting items index of the drag. </param>
        /// <param name="destInvIndex"> The destination inventory index of the drag. </param>
        /// <param name="destIndex"> The destination items index of the drag. </param>
        /// <param name="amountTryingToMove"> How much the player is trying to move. </param>
        [Command]
        public void CmdUIDrop(ushort startInvIndex, ushort startIndex, ushort destInvIndex, ushort destIndex, ushort amountTryingToMove)
        {
            ServerSimpleItem startItem = MakeItemReference(startInvIndex, startIndex);
            ServerSimpleItem destItem = MakeItemReference(destInvIndex, destIndex);

            // If the drag is invalid, return without doing anything.
            if (!IsValidDrag(startInvIndex, startIndex, destInvIndex, destIndex, amountTryingToMove)) { return; }

            // Check if trying to stack up items.
            if (startItem.IsSameItem(destItem) && destItem.stackSize != destItem.MaxStackSize)
            {
                StackSlotIntoSlot(startInvIndex, startIndex, destInvIndex, destIndex, amountTryingToMove);
            }
            // Check if trying to move a whole slot into another slot.
            else if (startItem.stackSize == amountTryingToMove)
            {
                SwapIndexes(startInvIndex, startIndex, destInvIndex, destIndex);
            }
            // If partially moving an item slot, check if moving into an empty space.
            else if (destItem == default)
            {
                SplitSlotIntoEmptySlot(startInvIndex, startIndex, destInvIndex, destIndex, amountTryingToMove);
            }
        }

        #region CMDUIDragLogic

        [Server]
        private bool IsValidDrag(ushort startInvIndex, ushort startIndex, ushort destInvIndex, ushort destIndex, ushort amountTryingToMove)
        {
            ServerSimpleItem startItem = MakeItemReference(startInvIndex, startIndex);
            ServerSimpleItem destItem = MakeItemReference(destInvIndex, destIndex);

            if (startInvIndex == destInvIndex && startIndex == destIndex)
            {
                Debug.Log("ERROR: Tried to drag slot onto itself.");
                return false;
            }

            // If the destination can't accept the existing start item or the start can't accept the existing destination item,
            if ((!serverInventories[destInvIndex].Accepts(startItem.id) && startItem != default) || (!serverInventories[startInvIndex].Accepts(destItem.id) && destItem != default))
            {
                Debug.Log("ERROR: An inventory could not accept one of the items being swapped.");
                return false;
            }

            if (!serverInventories[startInvIndex].IsAllowedToUse(connectionToClient) || !serverInventories[startInvIndex].IsAllowedToUse(connectionToClient))
            {
                // Stop the function and leave
                Debug.Log("ERROR: Did not have authority over this inventory.");
                return false;
            }

            if (amountTryingToMove <= 0)
            {
                Debug.Log("ERROR: Tried to move an invalid amount of an item.");
            }

            if (serverInventories[startInvIndex].items[startIndex].stackSize < amountTryingToMove)
            {
                Debug.Log("ERROR: Tried to move more than exists.");
                return false;
            }

            // This doesn't work well with outside inventories.
            if (serverInventories[startInvIndex].items[startIndex] == default)
            {
                Debug.Log("ERROR: Tried to move nothing.");
                return false;
            }

            return true;
        }

        [Server]
        private void SwapIndexes(ushort startInvIndex, ushort startIndex, ushort destInvIndex, ushort destIndex)
        {
            ServerSimpleItem startItem = MakeItemReference(startInvIndex, startIndex);
            ServerSimpleItem destItem = MakeItemReference(destInvIndex, destIndex);

            serverInventories[destInvIndex].SetItem(destIndex, startItem);
            serverInventories[startInvIndex].SetItem(startIndex, destItem);
        }

        [Server]
        private void StackSlotIntoSlot(ushort startInvIndex, ushort startIndex, ushort destInvIndex, ushort destIndex, ushort? amountTryingToMove)
        {
            serverInventories[startInvIndex].SubStackSize(startIndex, serverInventories[destInvIndex].AddStackSize(
                        destIndex, amountTryingToMove == null ? serverInventories[startInvIndex].items[startIndex].stackSize : (ushort)Mathf.Min(serverInventories[startInvIndex].items[startIndex].stackSize, (ushort)amountTryingToMove))
                );
        }

        [Server]
        private void SplitSlotIntoEmptySlot(ushort startInvIndex, ushort startIndex, ushort destInvIndex, ushort destIndex, ushort amountTryingToMove)
        {
            serverInventories[destInvIndex].SetItem(destIndex, serverInventories[startInvIndex].items[startIndex], 0, false, false);        // Copy an empty amount into the destination index
            ushort amountMoved = serverInventories[startInvIndex].SubStackSize(startIndex, amountTryingToMove, true);
            serverInventories[destInvIndex].AddStackSize(destIndex, amountMoved, true);
        }

        #endregion CmdUIDropLogic

        #endregion Inventory Manipulation

        // ======================
        // MISC
        // ======================

        #region Miscellaneous

        [Server]
        /// <summary> ushorthand for inventories[invIndex].items[itemIndex] </summary>
        private ServerSimpleItem MakeItemReference(ushort invIndex, ushort itemIndex)
        {
            return serverInventories[invIndex].items[itemIndex];
        }

        #endregion Miscellaneous

        // ======================
        // UPDATERS
        // ======================

        #region Updaters

        /// <summary> Fully syncs an inventory on a client. </summary>
        [TargetRpc]
        public void TargetSyncInventory(NetworkConnection target, ushort invIndex, NetworkInventory networkInventory)
        {
            clientInventories[invIndex].items = networkInventory.ItemsAsClient;
            UpdateClientInventory(invIndex);
        }

        /// <summary> Syncs the inventory's slot on a client. </summary>
        [TargetRpc]
        public void TargetSyncInventorySlot(NetworkConnection target, ushort invIndex, ushort index, NetworkSimpleItem item)
        {
            clientInventories[invIndex].items[index] = item.AsClient;
            UpdateClientInventory(invIndex, index);
        }

        /// <summary> Conveys to this InventoryDisplayManager that an item has been updated. </summary>
        [Client]
        public void UpdateClientInventory(ushort? invIndex = null, ushort? index = null)
        {
            if (invIndex == null)
            {
                for (ushort i = 0; i < serverInventories.Count; i++)
                {
                    inventoryDisplayManager.UpdateTabSlots(i);
                }
                return;
            }

            if (index == null)
            {
                inventoryDisplayManager.UpdateTabSlots((ushort)invIndex);
            }
            else
            {
                inventoryDisplayManager.UpdateTabSlot((ushort)invIndex, (ushort)index);
            }
        }

        #endregion Updaters

        // ======================
        // CLIENT SPECIFIC
        // ======================

        #region Client Specific

        // Picked up item cache 
        private (ushort invIndex, ushort index, ushort amount) pickedUpItem;

        [Client]
        private void PickUpSlot(ushort invIndex, ushort index, ushort? amount)
        {
            if (clientInventories[invIndex].items[index].isEmpty)
            {
                Debug.Log("ERROR: Cannot pick up nothing.");
                return;
            }

            pickedUpItem = (invIndex, index, amount == null ? clientInventories[invIndex].items[index].stackSize : (ushort)amount);

            inventoryDisplayManager.PickUpSlot(invIndex, index, pickedUpItem.amount);
        }

        [Client]
        private void DropSlot(ushort invIndex, ushort index)
        {
            CmdUIDrop(pickedUpItem.invIndex, pickedUpItem.index, invIndex, index, pickedUpItem.amount);
            inventoryDisplayManager.DropSlot();
        }

        [Client]
        private void OpenSlot()
        {
            CmdOpenItemInventory(pickedUpItem.invIndex, pickedUpItem.index);
            ClearSlotPickedUpCache();
        }

        private void ClearSlotPickedUpCache()
        {
            pickedUpItem = default;
            inventoryDisplayManager.DropSlot();
        }
        #endregion Client Specific


        // ======================
        // INFORMATION
        // ======================

        #region Information

        #endregion Information
    }
}