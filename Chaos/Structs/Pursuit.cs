namespace Chaos
{
    internal struct Pursuit
    {
        internal string Text { get; }
        internal PursuitIds PursuitId { get; }
        internal ushort DialogId { get; }

        internal Pursuit(string text, PursuitIds pursuitId, ushort dialogId)
        {
            Text = text;
            PursuitId = pursuitId;
            DialogId = dialogId;
        }
    }
}
