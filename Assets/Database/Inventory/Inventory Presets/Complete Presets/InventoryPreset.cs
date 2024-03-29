using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WrongWarp
{
    [CreateAssetMenu(fileName = "New Inventory Preset", menuName = "Assets/Inventory/Inventory Preset")]
    public class InventoryPreset : ScriptableObject
    {
        [Header("Defaults")]
        [Tooltip("How large is this inventory?")]
        [SerializeField] public short length;

        [Tooltip("What preset does this use?")]
        [SerializeField] public DisplayTabPreset displayTabPreset;
    }
}