using UnityEngine;

namespace WrongWarp
{
    public interface ItemData
    {
        public ushort id { get; set; }
        public ushort stackSize { get; set; }
        public IDBinaryTags idbTags { get; set; }
    }
}
