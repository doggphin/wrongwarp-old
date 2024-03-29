namespace WrongWarp
{
    #pragma warning disable CS0660
    #pragma warning disable CS0661
    public struct DisplayPresetOverride
    #pragma warning restore CS0661
    #pragma warning restore CS0660
    {
        public ushort? backgroundGeneratorIndex;
        public ushort? positionsGeneratorIndex;
        public ushort? slotTypeIndex;

        public DisplayPresetOverride(ushort? backgroundGeneratorIndex = null, ushort? positionsGeneratorIndex = null, ushort? slotTypeIndex = null)
        {
            this.backgroundGeneratorIndex = backgroundGeneratorIndex;
            this.positionsGeneratorIndex = positionsGeneratorIndex;
            this.slotTypeIndex = slotTypeIndex;
        }

        public static bool operator ==(DisplayPresetOverride a, DisplayPresetOverride b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(DisplayPresetOverride a, DisplayPresetOverride b)
        {
            return !a.Equals(b);
        }
    }
}
