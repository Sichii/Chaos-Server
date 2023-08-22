using Chaos.Common.Definitions;

namespace Chaos.Utilities;

public sealed class NameComposer
{
    private readonly string BaseName;
    private readonly bool IsDyeable;
    public DisplayColor Color { get; private set; }
    public string ComposedName { get; private set; }
    public string? CustomName { get; private set; }
    public string? Prefix { get; private set; }
    public string? Suffix { get; private set; }

    public NameComposer(string baseName, bool isDyeable = false)
    {
        BaseName = baseName;
        ComposedName = baseName;
        IsDyeable = isDyeable;
    }

    public static implicit operator string(NameComposer composer) => composer.ComposedName;
    public static implicit operator NameComposer(string baseName) => new(baseName);

    private void Recompose()
    {
        if (!string.IsNullOrEmpty(CustomName))
        {
            ComposedName = CustomName;

            return;
        }

        var resultName = BaseName;

        if (!string.IsNullOrEmpty(Prefix))
            resultName = $"{Prefix} {resultName}";

        if (!string.IsNullOrEmpty(Suffix))
            resultName = $"{resultName} {Suffix}";

        if (IsDyeable && (Color != DisplayColor.Default))
            resultName = $"{Color} {resultName}";

        ComposedName = resultName;
    }

    public void SetColor(DisplayColor color)
    {
        Color = color;
        Recompose();
    }

    public void SetCustomName(string? customName)
    {
        CustomName = customName;
        Recompose();
    }

    public void SetPrefix(string? prefix)
    {
        Prefix = prefix;
        Recompose();
    }

    public void SetSuffix(string? suffix)
    {
        Suffix = suffix;
        Recompose();
    }
}