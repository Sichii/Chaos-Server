namespace Chaos.Common.Utilities;

public static class InterlockedEx
{
    public static double Add(ref double source, double value)
    {
        var newCurrentValue = source;

        while (true)
        {
            var currentValue = newCurrentValue;
            var newValue = currentValue + value;
            newCurrentValue = Interlocked.CompareExchange(ref source, newValue, currentValue);

            if (newCurrentValue.Equals(currentValue))
                return newValue;
        }
    }

    public static float Add(ref float source, float value)
    {
        var newCurrentValue = source;

        while (true)
        {
            var currentValue = newCurrentValue;
            var newValue = currentValue + value;
            newCurrentValue = Interlocked.CompareExchange(ref source, newValue, currentValue);

            if (newCurrentValue.Equals(currentValue))
                return newValue;
        }
    }

    public static T SetReference<T>(ref T source, Func<T> valueFactory) where T: class
    {
        var newCurrentValue = source;

        while (true)
        {
            var currentValue = newCurrentValue;
            var newValue = valueFactory();
            newCurrentValue = Interlocked.CompareExchange(ref source, newValue, currentValue);

            if (newCurrentValue.Equals(currentValue))
                return newValue;
        }
    }

    public static int SetValue(ref int source, Func<int> valueFactory)
    {
        var newCurrentValue = source;

        while (true)
        {
            var currentValue = newCurrentValue;
            var newValue = valueFactory();
            newCurrentValue = Interlocked.CompareExchange(ref source, newValue, currentValue);

            if (newCurrentValue.Equals(currentValue))
                return newValue;
        }
    }

    public static uint SetValue(ref uint source, Func<uint> valueFactory)
    {
        var newCurrentValue = source;

        while (true)
        {
            var currentValue = newCurrentValue;
            var newValue = valueFactory();
            newCurrentValue = Interlocked.CompareExchange(ref source, newValue, currentValue);

            if (newCurrentValue.Equals(currentValue))
                return newValue;
        }
    }

    public static double SetValue(ref double source, Func<double> valueFactory)
    {
        var newCurrentValue = source;

        while (true)
        {
            var currentValue = newCurrentValue;
            var newValue = valueFactory();
            newCurrentValue = Interlocked.CompareExchange(ref source, newValue, currentValue);

            if (newCurrentValue.Equals(currentValue))
                return newValue;
        }
    }

    public static float SetValue(ref float source, Func<float> valueFactory)
    {
        var newCurrentValue = source;

        while (true)
        {
            var currentValue = newCurrentValue;
            var newValue = valueFactory();
            newCurrentValue = Interlocked.CompareExchange(ref source, newValue, currentValue);

            if (newCurrentValue.Equals(currentValue))
                return newValue;
        }
    }

    public static long SetValue(ref long source, Func<long> valueFactory)
    {
        var newCurrentValue = source;

        while (true)
        {
            var currentValue = newCurrentValue;
            var newValue = valueFactory();
            newCurrentValue = Interlocked.CompareExchange(ref source, newValue, currentValue);

            if (newCurrentValue.Equals(currentValue))
                return newValue;
        }
    }

    public static ulong SetValue(ref ulong source, Func<ulong> valueFactory)
    {
        var newCurrentValue = source;

        while (true)
        {
            var currentValue = newCurrentValue;
            var newValue = valueFactory();
            newCurrentValue = Interlocked.CompareExchange(ref source, newValue, currentValue);

            if (newCurrentValue.Equals(currentValue))
                return newValue;
        }
    }
}