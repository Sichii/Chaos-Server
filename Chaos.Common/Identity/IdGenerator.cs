using System.Numerics;

namespace Chaos.Common.Identity;

public class IdGenerator<T> where T : INumber<T>
{
    private T CurrentId;

    public T NextId
    {
        get
        {
            lock (this)
            {
                CurrentId++;

                return CurrentId;
            }
        }
    }
}