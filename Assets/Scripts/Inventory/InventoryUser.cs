using Mirror;

namespace WrongWarp
{
    public struct InventoryUser
    {
        public NetworkConnection conn;
        public ushort? invIndex;

        public bool canGive;
        public bool canTake;

        public InventoryUser(NetworkConnection conn = null, ushort? invIndex = null, bool canGive = true, bool canTake = true)
        {
            this.conn = conn;
            this.invIndex = invIndex;
            this.canGive = canGive;
            this.canTake = canTake;
        }
    }
}