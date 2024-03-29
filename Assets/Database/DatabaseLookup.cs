using UnityEngine;
using System.Linq;

namespace WrongWarp
{
    /// <summary>
    /// Holds a Database ScriptableObject and contains functions for converting ID => SO (ByID) or SO => ID (GetID).
    /// </summary>
    public class DatabaseLookup : MonoBehaviour
    {
        [SerializeField] private Database serializedDatabase;
        public static Database database { get; private set; }

        private void Start()
        {
            database = serializedDatabase;      // Load serializedDatabase as a static Database for use in static functions
            DontDestroyOnLoad(gameObject);      // Save DatabaseLookup prefab across scenes
        }

        #region ID Lookup

        public static Item ItemByID(ushort ID)
        {
            return database.itemDatabase[ID];
        }


        public static InventoryPreset InventoryPresetByID(ushort ID)
        {
            return database.inventoryPresetDatabase[ID];
        }
        public static DisplayTabPreset TabPresetByID(ushort ID)
        {
            return database.displayTabPresetDatabase[ID];
        }
        public static BackgroundGenerator BackgroundGeneratorByID(ushort ID)
        {
            return database.backgroundGeneratorDatabase[ID];
        }
        public static PositionGenerator PositionGeneratorByID(ushort ID)
        {
            return database.positionGeneratorDatabase[ID];
        }
        public static SlotType SlotTypeByID(ushort ID)
        {
            return database.slotTypeDatabase[ID];
        }
        public static ItemType ItemTypeByID(ushort ID)
        {
            return database.itemTypeDatabase[ID];
        }

        #endregion ID Lookup

        #region Preset Lookup

        public static ushort GetID(DisplayTabPreset preset)
        {
            return (ushort)database.displayTabPresetDatabase.IndexOf(preset);
        }
        public static ushort GetID(BackgroundGenerator preset)
        {
            return (ushort)database.backgroundGeneratorDatabase.IndexOf(preset);
        }
        public static ushort GetID(PositionGenerator preset)
        {
            return (ushort)database.positionGeneratorDatabase.IndexOf(preset);
        }
        public static ushort GetID(SlotType preset)
        {
            return (ushort)database.slotTypeDatabase.IndexOf(preset);
        }
        public static ushort GetID(ItemType preset)
        {
            return (ushort)database.itemTypeDatabase.IndexOf(preset);
        }
        public static ushort GetID(InventoryPreset preset)
        {
            return (ushort)database.inventoryPresetDatabase.IndexOf(preset);
        }
        public static ushort GetID(Item preset)
        {
            return (ushort)database.itemDatabase.IndexOf(preset);
        }

        #endregion Preset Lookup

    }
}