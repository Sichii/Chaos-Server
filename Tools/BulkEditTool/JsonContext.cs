using System.IO;
using BulkEditTool.Model.Tables;
using Chaos;
using Chaos.Common.Abstractions;
using Chaos.Extensions;
using Chaos.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace BulkEditTool;

public class JsonContext
{
    private static readonly IServiceProvider Services;
    public static AislingRepository Aislings { get; private set; } = null!;
    public static DialogTemplateRepository DialogTemplates { get; private set; } = null!;
    public static ItemTemplateRepository ItemTemplates { get; private set; } = null!;
    public static LootTableRepository LootTables { get; private set; } = null!;
    public static MapInstanceRepository MapInstances { get; private set; } = null!;
    public static MapTemplateRepository MapTemplates { get; private set; } = null!;
    public static MerchantTemplateRepository MerchantTemplates { get; private set; } = null!;
    public static MonsterTemplateRepository MonsterTemplates { get; private set; } = null!;
    public static ReactorTileTemplateRepository ReactorTileTemplates { get; private set; } = null!;
    public static SkillTemplateRepository SkillTemplates { get; private set; } = null!;
    public static SpellTemplateRepository SpellTemplates { get; private set; } = null!;

    static JsonContext()
    {
        var services = new ServiceCollection();

        // @formatter:off
        var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json")
                            #if DEBUG
                            .AddJsonFile("appsettings.local.json")
                            #else
                            .AddJsonFile("appsettings.prod.json")
                            //.AddJsonFile("appsettings.local.json")
                            #endif
                            ;
        // @formatter:on
        var configuration = builder.Build();
        var startup = new Startup(configuration);
        services.AddSingleton(startup.Configuration);
        services.AddLogging();

        startup.AddJsonSerializerOptions(services);

        services.AddOptionsFromConfig<ChaosOptions>(Startup.ConfigKeys.Options.Key);
        services.AddSingleton<IStagingDirectory>(sp => sp.GetRequiredService<IOptions<ChaosOptions>>().Value);

        services.AddStorage();

        Services = services.BuildServiceProvider();

        var stagingDir = Services.GetRequiredService<IStagingDirectory>();
        stagingDir.StagingDirectory = Path.Combine("..", stagingDir.StagingDirectory);

        CreateTables();
    }

    private static void CreateTables()
    {
        LootTables = ActivatorUtilities.CreateInstance<LootTableRepository>(Services);
        Aislings = ActivatorUtilities.CreateInstance<AislingRepository>(Services);
        MapInstances = ActivatorUtilities.CreateInstance<MapInstanceRepository>(Services);
        DialogTemplates = ActivatorUtilities.CreateInstance<DialogTemplateRepository>(Services);
        ItemTemplates = ActivatorUtilities.CreateInstance<ItemTemplateRepository>(Services);
        MapTemplates = ActivatorUtilities.CreateInstance<MapTemplateRepository>(Services);
        MerchantTemplates = ActivatorUtilities.CreateInstance<MerchantTemplateRepository>(Services);
        MonsterTemplates = ActivatorUtilities.CreateInstance<MonsterTemplateRepository>(Services);
        ReactorTileTemplates = ActivatorUtilities.CreateInstance<ReactorTileTemplateRepository>(Services);
        SkillTemplates = ActivatorUtilities.CreateInstance<SkillTemplateRepository>(Services);
        SpellTemplates = ActivatorUtilities.CreateInstance<SpellTemplateRepository>(Services);
    }

    internal static Task LoadAsync() => Task.WhenAll(
        LootTables.LoadAsync(),
        Aislings.LoadAsync(),
        MapInstances.LoadAsync(),
        DialogTemplates.LoadAsync(),
        ItemTemplates.LoadAsync(),
        MapTemplates.LoadAsync(),
        MerchantTemplates.LoadAsync(),
        MonsterTemplates.LoadAsync(),
        ReactorTileTemplates.LoadAsync(),
        SkillTemplates.LoadAsync(),
        SpellTemplates.LoadAsync());

    public static Task SaveChangesAsync() => Task.WhenAll(
        LootTables.SaveChangesAsync(),
        Aislings.SaveChangesAsync(),
        MapInstances.SaveChangesAsync(),
        DialogTemplates.SaveChangesAsync(),
        ItemTemplates.SaveChangesAsync(),
        MapTemplates.SaveChangesAsync(),
        MerchantTemplates.SaveChangesAsync(),
        MonsterTemplates.SaveChangesAsync(),
        ReactorTileTemplates.SaveChangesAsync(),
        SkillTemplates.SaveChangesAsync(),
        SpellTemplates.SaveChangesAsync());
}