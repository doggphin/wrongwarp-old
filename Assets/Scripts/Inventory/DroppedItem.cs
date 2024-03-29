using Mirror;
using UnityEngine;

namespace WrongWarp
{
    public class DroppedItem : NetworkBehaviour, IInteractable
    {
        public string myName = "Unassigned Item Name";
        public ServerSimpleItem item;
        public bool hasBeenTaken = false;

        /// <summary> Interact with this object to pick it up. If it's fully depleted, it deletes itself. </summary>
        [Command(requiresAuthority = false)]
        public void CmdInteract(NetworkConnectionToClient sender = null)
        {
            InventoryManager senderInventory = sender.identity.GetComponent<InventoryManager>();
            item.stackSize -= senderInventory.serverInventories[0].AddItem(item);

            if (item.stackSize == 0)
            {
                hasBeenTaken = true;
                GetComponent<NetworkEntity>().ServerKill();
            }
        }

        public string ReturnText()
        {
            return $"Take {myName}";
        }
    }
}