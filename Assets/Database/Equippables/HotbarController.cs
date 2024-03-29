using UnityEngine;
using Mirror;
using System;

namespace WrongWarp
{
    public class HotbarController : NetworkBehaviour
    {
        [SerializeField] private EquippableHolder equippableHolder;
        [SerializeField] private InventoryManager inventoryManager;
        public BaseEquippable EquippedBaseEquippable
        {
            get
            {
                return equippableHolder.heldItem != null ? equippableHolder.heldItem.GetComponent<BaseEquippable>() : null;
            }
        }

        // -1 means no index selected
        public ushort? indexSelected = null;
        public ServerSimpleItem SelectedServerItem { get; private set; }
        public ClientSimpleItem SelectedClientItem { get; private set; }

        [SyncVar(hook = nameof(SetEquippedItem))]
        private ushort selectedID;

        public static event Action<ushort> HotbarSelected = delegate { };

        /// <summary>
        /// Subscribes this hotbar controller to hotbar-specific input actions.
        /// </summary>
        public void Init()
        {
            InputManager.UIInput.SlotSelect.Slot1.started += ctx => { ClientSelectHotbar(0); HotbarSelected(0); };
            InputManager.UIInput.SlotSelect.Slot2.started += ctx => { ClientSelectHotbar(1); HotbarSelected(1); };
            InputManager.UIInput.SlotSelect.Slot3.started += ctx => { ClientSelectHotbar(2); HotbarSelected(2); };
            InputManager.UIInput.SlotSelect.Slot4.started += ctx => { ClientSelectHotbar(3); HotbarSelected(3); };
            InputManager.UIInput.SlotSelect.Slot5.started += ctx => { ClientSelectHotbar(4); HotbarSelected(4); };
            InputManager.UIInput.SlotSelect.Slot6.started += ctx => { ClientSelectHotbar(5); HotbarSelected(5); };
            InputManager.PlayerControls.PlayerUse.Use.started += ctx => { UseItem(); };
        }

        /// <summary>
        /// Logically selects a hotbar index on the client.
        /// </summary>
        /// <param name="index"></param>
        [Client]
        private void ClientSelectHotbar(ushort index)
        {
            indexSelected = indexSelected == index ? null : index;
            ClientSimpleItem tempItem = indexSelected == -1 ? default : inventoryManager.ClientHotbar.items[index];
            equippableHolder.SetNewEquip(tempItem.id, hasAuthority || isServer);
            if (equippableHolder.heldItem) { EquippedBaseEquippable.clientSimpleItem = tempItem; }
            CmdSelectHotbar(indexSelected);
        }

        [Client]
        private void ClientUseItem()
        {
            if(indexSelected == -1) { return; }
            if (SelectedClientItem == default || EquippedBaseEquippable == null) { return; }
            EquippedBaseEquippable.Use();
        }

        /// <summary>
        /// Asks the server to change the index of the selected hotbar (inventoryManager.serverInventories[0]) item.
        /// </summary>
        /// <param name="index"> The index to select. </param>
        [Command]
        private void CmdSelectHotbar(ushort? index)
        {
            // If trying to select an index value that isn't possible or the value already being used, exit
            if (index > 5 || index == indexSelected) { return; }

            // Next, set indexSelected, selectedID and the equipped item's serverSimpleItem
            bool isDeselecting = index == null;
            indexSelected = index;
            selectedID = isDeselecting ? (ushort)0 : inventoryManager.ServerHotbar.items[(ushort)index].id;
            if (equippableHolder.heldItem) { EquippedBaseEquippable.serverSimpleItem = isDeselecting ? default : inventoryManager.ServerHotbar.items[(ushort)index]; }
                
            // If not a client or host that would have been told to set items from the selectedID syncvar hook, manually set equipped item
            if (isServerOnly) { SetEquippedItem(0, selectedID); }
        }

        /// <summary>
        /// Asks the server to use whatever currently item is currently equipped.
        /// </summary>
        private void UseItem()
        {
            if(indexSelected == null) { return; }
            var itemSelected = inventoryManager.serverInventories[1].items[(ushort)indexSelected];
            BaseEquippable baseEquippableComponent = EquippedBaseEquippable;
            if (itemSelected == default || baseEquippableComponent == null) { return; }
            baseEquippableComponent.Use();
        }

        /// <summary>
        /// Hook called when selectedID is changed on the server.
        /// </summary>
        private void SetEquippedItem(ushort oldID, ushort newID)
        {
            if(newID == selectedID && hasAuthority) { return; }
            equippableHolder.SetNewEquip(newID, hasAuthority || isServer);
        }
    }
}
