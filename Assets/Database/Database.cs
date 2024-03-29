using System.Collections.Generic;
using UnityEngine;

namespace WrongWarp
{
    [CreateAssetMenu(fileName = "New Database", menuName = "Assets/Database")]
    public class Database : ScriptableObject
    {
        // ===================================
        // Item Data
        // ===================================

        #region Item Data

        public List<Item> itemDatabase;

        #endregion Item Data

        // ===================================
        // Inventory Data
        // ===================================

        #region Inventory Data

        public List<InventoryPreset> inventoryPresetDatabase;
        public List<DisplayTabPreset> displayTabPresetDatabase;

        public List<BackgroundGenerator> backgroundGeneratorDatabase;
        public List<PositionGenerator> positionGeneratorDatabase;
        public List<SlotType> slotTypeDatabase;
        public List<ItemType> itemTypeDatabase;

        #endregion Inventory Data

        // ===================================
        // Inventory Data
        // ===================================
    }
}