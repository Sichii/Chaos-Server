#region
using System.Reflection;
using System.Text;
using Chaos.Extensions.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using NJsonSchema;
using NJsonSchema.Generation;
using NJsonSchema.NewtonsoftJson.Generation;
#endregion

namespace SchemaGenerator;

/// <summary>
///     Schema processor that handles custom types that use System.Text.Json converters
/// </summary>
public sealed class CustomTypeSchemaProcessor : ISchemaProcessor
{
    private static readonly HashSet<string> GeometryTypes =
    [
        "Chaos.Geometry.Abstractions.ILocation",
        "Chaos.Geometry.Abstractions.Definitions.Location",
        "Chaos.Geometry.Abstractions.IPoint",
        "Chaos.Geometry.Abstractions.Definitions.Point",
        "Chaos.Geometry.Abstractions.IPolygon",
        "Chaos.Geometry.Abstractions.Definitions.Polygon",
        "Chaos.Geometry.Abstractions.IRectangle",
        "Chaos.Geometry.Abstractions.Definitions.Rectangle"
    ];

    public void Process(SchemaProcessorContext context)
    {
        var type = context.ContextualType.Type;
        var fullName = type.FullName;

        // Check if this is one of our geometry types
        if ((fullName != null) && GeometryTypes.Contains(fullName))
        {
            // Handle each type based on how it's actually serialized
            if (fullName.Contains("Rectangle"))
            {
                // Rectangle is serialized as an object with Top, Left, Width, Height properties
                context.Schema.Type = JsonObjectType.Object;
                context.Schema.Properties.Clear();

                context.Schema.Properties["Top"] = new JsonSchemaProperty
                {
                    Type = JsonObjectType.Number
                };

                context.Schema.Properties["Left"] = new JsonSchemaProperty
                {
                    Type = JsonObjectType.Number
                };

                context.Schema.Properties["Width"] = new JsonSchemaProperty
                {
                    Type = JsonObjectType.Number
                };

                context.Schema.Properties["Height"] = new JsonSchemaProperty
                {
                    Type = JsonObjectType.Number
                };
                context.Schema.RequiredProperties.Add("Top");
                context.Schema.RequiredProperties.Add("Left");
                context.Schema.RequiredProperties.Add("Width");
                context.Schema.RequiredProperties.Add("Height");
                context.Schema.Description = "Rectangle with Top, Left, Width, and Height properties";
            } else if (fullName.Contains("Polygon"))
            {
                // Polygon is serialized as an array of point strings
                context.Schema.Type = JsonObjectType.Array;

                context.Schema.Item = new JsonSchema
                {
                    Type = JsonObjectType.String,
                    Description = "Point in format: '(X, Y)' (e.g., '(50, 100)')"
                };
                context.Schema.Description = "Array of points, each in format '(X, Y)'";
                context.Schema.Properties.Clear();
            } else
            {
                // Point and Location are serialized as strings
                context.Schema.Type = JsonObjectType.String;
                context.Schema.Description = GetDescriptionForType(fullName);
                context.Schema.Properties.Clear();
            }

            // Clear any other generated schema parts
            context.Schema.AllOf.Clear();
            context.Schema.AnyOf.Clear();
            context.Schema.OneOf.Clear();
        }

        // Check if this is DynamicVars or StaticVars
        if (fullName is "Chaos.Common.Collections.DynamicVars" or "Chaos.Common.Collections.StaticVars")
        {
            // Both are serialized as dictionaries with string keys and any values
            context.Schema.Type = JsonObjectType.Object;

            context.Schema.AdditionalPropertiesSchema = new JsonSchema
            {
                // Allow any type of value
                Description = "Any valid JSON value"
            };

            context.Schema.Description = fullName.Contains("DynamicVars")
                ? "Dynamic variables stored as key-value pairs"
                : "Static variables stored as key-value pairs";

            // Clear any auto-generated properties
            context.Schema.Properties.Clear();
            context.Schema.AllOf.Clear();
            context.Schema.AnyOf.Clear();
            context.Schema.OneOf.Clear();
        }
    }

    private static string GetDescriptionForType(string fullName)
        => fullName switch
        {
            _ when fullName.Contains("Location") => "Location in format: 'mapInstanceId:(X, Y)' (e.g., 'map1:(50, 100)')",
            _ when fullName.Contains("Point")    => "Point in format: '(X, Y)' (e.g., '(50, 100)')",
            _                                    => "Serialized as string"
        };
}

public static class JsonSchemaGenerator
{
    private static string ConvertToKebabCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = new StringBuilder();

        for (var i = 0; i < input.Length; i++)
        {
            var ch = input[i];

            if (char.IsUpper(ch))
            {
                if (((i > 0) && !char.IsUpper(input[i - 1])) || ((i > 0) && (i < (input.Length - 1)) && !char.IsUpper(input[i + 1])))
                    result.Append('-');

                result.Append(char.ToLowerInvariant(ch));
            } else
                result.Append(ch);
        }

        return result.ToString();
    }

    public static async Task GenerateAllSchemasAsync(string? schemasProjectPath = null, string? outputPath = null)
    {
        schemasProjectPath ??= AppDomain.CurrentDomain.BaseDirectory;
        outputPath ??= schemasProjectPath;

        // Load the Chaos.Schemas assembly
        var schemasAssemblyPath = Path.Combine(
            schemasProjectPath,
            "bin",
            "Debug",
            "net10.0",
            "Chaos.Schemas.dll");
        var assembly = Assembly.LoadFrom(schemasAssemblyPath);

        var schemaTypes = assembly.GetTypes()
                                  .Where(t => t is { IsClass: true, IsAbstract: false, Namespace: not null }
                                              && t.Namespace.StartsWithI("Chaos.Schemas.Templates")
                                              && t.Name.EndsWithI("Schema"))
                                  .ToList();

        // Configure JSON serializer settings to handle enums as strings
        // StringEnumConverter automatically handles flag enums as comma-delimited strings
        var serializerSettings = new JsonSerializerSettings
        {
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter
                {
                    AllowIntegerValues = false // Force all enums to be strings only
                }
            },
            ContractResolver = new DefaultContractResolver()
        };

        var settings = new NewtonsoftJsonSchemaGeneratorSettings
        {
            FlattenInheritanceHierarchy = false,
            GenerateAbstractSchemas = true,
            DefaultReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull,
            DefaultDictionaryValueReferenceTypeNullHandling = ReferenceTypeNullHandling.NotNull,
            GenerateEnumMappingDescription = true,
            TypeNameGenerator = new DefaultTypeNameGenerator(),
            SchemaNameGenerator = new DefaultSchemaNameGenerator(),
            GenerateExamples = false,
            ExcludedTypeNames = [],
            GenerateCustomNullableProperties = false,
            SchemaType = SchemaType.JsonSchema,
            GenerateAbstractProperties = false,
            IgnoreObsoleteProperties = false,
            AllowReferencesWithProperties = false,
            GenerateXmlObjects = false,
            SerializerSettings = serializerSettings
        };

        // Add schema processor to handle custom types (geometry, DynamicVars, StaticVars)
        settings.SchemaProcessors.Add(new CustomTypeSchemaProcessor());

        foreach (var schemaType in schemaTypes)
            try
            {
                await GenerateSchemaForTypeAsync(schemaType, outputPath, settings);
            } catch (Exception ex)
            {
                Console.WriteLine($"Failed to generate schema for {schemaType.Name}: {ex.Message}");
            }
    }

    private static async Task GenerateSchemaForTypeAsync(Type type, string outputPath, NewtonsoftJsonSchemaGeneratorSettings settings)
    {
        var schema = JsonSchema.FromType(type, settings);

        // Create schemas directory in the output path
        var schemasDir = Path.Combine(outputPath, "schemas");
        Directory.CreateDirectory(schemasDir);

        // Generate the file name (convert PascalCase to kebab-case)
        var fileName = ConvertToKebabCase(type.Name.Replace("Schema", "")) + ".schema.json";
        var filePath = Path.Combine(schemasDir, fileName);

        // Generate JSON with proper formatting
        var json = schema.ToJson();
        await File.WriteAllTextAsync(filePath, json);

        Console.WriteLine($"Generated schema: {filePath}");
    }
}