using System.Linq.Expressions;
using FluentAssertions;

namespace Chaos.Extensions.Common.Tests;

public sealed class StringProcessTests
{
    [Fact]
    public void stuff()
    {
        var ctor = typeof(Bonk).GetConstructor(new[] { typeof(string), typeof(object[]) });

        var strParam = Expression.Parameter(typeof(string));
        var objsParam = Expression.Parameter(typeof(object[]));

        var call = Expression.New(ctor!, strParam, objsParam);

        var lambda = Expression.Lambda<Func<string, object[], object>>(call, strParam, objsParam).Compile();

        var obj1 = lambda("beep {Boop}", new[] { (object)"things" });
        var obj2 = lambda("beep {Boop} {Bonk}", new[] { (object)"things", "binks" });
    }

    [Fact]
    public void Test1()
    {
        const string STR = "{{One}} is {One}";

        var result = STR.Inject(1);

        result.Should().Be("{One} is 1");
    }

    public class Bonk
    {
        public object[] Objs { get; }
        public string Str { get; }

        public Bonk(string beep, params object[] objs)
        {
            Str = beep;
            Objs = objs;
        }
    }
}