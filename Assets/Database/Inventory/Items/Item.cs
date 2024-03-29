using UnityEngine;
using System.Collections.Generic;

namespace WrongWarp
{

    [CreateAssetMenu(fileName = "New Item", menuName = "Assets/Item")]
    public class Item : ScriptableObject
    {
        [Header("Base Item Information")]
        public ushort itemID;



        public ushort maxStackSize;

        [Space]
        [Header("Item Visuals")]
        public Sprite itemSprite;
        public string itemName;
        [TextArea] public string itemDescription;

        [Space]
        [Header("Item Inventory")]
        public InventoryPreset inventoryPreset;

        [Space]
        [Header("Usable Item Info")]
        public Equippable equippable;

        [Space]
        [Header("Item Tags")]
        public SerializedTag[] serializedTags;

        /// <summary> Returns the stack size text to display for a given SimpleItem version of this item. </summary>
        public string GetSlotStackText(ushort stackSize)
        {
            if (stackSize == 1 && maxStackSize == 1)
            {
                return "";
            }
            else
            {
                return stackSize.ToString();
            }
        }

        /// <summary> Returns the stack size text color to use for a given item. </summary>
        public Color GetSlotStackColor(ushort stackSize)
        {
            if (stackSize <= 0)                    // A non-0 item ID with a stackSize of 0 or below shouldn't exist, set color to red
            {
                return Color.red;
            }
            else if (stackSize == maxStackSize)    // Max stack size color is green
            {
                return Color.green;
            }
            else if (stackSize < maxStackSize)     // Below max stack size color is white
            {
                return Color.white;
            }
            else                                   // Otherwise this item's stackSize must be above its max stack size, set color to red
            {
                return Color.red;
            }
        }
    }
}