using Mirror;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace WrongWarp
{
    /// <summary> A container for SimpleItems. </summary>
    public class ServerInventory
    {
        /// <summary> The items this inventory contains. </summary>
        public ServerSimpleItem[] items { get; private set; }
        /// <summary> The display preset ID to inherit visual data and a whitelist from. </summary>
        public ushort displayPresetId { get; private set; }
        /// <summary> A struct that defines what aspects of this inventory's displayPresetId should be overwritten. <para> Default values (null) will result in no override. </para></summary>
        public DisplayPresetOverride displayPresetOverride { get; private set; }

        /// <summary> The NetworkIdentity that this inventory is attached to. </summary>
        public NetworkIdentity ownerIdentity { get; private set; }
        /// <summary> The list of connections that have access to this inventory and their permission level. </summary>
        public HashSet<InventoryUser> users { get; private set; } = new();
        /// <summary> The inventory this inventory is nested within, which may not exist. </summary>
        public InventoryLocation parentInventoryLocation { get; private set; }

        

        // ======================
        // CONSTRUCTORS
        // ======================

        #region Constructors

        // Server constructors, full information
        public ServerInventory(ushort inventoryPresetId, NetworkIdentity ownerIdentity = null, DisplayPresetOverride displayPresetOverride = default, InventoryLocation parentInventoryLocation = default)
        {
            items = new ServerSimpleItem[DatabaseLookup.InventoryPresetByID(inventoryPresetId).length];
            displayPresetId = DatabaseLookup.GetID(DatabaseLookup.InventoryPresetByID(inventoryPresetId).displayTabPreset);
            ServerConstructor(displayPresetOverride, ownerIdentity, parentInventoryLocation);
        }
        public ServerInventory(InventoryPreset inventoryPreset, NetworkIdentity ownerIdentity = null, DisplayPresetOverride displayPresetOverride = default, InventoryLocation parentInventoryLocation = default)
        {
            items = new ServerSimpleItem[inventoryPreset.length];
            displayPresetId = DatabaseLookup.GetID(inventoryPreset.displayTabPreset);
            ServerConstructor(displayPresetOverride, ownerIdentity, parentInventoryLocation);
        }
        public ServerInventory(ServerSimpleItem[] items = null, NetworkIdentity ownerIdentity = null, ushort displayPresetId = 0, DisplayPresetOverride displayPresetOverride = default, InventoryLocation parentInventoryLocation = default)
        {
            this.items = items;
            this.displayPresetId = displayPresetId;
            ServerConstructor(displayPresetOverride, ownerIdentity, parentInventoryLocation);
        }
        private void ServerConstructor(DisplayPresetOverride displayPresetOverride, NetworkIdentity ownerIdentity, InventoryLocation parentInventoryLocation)
        {
            users = new();
            this.parentInventoryLocation = parentInventoryLocation;
            this.displayPresetOverride = displayPresetOverride;
            this.ownerIdentity = ownerIdentity;
        }

        #endregion Constructors

        // ======================
        // CONVERSION
        // ======================

        #region Conversion

        /// <summary>
        /// Returns the network version of this Inventory for sending to clients.
        /// </summary>
        /// <param name="hasOwnership"> Does the connection that will be sent this NetworkInventory own this inventory? </param>
        /// <returns> The version of this inventory to send over the network to a client. </returns>
        public NetworkInventory ConvertToNetwork(bool hasOwnership)
        {
            return new NetworkInventory(ConvertItemsToNetwork(), parentInventoryLocation != default, displayPresetId, displayPresetOverride, hasOwnership);
        }

        /// <summary>
        /// Returns a NetworkItem[] version of this inventory's items.
        /// </summary>
        /// <returns> This inventory's "ServerSimpleItems[]" items as "NetworkItem[] items". </returns>
        public NetworkSimpleItem[] ConvertItemsToNetwork()
        {
            var networkItems = new NetworkSimpleItem[items.Length];
            for (ushort i = 0; i < networkItems.Length; i++)
            {
                networkItems[i] = items[i].ConvertToNetwork();
            }
            return networkItems;
        }

        #endregion Conversion

        // ======================
        // Inventory Permissions
        // ======================

        #region Inventory Permissions

        /// <summary>
        /// Sets the parent of this inventory to another inventory along with all of its children.
        /// </summary>
        /// <param name="parentInventory"> The inventory to set this inventory and its children's parents to. </param>
        [Server]
        public void SetParentInventory(ServerInventory parentInventory)
        {
            // Check if inventory is actually being changed
            if (parentInventoryLocation.inventory != parentInventory)
            {
                // Alert clients if hasParent has changed
            }
            else
            {
                // If parent inventory hasn't changed, no need to set parents
                return;
            }

            parentInventoryLocation = new InventoryLocation(parentInventory, parentInventoryLocation.index);
            ownerIdentity = parentInventory.ownerIdentity;

            // Recursively call all items within this inventory to update their owner inventory
            for(ushort i=0; i < items.Length; i++)
            {
                if(items[i].itemInventory != null)
                {
                    items[i].itemInventory.SetParentInventory(parentInventory);
                }
            }
        }

        /// <summary>
        /// Removes an observing connection from this inventory, no longer allowing them to interact with it.
        /// </summary>
        /// <param name="conn"> The connection to remove as an observer of this inventory. </param>
        /// <returns> Returns whether teh observer was removed. </returns>
        [Server]
        public bool RemoveObserver(NetworkConnection conn)
        {
            foreach(InventoryUser user in users)
            {
                if(user.conn == conn)
                {
                    users.Remove(user);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Adds an observing connection to this inventory, which allows them to interact with it assuming they have it opened on their screen.
        /// </summary>
        /// <param name="conn"> The connection to add as an observer of this inventory. </param>
        /// <returns> Returns whether the observer was successfully added. </returns>
        [Server]
        public bool AddObserver(NetworkConnection conn, ushort invIndex)
        {
            // Make sure this observer doesn't exist yet
            if (GetClientIndex(conn) != null)
            {
                Debug.Log("ERROR: Client is already an observer.");
                return false;
            }
            users.Add(new InventoryUser(conn, invIndex));
            return true;
        }

        /// <summary>
        /// Checks if a connection is observing this inventory.
        /// </summary>
        /// <param name="conn"> The connection to check for. </param>
        /// <returns></returns>
        [Server]
        public bool IsAllowedToUse(NetworkConnection conn)
        {
            // Also return true if the connection is the owner
            return GetClientIndex(conn) != null;
        }

        /// <summary>
        /// <summary> Returns the inventories index of a client. Returns null if the connection wasn't an observer. </summary>
        /// </summary>
        /// <param name="conn"> The connection to get the index of. </param>
        /// <returns></returns>
        [Server]
        private ushort? GetClientIndex(NetworkConnection conn)
        {
            foreach(InventoryUser user in users)
            {
                if(user.conn == conn)
                {
                    return user.invIndex;
                }
            }
            return null;  // Return not found.
        }

        /// <summary>
        /// Returns whether this inventory will accept an item ID.
        /// </summary>
        /// <param name="itemId"> Item ID to check. </param>
        /// <returns></returns>
        [Server]
        public bool Accepts(ushort itemId)
        {
            return DatabaseLookup.TabPresetByID(displayPresetId).Accepts(itemId);
        }

        /// <summary>
        /// Removes all observers from this inventory.
        /// </summary>
        [Server]
        // Add one here to close an inventory if the owner leaves
        public void RemoveAllObservers()
        {
            foreach (InventoryUser user in users)
            {
                user.conn.identity.GetComponent<InventoryManager>().RemoveInventory((ushort)user.invIndex);
            }
        }

        /// <summary>
        /// Updates an item's parentInventoryLocation to its location within this inventory.
        /// </summary>
        /// <param name="index"> Index of the item whose inventory's parentInventoryLocation is to be changed. </param>
        [Server]
        private void UpdateParentInformation(ushort index)
        {
            // If in a base inventory with no parent, set inventory location to (null, null); otherwise, set it to this inventory and the index the item is at
            InventoryLocation inventoryLocationOfIndex = items[index].itemInventory.parentInventoryLocation;
            InventoryLocation expectedInventoryLocationOfIndex = new InventoryLocation(this, index);

            // If parent inventory reference needs to be updated,
            if(inventoryLocationOfIndex.inventory != expectedInventoryLocationOfIndex.inventory)
            {
                // Update the item (and all of its children's) parent inventory reference(s)
                items[index].itemInventory.SetParentInventory(this);

                //Debug.Log($"Updated the parent inventory of slot {index}.");
            }

            // If parent inventory location index needs to be updated,
            if (inventoryLocationOfIndex.index != expectedInventoryLocationOfIndex.index)
            {
                // Update the item's parent inventory location index
                items[index].itemInventory.parentInventoryLocation = new InventoryLocation(inventoryLocationOfIndex.inventory, index);

                //Debug.Log($"Updated the location index of slot {index}'s inventory to 'index {expectedInventoryLocationOfIndex.index}'.");
            }
        }

        #endregion Inventory Permissions

        // ======================
        // INFORMATION
        // ======================

        #region Information

        /// <summary>
        /// Returns the amount of a given set of item IDs exists within this inventory.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Server]
        public int GetAmountOfIDs(ushort[] ids)
        {
            int amount = 0;
            foreach (ServerSimpleItem item in items)
            {
                if (ids.Contains(item.id))
                {
                    amount += item.stackSize;
                }
            }
            return amount;
        }

        #region GetAmountOFIDsOverloads

            [Server]
            public int GetAmountOfID(ushort id)
            {
                return GetAmountOfIDs(new ushort[1]{ id });
            }
            [Server]
            public int GetAmountOfIDs(ItemType itemType)
            {
                return GetAmountOfIDs(itemType.items.ToArray());
            }

         #endregion #region GetAmountOFIDsOverloads

        /// <summary> Finds slots an ID can fit into within the inventory. </summary>
        [Server]
        public ushort[] GetSpacesForItem(ServerSimpleItem itemToFindSpaceFor)
        {
            List<ushort> slotSpaces = new();
            ushort index = 0;
            foreach (ServerSimpleItem item in items)
            {
                if (item == default || (item.IsSameItem(itemToFindSpaceFor) && item.SpaceUntilFull != 0))
                {
                    slotSpaces.Add(index);
                }
                index++;
            }
            return slotSpaces.ToArray();
        }

        /// <summary> Finds the amount of an ID that can fit in the inventory. </summary>
        [Server]
        public ushort GetSpaceForItem(ServerSimpleItem item)
        {
            ushort[] spacesToCheck = GetSpacesForItem(item);

            ushort spaceForID = 0;
            foreach (ushort spaceToCheck in spacesToCheck)
            {
                if (items[spaceToCheck] == default)
                {
                    spaceForID += item.MaxStackSize;
                }
                else
                {
                    spaceForID += items[spaceToCheck].SpaceUntilFull;
                }
            }
            return spaceForID;
        }

        /// <summary> Checks amount of empty spaces within the inventory. </summary>
        [Server]
        public ushort GetEmptySpaces()
        {
            ushort emptySpaces = 0;
            foreach (ServerSimpleItem item in items)
            {
                if (item == default) { emptySpaces++; }
            }
            return emptySpaces;
        }

        /// <summary> Checks for the first empty space in an inventory. </summary>
        [Server]
        // This might be able to replace IsFull()- just check if inventory slot is null, and if so, it's full (otherwise go as normal)
        public ushort? GetFirstEmptySpace()
        {
            for (ushort i = 0; i < items.Length; i++)
            {
                if (items[i] == default)
                {
                    return i;
                }
            }
            return null;
        }

        #endregion Information

        /// <summary>
        /// Attempts to add an item to this inventory wherever it can fit.
        /// </summary>
        /// <param name="item"> Item to try to add. </param>
        /// <returns> How much of the item was taken. </returns>
        [Server]
        public ushort AddItem(ServerSimpleItem item)
        {
            ushort[] indexesToFill = GetSpacesForItem(item);
            ushort amountTryingToAdd = item.stackSize;
            foreach (ushort index in indexesToFill)
            {
                if (items[index] == default) { SetItem(index, item, 0, false, false); }    // If slot is empty, replace it with the item attempting to add of amount 0
                amountTryingToAdd -= AddStackSize(index, amountTryingToAdd);                 // Reduce amountTryingToAdd by amount that fits into the slot
                if (amountTryingToAdd == 0) { break; }                                           // If amount has ran out, exit loop.
            }

            return (ushort)Mathf.Abs(item.stackSize - amountTryingToAdd);               // Return amount that was taken
        }

        /// <summary>
        /// Sets an index to a simpleItem of a given amount.
        /// </summary>
        /// <param name="index"> Index to set the item of. </param>
        /// <param name="item"> What item to set this index to. </param>
        /// <param name="amount"> How much to set this index to be. </param>
        /// <param name="updateClients"> Should clients be updated when this slot is changed? </param>
        /// <param name="clearIfEmpty"> Should this item be cleared if amount is set to 0? </param>
        [Server]
        public void SetItem(ushort index, ServerSimpleItem item, ushort? amount = null, bool updateClients = true, bool clearIfEmpty = true)
        {
            // If amount is less than or equal to 0 (not including if amount is null and stackSize is more than 0) and this should be cleared if empty,
            if (amount <= 0 && !(amount == null && item.stackSize > 0) && clearIfEmpty)
            { 
                ClearItem(index);
                return;
            }
            items[index] = item;
            items[index].stackSize = amount == null ? item.stackSize : (ushort)amount;          // If stackSize was left as default (null), set stackSize to itemToSet amount
            if (items[index].itemInventory != null) { UpdateParentInformation(index); }         // If item has an item inventory, check if its parent information needs to be updated
            if (updateClients) { UpdateObserversSingle(index); }
        }

        /// <summary>
        /// Adds to the stack size of an item.
        /// <para>  Deletes item if stack size is 0 and caps out at maxStackSize. </para>
        /// </summary>
        /// <param name="index"> Index to set change the stack size of. </param>
        /// <param name="amountToChange"> How much to attempt to add to the stack size. </param>
        /// /// <param name="updateClients"> Should setting the stackSize update clients? </param>
        /// <returns> Amount that ended up being changed. </returns>
        [Server]
        public ushort AddStackSize(ushort index, ushort amountToChange, bool updateClients = true)
        {
            ushort originalStackSize = items[index].stackSize;
            return (ushort)(SetStackSize(index, (ushort)(items[index].stackSize + amountToChange), updateClients) - originalStackSize);
        }

        /// <summary>
        /// Reduces the stack size of an item.
        /// <para>  Deletes item if stack size is 0 and caps out at maxStackSize. </para>
        /// </summary>
        /// <param name="index"> Index to set change the stack size of. </param>
        /// <param name="amountToChange"> How much to attempt to remove from the stack size. </param>
        /// /// <param name="updateClients"> Should setting the stackSize update clients? </param>
        /// <returns> Amount that ended up being changed. </returns>
        [Server]
        public ushort SubStackSize(ushort index, ushort amountToChange, bool updateClients = true)
        {
            ushort originalStackSize = items[index].stackSize;
            return (ushort)(originalStackSize - SetStackSize(index, (ushort)(items[index].stackSize - amountToChange), updateClients));
        }

        /// <summary>
        /// Sets the stack size of an item.
        /// <para> Clamped between 0 and the item's max stack size. Deletes item if stack size hits 0. </para>
        /// </summary>
        /// <param name="index"> Index to set the stack size of. </param>
        /// <param name="stackSize"> Stack size to try to set the index to. </param>
        /// <param name="updateClients"> Should changing the stackSize update clients? </param>
        /// <returns> New stack size of the item. </returns>
        [Server]
        public ushort SetStackSize(ushort index, ushort stackSize, bool updateClients = true)
        {
            //Debug.Log($"Trying to set {index} to stack size {stackSize}.");
            items[index].stackSize = (ushort)Mathf.Clamp(stackSize, 0, items[index].MaxStackSize);

            // If slot isn't emptied (which will update clients) and updateClients is true, update clients
            if (!ClearIfEmpty(index, updateClients) && updateClients)
            {
                UpdateObserversSingle(index);
            }
            //Debug.Log($"Ended up setting {index} to stack size {stackSize}.");
            return items[index].stackSize;
        }

        /// <summary>
        /// Clears this slot and updates clients if stack size is 0.
        /// </summary>
        /// <param name="index"> Index to check if empty. </param>
        /// <param name="updateClients"> Should deleting the item update clients? </param>
        /// <returns> Was the stack size 0? </returns>
        [Server]
        public bool ClearIfEmpty(ushort index, bool updateClients = true)
        {
            if (items[index].stackSize == 0) { ClearItem(index, updateClients); return true; }
            return false;
        }

        /// <summary>
        /// Sets this index to default (deletes item in slot).
        /// </summary>
        /// <param name="index"> Index to clear. </param>
        /// <param name="updateClients"> Should clients be updated of this change?</param>
        [Server]
        public void ClearItem(ushort index, bool updateClients = true)
        {
            items[index] = default;
            if (updateClients == true) { UpdateObserversSingle(index); }
        }

        /// <summary>
        /// Sets this inventory's items array length. Drops items that couldn't fit in the inventory.
        /// </summary>
        /// <param name="newLength"> The length to set items to. </param>
        [Server]
        public void SetLength(ushort newLength)
        {
            ServerSimpleItem[] newInventory = new ServerSimpleItem[newLength];
            ushort i = 0;
            foreach (ServerSimpleItem oldItem in items)
            {
                if (i < newLength)
                {
                    newInventory[i] = items[i];
                }
                else
                {
                    // Inventory became too small, drop oldItem
                }
                i++;
            }
            items = newInventory;
            UpdateObserversComplete();
        }

        /// <summary>
        /// Fully sets this inventory's items.
        /// </summary>
        /// <param name="newInventory"> The array of items to set this inventory's items to. </param>
        [Server]
        public void SetInventory(ServerSimpleItem[] newInventory)
        {
            items = newInventory;
            UpdateObserversComplete();
        }

        // ======================
        // UPDATERS
        // ======================

        #region Updaters

        /// <summary>
        /// Updates all observers of all items in this inventory.
        /// </summary>
        [Server]
        public void UpdateObserversComplete()
        {
            // For every observer of the inventory, 
            foreach (InventoryUser user in users)
            {
                user.conn.identity.gameObject.GetComponent<InventoryManager>().TargetSyncInventory(user.conn, (ushort)user.invIndex, ConvertToNetwork(user.conn.identity == ownerIdentity));
            }
        }

        /// <summary>
        /// Updates all observers of a single index in this inventory.
        /// </summary>
        /// <param name="index"> Which index should clients be updated of? </param>
        [Server]
        public void UpdateObserversSingle(ushort index)
        {
            foreach (InventoryUser user in users)
            {
                // Sync that observer's inventories[invIndex] to the index's new item
                user.conn.identity.gameObject.GetComponent<InventoryManager>().TargetSyncInventorySlot(user.conn, (ushort)user.invIndex, index, items[index].ConvertToNetwork());
            }
        }

        #endregion Updaters

    }
}