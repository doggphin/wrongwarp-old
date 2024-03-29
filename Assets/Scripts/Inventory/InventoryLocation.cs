using UnityEngine;

namespace WrongWarp
{
    /// <summary> A struct that stores a pointer to an inventory and index. </summary>
    #pragma warning disable CS0660
    #pragma warning disable CS0661
    public struct InventoryLocation
    #pragma warning restore CS0661
    #pragma warning restore CS0660
    {
        public ServerInventory inventory;
        public ushort? index;

        public InventoryLocation(ServerInventory inventory, ushort? index)
        {
            this.inventory = inventory;
            this.index = index;
        }

        public static bool operator ==(InventoryLocation a, InventoryLocation b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(InventoryLocation a, InventoryLocation b)
        {
            return !a.Equals(b);
        }
    }
}
