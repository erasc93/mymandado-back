using core;
using core_mandado.Cart;
using core_mandado.Products;
using core_mandado.Users;
using core_mandado.Users.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Services.Dapper;
using Services.Factories;
using Services.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace api_mandado.services;

public sealed class DI_Services
{
    private static DI_Services? _instance;
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
        services.AddOpenApi();

        DBConnection(services, config);
        AddRepositories(services);
        AddAuthentication(services, config);
        AddFactories(services);
    }

    private void AddAuthentication(IServiceCollection services, IConfiguration configurationManager)
    {

        string? secretKey = configurationManager["JwtSettings:SecretKey"];
        if (secretKey is null)
        {
            throw new ArgumentNullException("SecretKey was not provided");
        }

        var alreadyRegistered = services.Any(s =>
            s.ServiceType == typeof(IConfigureOptions<AuthenticationOptions>));
        if (!alreadyRegistered)
        {

            AuthenticationBuilder
                authBuilder = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
            authBuilder.AddJwtBearer((options) =>
                                                                    {
                                                                        byte[]
                                                                            bytes = Encoding.UTF8.GetBytes(secretKey);

                                                                        options.TokenValidationParameters = new TokenValidationParameters
                                                                        {
                                                                            ValidateIssuer = false,
                                                                            ValidateAudience = false,
                                                                            IssuerSigningKey = new SymmetricSecurityKey(bytes)
                                                                        };
                                                                    });

        }
        services.AddSingleton<IJwtTokenGenerator>(new JwtTokenGenerator(secretKey, new JwtSecurityTokenHandler()));
        services.AddAuthorization();
        services.AddHttpContextAccessor();
        services.AddScoped<UserContextService>();
    }

    private void AddFactories(IServiceCollection services)
    {
        services.AddSingleton<FactoryProducts>();
    }
    private void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IRepo_Products, Repo_Products>();
        services.AddScoped<IRepo_Users, Repo_Users>();
        services.AddScoped<IRepo_Cart, Repo_Cart>();
        services.AddScoped<IRepo_Users, Repo_Users>();
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
        services.AddScoped<Repo_TableInfos>();

        //services.AddScoped<Repo_DbTable<T>>();
        services.AddScoped(typeof(Repo_AnyTable<>));
    }

    private void AddOpenApi(IServiceCollection services)
    {
        //builder.Services.AddScoped<WeatherForecast>();

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        //builder.Services.AddOpenApi();
    }
}
