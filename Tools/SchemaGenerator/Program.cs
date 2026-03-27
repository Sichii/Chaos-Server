#region
using SchemaGenerator;
#endregion

var schemasProjectPath = Path.GetFullPath(
    Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "..",
        "..",
        "..",
        "..",
        "..",
        "Chaos.Schemas"));

// Check if an output path was provided as command line argument
string outputPath;

if (args.Length > 0)
{
    outputPath = Path.GetFullPath(args[0]);
    Console.WriteLine($"Using custom output path: {outputPath}");
} else
{
    outputPath = schemasProjectPath;
    Console.WriteLine($"Using default output path: {outputPath}");
}

Console.WriteLine("========================================");

await JsonSchemaGenerator.GenerateAllSchemasAsync(schemasProjectPath, outputPath);

Console.WriteLine("========================================");
Console.WriteLine("Schema generation complete!");