using System.Collections.Generic;

namespace WrongWarp
{
    /// <summary> A simplified ServerInventory turned into a struct for communication to clients without collecting garbage. 
    /// <para> Not used directly, only useful for converting into a ClientInventory. </para> </summary>
    public struct NetworkInventory
    {
        public NetworkSimpleItem[] items;
        public bool hasParent;
        public bool hasOwnership;
        public ushort presetId;
        public DisplayPresetOverride presetOverride;

        public NetworkInventory(NetworkSimpleItem[] items = null, bool hasParent = false, ushort presetId = 0, DisplayPresetOverride presetOverride = default, bool hasOwnership = false)
        {
            this.items = items;
            this.hasParent = hasParent;
            this.presetId = presetId;
            this.presetOverride = presetOverride;
            this.hasOwnership = hasOwnership;
        }

        public ClientSimpleItem[] ItemsAsClient
        {
            get
            {
                List<ClientSimpleItem> clientItems = new();
                foreach(var item in items)
                {
                    clientItems.Add(item.AsClient);
                }
                return clientItems.ToArray();
            }
        }
    }
}