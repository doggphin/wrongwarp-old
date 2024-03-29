using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WrongWarp
{
    namespace WrongWarp
    {
        public class ItemDisplayPanel : MonoBehaviour
        {
            [Header("Icon Properties")]
            public ushort myInventoryIndex;
            public TMP_Text stackSize;
            public Image image;

            [Header("Item Information")]
            public TMP_Text itemName;
            public TMP_Text itemDescription;

            [Header("References")]
            public GameObject openButton;
            public InventoryDisplayManager inventoryDisplayManager;         // Set on instantiation
            public InventoryDisplayTab myInventoryDisplayTab;

            public void OpenButtonPressed(ushort index)
            {
                inventoryDisplayManager.OpenItemInventory(myInventoryIndex, index);
            }
        }
    }
}