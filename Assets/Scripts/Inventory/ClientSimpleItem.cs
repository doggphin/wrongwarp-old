using System.Collections.Generic;

namespace WrongWarp
{
    #pragma warning disable CS0660
    #pragma warning disable CS0661
    /// <summary> A further simplified ServerSimpleItem (used to identify instances of Items) used on clients. </summary>
    public struct ClientSimpleItem
    #pragma warning restore CS0661
    #pragma warning restore CS0660
    {
        public ushort id;
        public ushort stackSize;
        public IDBinaryTags idbTags;
        public bool hasInventory;
        //public bool canBeMoved;   // Would be useful for non-cheating clients

        public ClientSimpleItem(ushort id = 0, ushort stackSize = 0, IDBinaryTags idbTags = null, bool hasInventory = false)
        {
            this.id = id;
            this.stackSize = stackSize;
            this.idbTags = idbTags;
            this.hasInventory = hasInventory;
        }

        public bool isEmpty { get { return id == 0 || stackSize == 0; } }

        public static bool operator ==(ClientSimpleItem a, ClientSimpleItem b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(ClientSimpleItem a, ClientSimpleItem b)
        {
            return !a.Equals(b);
        }
    }
}