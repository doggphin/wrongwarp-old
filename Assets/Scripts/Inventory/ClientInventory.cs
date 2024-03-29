namespace WrongWarp
{
    /// <summary> The necessary information for a client to display an inventory. Carbon copy of NetworkInventory in class form for less intensive modification. </summary>
    public class ClientInventory
    {
        public ClientSimpleItem[] items;
        public bool hasParent;
        public bool hasOwnership;
        public ushort displayPresetId;
        public DisplayPresetOverride displayPresetOverride;

        public ClientInventory(NetworkInventory networkInventory)
        {
            items = networkInventory.ItemsAsClient;
            hasParent = networkInventory.hasParent;
            hasOwnership = networkInventory.hasOwnership;
            displayPresetId = networkInventory.presetId;
            displayPresetOverride = networkInventory.presetOverride;
        }
    }
}