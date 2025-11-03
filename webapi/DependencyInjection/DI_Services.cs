using core_mandado.models;
using core_mandado.repositories;
using dbaccess;
using repositories;
using repositories.infoSchema;

namespace api_mandado.DependencyInjection;

public sealed class DI_Services
{
    private static DI_Services _instance;
    public static DI_Services instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DI_Services();
            }
            return _instance;

        }
    }
    private DI_Services() { }

    public void AddDependencies(IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();

        DBConnection(services, config);
        AddRepositories(services);
    }
    private void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IRepo_Products, Repo_Products>();
        services.AddScoped<IRepo_Users,Repo_Users>();
        services.AddScoped<IRepo_Cart,Repo_Cart>();
        //services.AddScoped(typeof(IProductsRepository<Product>),typeof(RepoProducts));
    }
    private void DBConnection(IServiceCollection services, IConfiguration config)
    {
        string?
            connectionString = config.GetConnectionString("DefaultConnection");
        if (connectionString is null)
        {
            throw new ArgumentNullException("Connection String was not provided for MySQL access.");
        }
        services.AddSingleton<IConnectionInformation_DB>(new ConnectionInformation_DB(connectionString));
        services.AddScoped<ICRUDQuery, DapperMySQL_DataAccess>();
        services.AddScoped<IFreeQuery, DapperMySQL_DataAccess>();


        services.AddScoped<Repo_StoredProcedures>();
        services.AddScoped<Repo_Tables>();

        //services.AddScoped<Repo_DbTable<T>>();
        services.AddScoped(typeof(Repo_DbTable<>));
    }

    private void AddOpenApi(IServiceCollection services)
    {
        //builder.Services.AddScoped<WeatherForecast>();

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        //builder.Services.AddOpenApi();
    }
}
