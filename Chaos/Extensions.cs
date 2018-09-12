namespace Chaos
{
    internal static class Extensions
    {
        internal static Direction Reverse(this Direction direction)
        {
            byte dir = (byte)(direction + 2);

            if (dir > 3)
                dir -= 4;

            return (Direction)dir;
        }
    }
}
