#region
using System.Runtime.CompilerServices;
#endregion

namespace Chaos.Tests;

internal static class ServerSetup
{
    [ModuleInitializer]
    public static void Initialize() => Testing.Infrastructure.ServerSetup.InitializeWorld();
}