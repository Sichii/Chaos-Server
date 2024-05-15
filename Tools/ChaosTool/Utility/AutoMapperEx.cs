using AutoMapper;

public static class AutoMapperEx
{
    private const string INVALID_OPERATION_MESSAGE
        = "Mapper not initialized. Call Initialize with appropriate configuration. If you are trying to use mapper instances through a container or otherwise, make sure you do not have any calls to the static Mapper.Map methods, and if you're using ProjectTo or UseAsDataSource extension methods, make sure you pass in the appropriate IConfigurationProvider instance.";

    private const string ALREADY_INITIALIZED = "Mapper already initialized. You must call Initialize once per application domain/process.";

    private static IConfigurationProvider _configuration = null!;
    private static IMapper _instance = null!;

    private static IConfigurationProvider Configuration
    {
        get => _configuration ?? throw new InvalidOperationException(INVALID_OPERATION_MESSAGE);
        set => _configuration = _configuration == null ? value : throw new InvalidOperationException(ALREADY_INITIALIZED);
    }

    public static IMapper Mapper
    {
        get => _instance ?? throw new InvalidOperationException(INVALID_OPERATION_MESSAGE);
        private set => _instance = value;
    }

    public static void AssertConfigurationIsValid() => Configuration.AssertConfigurationIsValid();

    public static void Initialize(Action<IMapperConfigurationExpression> config) => Initialize(new MapperConfiguration(config));

    public static void Initialize(MapperConfiguration config)
    {
        Configuration = config;
        Mapper = Configuration.CreateMapper();
    }

    public static TDestination MapPropertiesToInstance<TDestination>(this object source, TDestination destination)
    {
        Mapper.Map(source, destination);

        return destination;
    }

    public static TDestination MapTo<TDestination>(this object source) => Mapper.Map<TDestination>(source)!;
}