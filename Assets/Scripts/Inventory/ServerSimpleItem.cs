using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace WrongWarp
{
    /// <summary> A simplified Item. Stored in inventories.
    /// <para> Can hold its own inventories. Stores an ID, stack size, and tags. </para></summary>
    #pragma warning disable CS0660
    #pragma warning disable CS0661
    public struct ServerSimpleItem : ItemData
    #pragma warning restore CS0661
    #pragma warning restore CS0660
    {
        public ushort id { get; set; }
        public ushort stackSize { get; set; }
        //public List<ItemTag> tags;
        public IDBinaryTags idbTags { get; set; }

        public ServerInventory itemInventory;

        public ushort MaxStackSize
        {
            get { return DatabaseLookup.ItemByID(id).maxStackSize; }
        }
        public ushort SpaceUntilFull
        {
            get { return (ushort)Mathf.Max(MaxStackSize - stackSize, 0); }
        }

        public ServerSimpleItem(ushort id = 0, ushort stackSize = 0, ServerInventory itemInventory = null, bool useDefaultTags = true, IDBinaryTags idbTags = null)
        {
            this.id = id;
            this.stackSize = stackSize;
            this.itemInventory = itemInventory;

            Item itemSO = DatabaseLookup.ItemByID(id);
            if (itemSO.inventoryPreset != null)
            {
                this.itemInventory = new ServerInventory(itemSO.inventoryPreset, null, default, default);
            }

            if(idbTags != null)
            {
                this.idbTags = idbTags; 
            }
            else
            {
                this.idbTags = new();
            }

            if(useDefaultTags && itemSO.serializedTags.Length != 0)
            {
                this.idbTags.AddSerializedTags(itemSO.serializedTags);
            }
        }

        public bool IsSameItem(ServerSimpleItem item)
        {
            if (id == item.id && idbTags == item.idbTags && itemInventory == null && item.itemInventory == null) { return true; }
            return false;
        }

        /// <summary> Returns the network version of this SimpleItem. </summary>
        public NetworkSimpleItem ConvertToNetwork()
        {  
            return new NetworkSimpleItem(id, stackSize, itemInventory != null, idbTags);
        }

        public static bool operator ==(ServerSimpleItem a, ServerSimpleItem b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ServerSimpleItem a, ServerSimpleItem b)
        {
            return !a.Equals(b);
        }
    }

    public enum TagExistsAction
    {
        Replace,
        Add,
        Skip
    }
}