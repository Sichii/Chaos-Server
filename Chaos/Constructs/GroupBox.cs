namespace Chaos
{
    internal sealed class GroupBox
    {
        internal User GroupLeader { get; set; }
        internal string Text { get; set; }
        internal byte MaxLevel { get; set; }
        internal byte[] MaxAmounts { get; set; }

        internal GroupBox(User leader, string text, byte maxLevel, byte[] maxAmounts)
        {
            GroupLeader = leader;
            Text = text;
            MaxLevel = maxLevel;
            MaxAmounts = maxAmounts;
        }
    }
}
