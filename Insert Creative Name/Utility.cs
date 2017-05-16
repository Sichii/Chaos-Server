using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insert_Creative_Name
{
    internal static class Utility
    {
        private static Random m_random = new Random();

        internal static int Random()
        {
            return m_random.Next();
        }

        internal static int Random(int maxValue)
        {
            return m_random.Next(maxValue);
        }

        internal static int Random(int minValue, int maxValue)
        {
            return m_random.Next(minValue, maxValue);
        }
    }
}
