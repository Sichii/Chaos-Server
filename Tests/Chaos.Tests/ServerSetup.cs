#region
using System.Runtime.CompilerServices;
using Chaos.Testing.Infrastructure;
#endregion

namespace Chaos.Tests;

internal static class ServerSetup
{
    [ModuleInitializer]
    public static void Initialize() => Testing.Infrastructure.ServerSetup.InitializeWorld();
}