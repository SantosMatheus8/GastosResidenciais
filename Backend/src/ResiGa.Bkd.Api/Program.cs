using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using ResiGa.Bkd.Domain.Exceptions;
using ResiGa.Bkd.Ioc;

namespace ResiGa.Bkd.Api;

[ExcludeFromCodeCoverage]
public static class Program
{
    private static void Configure(IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseStaticFiles();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
            c.RoutePrefix = string.Empty;
        });

        app.UseMiddleware<ExceptionMiddleware>();
        app.UseRouting();
        app.UseCors("CorsPolicy");
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers().RequireCors("CorsPolicy");
        });
    }

    private class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (ResigaBaseException ex)
            {
                logger.LogWarning(ex, "Erro de negocio: {Message}", ex.Message);
                context.Response.StatusCode = ex.StatusCode;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = ex.Message }));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro inesperado");
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = "Erro interno do servidor" }));
            }
        }
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<SqlConnection>(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("Database");
            return new SqlConnection(connectionString);
        });

        services.AddControllers();
        services.AddRouting(options => options.LowercaseUrls = true);
        services.AddEndpointsApiExplorer();
        services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            c.DescribeAllParametersInCamelCase();
        });

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", policy =>
            {
                policy.WithOrigins("http://localhost:3000")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        services.RegisterDependencies(config);
        services.AddHealthChecks();
    }

    public static void Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();
            Configure(app);

            app.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro inesperado: " + ex.Message + "\n" + ex.StackTrace);
        }
    }
}