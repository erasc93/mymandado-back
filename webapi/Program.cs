using api_mandado.services;

namespace api_mandado;


public class Program
{
    private const string CORS_NAME = "AllowAll";
    public static void Main(string[] args)
    {

        WebApplicationBuilder builder;
        WebApplication app;


        builder = WebApplication.CreateBuilder(args);



        DI_Services.instance.AddDependencies(builder.Services, builder.Configuration);

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        builder.Services.AddCors((options) =>
        {
            options.AddPolicy(CORS_NAME,
                (builder) =>
                {
                    builder.AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowAnyOrigin();
                });
        });


        app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseCors(CORS_NAME);//
        app.UseAuthentication();//
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
