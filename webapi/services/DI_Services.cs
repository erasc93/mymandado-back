using api_mandado.services.Security;
using core;
using core_mandado.Cart;
using core_mandado.Products;
using core_mandado.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Services.Dapper.DBWire;
using Services.Dapper.Interfaces;
using Services.Dapper.Queries;
using Services.Repositories;
using System.Configuration;
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
            _instance ??= new DI_Services();
            return _instance;

        }
    }
    private DI_Services() { }

    public void AddDependencies(IServiceCollection services, IConfiguration config)
    {
        DBConnection(services, config);
        AddRepositories(services);
        AddTokenBasedAuthentication(services, config);
    }

    private void AddTokenBasedAuthentication(IServiceCollection services, IConfiguration configurationManager)
    {

        string? secretKey;
        secretKey = configurationManager["JwtSettings:SecretKey"];

        if (secretKey is null)
        {
            throw new ConfigurationErrorsException("JwtSettings:SecretKey was not provided");
        }

        bool alreadyRegistered;

        alreadyRegistered = services.Any(s => s.ServiceType == typeof(IConfigureOptions<AuthenticationOptions>));
        if (!alreadyRegistered)
        {
            AuthenticationBuilder authBuilder;
            authBuilder = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

            //authBuilder.AddJwtBearer(BearerBuilder);
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
        services.AddScoped<ClaimsAccessor>();
    }

    private void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IRepo_Products, Repo_Products>();
        services.AddScoped<IRepo_Users, Repo_Users>();
        services.AddScoped<IRepo_Cart, Repo_Cart>();
        services.AddScoped<IRepo_CartItems, Repo_CartItems>();
        services.AddScoped<IRepo_Users, Repo_Users>();
        //services.AddScoped(typeof(IProductsRepository<Product>),typeof(RepoProducts));
    }
    private void DBConnection(IServiceCollection services, IConfiguration config)
    {

        string
            connectionString = config.GetConnectionString("DefaultConnection")
            ?? throw new ConfigurationErrorsException("Connection String was not provided for MySQL access.");
        services.AddSingleton<IConnectionInformation_DB>(new ConnectionInformation_DB(connectionString));

        services.AddScoped<ITransactionHandle, TransactionHandle>();
        services.AddScoped<IQueries, Queries>();

        services.AddScoped<ICRUD, CRUD>();
        services.AddScoped<IBulk, BulkCRUD>();
        services.AddScoped<IFreeQuery, FreeQuery>();

        services.AddScoped<ICRUDAsync, CRUDAsync>();
        services.AddScoped<IBulkAsync, BulkAsync>();
        services.AddScoped<IFreeQueryAsync, FreeQueryAsync>();

        services.AddScoped<Repo_StoredProcedures>();
        services.AddScoped<Repo_TableInfos>();

        services.AddScoped(typeof(Repo_AnyTable<>));
    }
}
