namespace Chaos.Core.Utilities;

public static class InterlockedEx
{
    public static double Add(ref double location1, double value)
    {
        var newCurrentValue = location1;

        while (true)
        {
            var currentValue = newCurrentValue;
            var newValue = currentValue + value;
            newCurrentValue = Interlocked.CompareExchange(ref location1, newValue, currentValue);

            if (newCurrentValue.Equals(currentValue))
                return newValue;
        }
    }

    public static float Add(ref float location1, float value)
    {
        var newCurrentValue = location1;

        while (true)
        {
            var currentValue = newCurrentValue;
            var newValue = currentValue + value;
            newCurrentValue = Interlocked.CompareExchange(ref location1, newValue, currentValue);

            if (newCurrentValue.Equals(currentValue))
                return newValue;
        }
    }
}