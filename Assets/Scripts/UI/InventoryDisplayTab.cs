using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WrongWarp
{
    public class InventoryDisplayTab : MonoBehaviour, ILeftClickDroppable, IRightClickDroppable
    {
        [Header("UI Element References")]
        [SerializeField] private GameObject inventoryIcon;
        [SerializeField] private Image panelBackground;
        [SerializeField] private Transform slotsHolder;

        public ClientInventory tabInventory { get; private set; }
        private DisplayTabPreset tabPreset;
        public ushort invIndex { get; private set; }
        public List<InventoryIcon> slotPool { get; private set; } = new();

        public static event Action DroppedOnTab = delegate { };

        /// <summary> Sets the values for this display tab. </summary>
        public void Initialize(ushort invIndex, ClientInventory tabInventory)
        {
            this.invIndex = invIndex;
            this.tabInventory = tabInventory;
            tabPreset = DatabaseLookup.TabPresetByID(tabInventory.displayPresetId);

            UpdateSlotPoolSize();
            UpdateSlotLocations();
            SetStyle();
        }

        public void Display()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void UpdateSlots()
        {
            UpdateSlotPoolSize();
            UpdateSlotLocations();
            UpdateSlotVisuals();
            for(ushort i=0; i < tabInventory.items.Length; i++)
            {
                UpdateSlotData(i);
            }
        }

        public void UpdateSlotData(ushort index)
        {
            slotPool[index].SetVisualData(tabInventory.items[index]);
        }

        /// <summary> Updates every slot's position within itemSlotPool. </summary>
        public void SetStyle()
        {
            Vector2 size = GetComponent<RectTransform>().sizeDelta;
            panelBackground.sprite = tabPreset.GetBackground(tabInventory.displayPresetOverride, (int)size.x, (int)size.y);
        }

        /// <summary> Updates every slot's position within itemSlotPool. </summary>
        public void UpdateSlotLocations()
        {
            Vector2[] slotPositions = tabPreset.GetSlotPositions(tabInventory.displayPresetOverride, slotPool.Count);
            slotPositions = tabPreset.PadPositions(tabInventory.displayPresetOverride, slotPositions);
            for (int i = 0; i < slotPositions.Length; i++)
            {
                slotPool[i].SetPosition(slotPositions[i]);
            }

            ResizeBackground(slotPositions);
        }

        public void InitializeSlot(ushort index, SlotType slotType, Vector2 slotSize)
        {
            InventoryIcon inventorySlot = slotPool[index];

            inventorySlot.Initialize(invIndex, index);
            inventorySlot.SetBackgroundVisuals(slotType, slotSize);
            slotPool[index].SetVisualData(tabInventory.items[index]);
        }

        /// <summary> Sets the size of this tab using the preset ID. </summary>
        public void ResizeBackground(Vector2[] slotPositions)
        {
            gameObject.GetComponent<RectTransform>().sizeDelta = tabPreset.GetBackgroundSize(tabInventory.displayPresetOverride, slotPositions);
        }

        // Would only ever be called if preset were to change, which shouldn't really ever happen
        /// <summary> Updates the visuals of every slot within itemSlotPool. </summary>
        public void UpdateSlotVisuals()
        {
            SlotType slotTypeToPass = DatabaseLookup.TabPresetByID(tabInventory.displayPresetId).GetSlotType(tabInventory.displayPresetOverride);
            foreach (InventoryIcon slot in slotPool)
            {
                slot.SetBackgroundVisuals(
                    DatabaseLookup.TabPresetByID(tabInventory.displayPresetId).GetSlotType(tabInventory.displayPresetOverride),
                    DatabaseLookup.TabPresetByID(tabInventory.displayPresetId).GetSlotSize(tabInventory.displayPresetOverride)
                    );
            }
        }

        /// <summary> Updates the size of this tab's itemSlotPool, instantiating and destroying icon slots as needed. </summary>
        private void UpdateSlotPoolSize()
        {
            if (slotPool.Count == tabInventory.items.Length)
            {
                return;
            }

            else if (slotPool.Count < tabInventory.items.Length)
            {
                int loops = tabInventory.items.Length - slotPool.Count;
                SlotType slotType = tabPreset.GetSlotType(tabInventory.displayPresetOverride);
                Vector2 slotSize = tabPreset.GetSlotSize(tabInventory.displayPresetOverride);
                for (ushort i = 0; i < loops; i++)
                {
                    InventoryIcon newSlot = Instantiate(inventoryIcon, slotsHolder, false).GetComponent<InventoryIcon>();

                    slotPool.Add(newSlot);
                    InitializeSlot(i, slotType, slotSize);
                }
                UpdateSlotLocations();
            }
            else
            {
                while (slotPool.Count > tabInventory.items.Length)
                {
                    Destroy(slotPool[slotPool.Count - 1].gameObject);
                    slotPool.RemoveAt(slotPool.Count - 1);
                }
                UpdateSlotLocations();
            }
        }

        public void OnLeftClickDropped()
        {
            DroppedOnTab();
        }

        public void OnRightClickDropped()
        {
            DroppedOnTab();
        }
    }
}
