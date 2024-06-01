using Chaos.Extensions.Common;

namespace Chaos.Site.Extensions;

public static class AgGridExtensions
{
    public static object GetAgGridProperties(this Type type, string pinnedColName, Action<Type>? config = null)
    {
        var ret = type.GetProperties()
                      .Select(
                          prop =>
                          {
                              var propType = prop.PropertyType;

                              if (Nullable.GetUnderlyingType(propType) is { } underlyingType)
                                  propType = underlyingType;

                              var identifier = prop.Name.EqualsI(pinnedColName);

                              return new
                              {
                                  headerName = prop.Name,
                                  field = prop.Name.ToLower(),
                                  filter = (propType == typeof(bool)) || propType is not { IsValueType: true, IsEnum: false }
                                      ? "agTextColumnFilter"
                                      : "agNumberColumnFilter",
                                  pinned = identifier ? "left" : null,
                                  autoHeight = true
                              };
                          });

        return ret;
    }
}