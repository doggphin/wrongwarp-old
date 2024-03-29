using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace WrongWarp
{
    [CreateAssetMenu(fileName = "New Display Tab Preset", menuName = "Assets/Inventory/Display Tab Preset")]
    public class DisplayTabPreset : ScriptableObject
    {
        [Header("Item ID Whitelist")]
        [Tooltip("Only allow IDs inside this list inside this inventory. Cannot be used with a blacklist.")]
        [SerializeField] private ItemType[] whitelist;  

        [Space]
        [Header("Item ID Blacklist")]
        [Tooltip("Allow all IDs inside this inventory except for what's on this list. Cannot be used with a whitelist.")]
        [SerializeField] private ItemType[] blacklist;

        /// <summary> Checks if this inventory preset will take an item ID. </summary>
        public bool Accepts(ushort itemId)
        {
            if(itemId == 0) { return true; }

            if (whitelist.Length != 0)
            {
                // If a blacklist also exists, it shouldn't and will be ignored
                if (blacklist.Length != 0) { Debug.Log("ERROR: Blacklist included with a whitelist. Whitelist will override it."); }

                // Check all itemTypes included in whitelist
                foreach (ItemType itemType in whitelist)
                {
                    // Check all their IDs
                    foreach (ushort Id in itemType.items)
                    {
                        // If there's a match, return true
                        if (itemId == Id)
                        {
                            return true;
                        }
                    }
                }

                // Otherwise this ID isn't in the whitelist, return false
                Debug.Log("Inventory whitelist didn't allow this item type. Returning false.");
                return false;
            }

            if (blacklist.Length != 0)
            {
                // Check all itemTypes included in blacklist
                foreach (ItemType itemType in blacklist)
                {
                    // Check all their IDs
                    foreach (ushort Id in itemType.items)
                    {
                        // If there's a match, return false
                        if (itemId == Id) { Debug.Log("Item ID was included in the blacklist."); return false; }
                    }
                }

                // Otherwise this ID wasn't found in the blacklist, return true
                Debug.Log("Inventory blacklist allows this item type. Returning true.");
                return true;
            }

            // If no whitelist or blacklist, this is a generic inventory preset. Return true.
            return true;
        }

        [Space]
        [Header("Slot Position Generator")]
        [Tooltip("Used to generate positions of slots within a tab.")]
        [SerializeField] private PositionGenerator positionGenerator;

        /// <summary> Returns an array of this preset's slot positions of a specified length. </summary>
        public Vector2[] GetSlotPositions(DisplayPresetOverride presetOverride, int amountOfSlots, float scale = 1)
        {
            if (presetOverride.positionsGeneratorIndex == null)
            {
                return positionGenerator.GetSlotPositions(amountOfSlots, scale);
            }
            else
            {
                return DatabaseLookup.PositionGeneratorByID((ushort)presetOverride.backgroundGeneratorIndex).GetSlotPositions(amountOfSlots, scale);
            }
        }

        [Space]
        [Header("Background Sprite Generator")]
        [Tooltip("Used to get or generate a tab's background sprite.")]
        [SerializeField] private BackgroundGenerator backgroundGenerator;

        /// <summary> Returns this preset's inventory background. Include an array of simpleItems for extra effects. </summary>
        public Sprite GetBackground(DisplayPresetOverride presetOverride, int width = 0, int height = 0, ServerSimpleItem[] items = null)
        {
            if (presetOverride.backgroundGeneratorIndex == null)
            {
                return backgroundGenerator.GetBackground(width, height, items);
            }
            return DatabaseLookup.BackgroundGeneratorByID((ushort)presetOverride.backgroundGeneratorIndex).GetBackground(width, height, items);
        }

        [Space]
        [Header("Slot Visuals")]
        [Tooltip("Stores all slot sprites and other visual information.")]
        // Slot type is left public since it stores too many variables
        [SerializeField] private SlotType slotType;

        /// <summary> Returns  </summary>
        public SlotType GetSlotType(DisplayPresetOverride presetOverride)
        {
            if (presetOverride.slotTypeIndex == null)
            {
                return slotType;
            }
            return DatabaseLookup.SlotTypeByID((ushort)presetOverride.slotTypeIndex);

        }

        /// <summary> Multiplies an array of slot positions by the padding of this preset. </summary>
        public Vector2[] PadPositions(DisplayPresetOverride presetOverride, Vector2[] slotPositions)
        {
            if (presetOverride.positionsGeneratorIndex == null)
            {
                return slotType.PadPositions(slotPositions);
            }
            return DatabaseLookup.SlotTypeByID((ushort)presetOverride.slotTypeIndex).PadPositions(slotPositions);
        }

        /// <summary> Returns the correct tab size to encapsulate all slots. </summary>
        public Vector2 GetBackgroundSize(DisplayPresetOverride presetOverride, Vector2[] slotPositions)
        {
            if (presetOverride.positionsGeneratorIndex == null)
            {
                return slotType.GetBackgroundSize(slotPositions);
            }
            return DatabaseLookup.SlotTypeByID((ushort)presetOverride.slotTypeIndex).GetBackgroundSize(slotPositions);
        }

        /// <summary> Returns the size to set all slots. </summary>
        public Vector2 GetSlotSize(DisplayPresetOverride presetOverride)
        {
            if (presetOverride.positionsGeneratorIndex == null)
            {
                return slotType.GetSlotScale();
            }
            return DatabaseLookup.SlotTypeByID((ushort)presetOverride.slotTypeIndex).GetSlotScale();
        }
    }
}