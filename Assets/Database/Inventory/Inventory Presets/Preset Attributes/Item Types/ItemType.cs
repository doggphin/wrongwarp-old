using System.Collections.Generic;
using UnityEngine;
namespace WrongWarp
{

    [CreateAssetMenu(fileName = "New Item Type", menuName = "Assets/Inventory/ItemType")]
    public class ItemType : ScriptableObject
    {
        [Header("Item Type Container")]
        [Tooltip("A list of item IDs that are of this item type.")]
        public ushort[] items;
    }
}
