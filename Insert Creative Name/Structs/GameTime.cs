using System;

namespace Chaos
{
    [Serializable]
    internal struct GameTime
    {
        byte some, weird, shit;

        internal static GameTime Now { get; }

        internal DateTime ToDateTime()
        {
            return DateTime.MinValue;
        }

        internal static GameTime FromDateTime(DateTime dTime)
        {
            return new GameTime();
        }

        public override string ToString()
        {
            return $@"{some} {weird} {shit}";
        }
    }
}
