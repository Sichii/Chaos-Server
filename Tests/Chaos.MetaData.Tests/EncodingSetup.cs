#region
using System.Runtime.CompilerServices;
using System.Text;
#endregion

namespace Chaos.MetaData.Tests;

internal static class EncodingSetup
{
    [ModuleInitializer]
    public static void Initialize() => Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
}