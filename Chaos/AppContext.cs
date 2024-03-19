using Chaos.Scripting.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;

namespace Chaos;

public static class AppContext
{
    public static IServiceProvider Provider { get; set; } = null!;
    public static ISimpleCache CacheProvider => Provider.GetRequiredService<ISimpleCache>();
    public static IScriptProvider ScriptProvider => Provider.GetRequiredService<IScriptProvider>();
    public static ITypeMapper TypeMapper => Provider.GetRequiredService<ITypeMapper>();
}