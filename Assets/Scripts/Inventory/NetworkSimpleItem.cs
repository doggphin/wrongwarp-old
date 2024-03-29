using System.Collections.Generic;
using UnityEngine;

namespace WrongWarp
{
    public struct NetworkSimpleItem
    {
        public ushort id;
        public ushort stackSize;
        public NetworkIDBTags networkIDBTags;
        public bool hasInventory;

        public NetworkSimpleItem(ushort id = 0, ushort stackSize = 0, bool hasInventory = false, IDBinaryTags idbTags = null)
        {
            this.id = id;
            this.stackSize = stackSize;
            this.hasInventory = hasInventory;
            networkIDBTags = new NetworkIDBTags(idbTags);
        }

        /// <summary>
        /// Returns this NetworkItem as a ClientSimpleItem.
        /// </summary>
        public ClientSimpleItem AsClient
        {
            get
            {
                return new ClientSimpleItem(id, stackSize, networkIDBTags.AsIDBinaryTags, hasInventory);
            }
        }
    }

    public struct NetworkIDBTags
    {
        public ushort[] ids;
        public Tag[] tags;

        public NetworkIDBTags(IDBinaryTags idbTags)
        {
            if(idbTags == null)
            {
                this.ids = null;
                this.tags = null;
                return;
            }

            List<ushort> ids = new();
            List<Tag> tags = new();
            foreach(var idbTag in idbTags.IDBTags)
            {
                ids.Add(idbTag.Key);
                tags.Add(idbTag.Value);
            }
            this.ids = ids.ToArray();
            this.tags = tags.ToArray();
        }

        public IDBinaryTags AsIDBinaryTags
        {
            get
            {
                if(ids == null || tags == null) { return null; }
                Dictionary<ushort, Tag> temp = new();
                for(ushort i=0; i < Mathf.Min(ids.Length, tags.Length); i++)
                {
                    temp.Add(ids[i], tags[i]);
                }
                return new IDBinaryTags(temp);
            }
        }
    }
}
