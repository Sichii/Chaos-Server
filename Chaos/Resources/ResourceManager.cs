using System.Reflection;

namespace Chaos.Resources;

public static class ResourceManager
{
    public static byte[] ChaosIcon => ReadResource("Chaos.Resources.chaos.ico");
    public static byte[] Sotp => ReadResource("Chaos.Resources.sotp.dat");

    private static byte[] ReadResource(string resouceName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        using var resourceStream = assembly.GetManifestResourceStream(resouceName);
        using var buffer = new MemoryStream();

        resourceStream!.CopyTo(buffer);

        return buffer.ToArray();
    }
}