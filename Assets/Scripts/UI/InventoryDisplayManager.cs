using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace WrongWarp
{
    /// <summary> Handles showing, organizing, and everything else regarding the inventory display. </summary>
    public class InventoryDisplayManager : MonoBehaviour, IToggleableUI
    {
        private InventoryManager inventoryManager;
        private DraggableInventoryIcon itemSlotDraggerInstance;
        private List<InventoryDisplayTab> displayTabPool = new();

        [Header("Serialized References")]
        [SerializeField] private GameObject itemSlotDragger;
        [SerializeField] private GameObject displayTab;
        [SerializeField] public GraphicRaycaster graphicRaycaster { get; private set; }

        [Header("Constants")]
        [SerializeField] private float tabPadding;
        [SerializeField] private float tabsYOffset;

        public bool isOpen { get; private set; }
        private bool isNewLayout;

        public static event Action InventoryClosed = delegate { };

        /// <summary> Sets up this display manager and its instanced objects. </summary>
        public void Init(InventoryManager inventoryManager)
        {
            this.inventoryManager = inventoryManager;
            graphicRaycaster = GetComponent<GraphicRaycaster>();

            itemSlotDraggerInstance = Instantiate(itemSlotDragger).GetComponent<DraggableInventoryIcon>();
            itemSlotDraggerInstance.transform.SetParent(gameObject.transform, false);
            itemSlotDraggerInstance.SetEnabled(false);
        }

        // ============================================
        // IToggleableUI interface required functions
        // ============================================

        #region IToggleableUI

        public UIType uiType { get; } = UIType.InventoryMenu;

        /// <summary> Opens the inventory UI. </summary>
        public void OpenUI()
        {
            isOpen = true;

            if (isNewLayout)
            {
                OrganizeTabs();
                isNewLayout = false;
            }

            foreach (InventoryDisplayTab tab in displayTabPool) { tab.Display(); }
        }

        /// <summary> Closes the inventory UI. </summary>
        public void CloseUI()
        {
            isOpen = false;

            foreach (InventoryDisplayTab tab in displayTabPool) { if (tab.invIndex != 1) { tab.Hide(); } }
            if (itemSlotDraggerInstance != null) { itemSlotDraggerInstance.SetEnabled(false); }

            InventoryClosed();
        }

        public GraphicRaycaster GetGraphicRaycaster()
        {
            return GetComponent<GraphicRaycaster>();
        }

        #endregion IToggleableUI

        // ============================================
        // Tab Management
        // ============================================

        #region Tab Management

        /// <summary> Puts tabs in the correct position based on size and properties. </summary>
        private void OrganizeTabs()
        {
            if (!isOpen)     // If getting an update while inventory is closed, organize tabs when it's opened again
            {
                isNewLayout = true;
                return;
            }

            RectTransform tabTransform = GetTabTransform(0);
            Vector2 mainTabSize = tabTransform.sizeDelta;     // Save the position of the main tab for later
            float externalInventoriesHeight = -tabTransform.sizeDelta.y * 0.5f + tabsYOffset;                                                  // Set to the bottom of the main inventory
            float internalInventoriesHeight = -tabTransform.sizeDelta.y * 0.5f + GetTabTransform(2).sizeDelta.y + tabPadding + tabsYOffset;    // Add equipment to the height of internal inventories

            for (int i = 0; i < displayTabPool.Count; i++)
            {
                tabTransform = GetTabTransform(i);  // Store a reference to this tab's transform

                if (i < 3)  // If setting one of the main inventories, specially set its position:
                {
                    switch (i)
                    {
                        case 0:     // Tab 0 (main inventory) goes in center
                            tabTransform.anchoredPosition = new Vector2(0, tabsYOffset);
                            mainTabSize = tabTransform.sizeDelta;
                            break;
                        case 1:     // Tab 1 (hotbar) goes on bottom
                            tabTransform.anchoredPosition = new Vector2(0, -(mainTabSize.y * 0.5f + tabTransform.sizeDelta.y * 0.5f) - tabPadding + tabsYOffset);
                            break;
                        case 2:     // Tab 2 (equipment) goes on left
                            tabTransform.anchoredPosition = new Vector2(-mainTabSize.x * 0.5f - tabTransform.sizeDelta.x * 0.5f - tabPadding, -mainTabSize.y * 0.5f + tabTransform.sizeDelta.y * 0.5f + tabsYOffset);
                            break;
                    }
                    continue;
                }

                //Otherwise, check if this inventory is external or internal.
                if (displayTabPool[i].tabInventory.hasOwnership)  // If this is an internal inventory,
                {
                    Debug.Log("Has ownership over this inventory.");
                    // Set x to left of main inventory and y to being level with its bottom
                    Vector2 tabPosition = new Vector2(-mainTabSize.x * 0.5f - tabTransform.sizeDelta.x * 0.5f - tabPadding, tabTransform.sizeDelta.y * 0.5f);
                    tabPosition.y += internalInventoriesHeight;                                     // Increase tab's height by the current height of internal inventories
                    internalInventoriesHeight += tabTransform.sizeDelta.y + tabPadding;             // Increase internal inventories' height by this tab's height
                    tabTransform.anchoredPosition = tabPosition;
                }
                else
                {
                    Debug.Log("Does not have ownership over this inventory.");
                    // Set x to right of main inventory and y to being level with its bottom
                    Vector2 tabPosition = new Vector2(mainTabSize.x * 0.5f + tabTransform.sizeDelta.x * 0.5f + tabPadding, tabTransform.sizeDelta.y * 0.5f);
                    tabPosition.y += externalInventoriesHeight;                           // Increase tab's height by the current height of all the external inventories
                    externalInventoriesHeight += tabTransform.sizeDelta.y + tabPadding;    // Increase external inventories' height by this tab's height
                    tabTransform.anchoredPosition = tabPosition;
                }
            }

            for (int i = 0; i < displayTabPool.Count; i++)
            {
                GetTabTransform(i).anchoredPosition += Vector2.down * 80;
            }
        }

        /// <summary> Gets the RectTransform of displayTabPool[index]. </summary>
        private RectTransform GetTabTransform(int index)
        {
            return displayTabPool[index].GetComponent<RectTransform>();
        }

        /// <summary> Destroys any existing tabs within displayTabPool. </summary>
        public void ClearTabs()
        {
            foreach (InventoryDisplayTab tab in displayTabPool) { Destroy(tab.gameObject); }
            displayTabPool.Clear();

            OrganizeTabs();
        }

        /// <summary> Destroys displayTabPool[invIndex]. </summary>
        public void RemoveTab(ushort invIndex)
        {
            // Base inventories can't be deleted (base, hotbar, equipment)
            if (invIndex <= 2)
            {
                Debug.Log("Can't remove the main inventory or hotbar.");
                return;
            }

            Destroy(displayTabPool[invIndex].gameObject);
            displayTabPool.RemoveAt(invIndex);

            OrganizeTabs();
        }

        /// <summary> Adds a new inventory as a tab. </summary>
        public void AddTab(ushort invIndex, ClientInventory clientInventory)
        {
            InventoryDisplayTab newTab = Instantiate(displayTab, gameObject.transform, false).GetComponent<InventoryDisplayTab>();
            displayTabPool.Insert(invIndex, newTab);

            newTab.Initialize(invIndex, clientInventory);
            newTab.gameObject.SetActive(isOpen);
            OrganizeTabs();
            itemSlotDraggerInstance.transform.SetAsLastSibling();   // Make sure itemSlotDragger is on top of all other UI
        }

        /// <summary> Switches the indexes of two tabs within the displayTabPool. </summary>
        public void SwapTabIndex(ushort invIndex1, ushort invIndex2)
        {
            InventoryDisplayTab tempInvIndex1 = displayTabPool[invIndex1];
            displayTabPool[invIndex1] = displayTabPool[invIndex2];
            displayTabPool[invIndex2] = tempInvIndex1;
        }


        /// <summary> Updates a slot within a display tab. </summary>
        public void UpdateTabSlot(ushort invIndex, ushort index)
        {
            displayTabPool[invIndex].GetComponent<InventoryDisplayTab>().UpdateSlotData(index);
        }

        /// <summary> Updates all the slots within an a display tab. </summary>
        public void UpdateTabSlots(ushort invIndex)
        {
            displayTabPool[invIndex].GetComponent<InventoryDisplayTab>().UpdateSlots();
        }

        #endregion Tab Management

        // ============================================
        // Inventory Manager Calls
        // ============================================

        #region Inventory Manager Calls

        /// <summary> Sets the properties of the item slot dragger and displays it. </summary>
        public void PickUpSlot(ushort invIndex, ushort index, ushort amount)
        {
            InventoryIcon iconToPickUp = displayTabPool[invIndex].slotPool[index];
            Item itemToLookUp = DatabaseLookup.ItemByID(displayTabPool[invIndex].tabInventory.items[index].id);

            itemSlotDraggerInstance.SetItemData(iconToPickUp.itemImage.sprite, itemToLookUp.GetSlotStackText(amount), itemToLookUp.GetSlotStackColor(amount));
            itemSlotDraggerInstance.SetEnabled(true);
        }

        public void DropSlot()
        {
            itemSlotDraggerInstance.SetEnabled(false);
        }

        /// <summary> Requests to the inventoryManager to open the inventory of an item. </summary>
        public void OpenItemInventory(ushort invIndex, ushort index)
        {
            inventoryManager.CmdOpenItemInventory(invIndex, index);
        }

        #endregion Inventory Manager Calls

    }
}