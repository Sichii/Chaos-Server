using System.Collections;
using System.IO;
using System.Windows.Controls;
using Chaos.Common.Converters;
using Chaos.Extensions.Common;
using ChaosTool.Model;

// ReSharper disable MemberCanBeMadeStatic.Global

namespace ChaosTool.Controls.Abstractions;

public abstract class PropertyEditorBase : UserControl
{
    protected virtual bool ValidatePreSave<T>(TraceWrapper<T> wrapper, TextBox pathTbox, TextBox templateKeyTbox)
    {
        var fileName = Path.GetFileNameWithoutExtension(pathTbox.Text);

        if (!fileName.EqualsI(templateKeyTbox.Text))
            return false;

        if (!wrapper.Path.EqualsI(pathTbox.Text))
            if (File.Exists(wrapper.Path))
                File.Delete(wrapper.Path);

        return true;
    }

    #region Primitive Manipulation
    protected IEnumerable<string> GetEnumNames<T>()
    {
        var type = typeof(T);
        var underlyingType = Nullable.GetUnderlyingType(type);

        if (underlyingType is not null)
            type = underlyingType;

        if (!type.IsEnum)
            throw new InvalidOperationException($"{type.Name} is not an enum");

        if (underlyingType is not null)
            return Enum.GetNames(type).Prepend(string.Empty);

        return Enum.GetNames(type);
    }

    protected T? ParsePrimitive<T>(string? str)
    {
        if (string.IsNullOrEmpty(str))
            return default;

        return PrimitiveConverter.Convert<T>(str);
    }

    protected string SelectPrimitive<T>(T value, IEnumerable enumerable)
    {
        var strings = enumerable.OfType<string>();

        if (value is null)
            return strings.First(string.IsNullOrEmpty);

        var valueStr = value.ToString()!;

        return strings.First(str => str.EqualsI(valueStr));
    }
    #endregion
}