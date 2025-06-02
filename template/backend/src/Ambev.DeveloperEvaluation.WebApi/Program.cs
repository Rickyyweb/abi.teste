using Ambev.DeveloperEvaluation.Application;
using Ambev.DeveloperEvaluation.IoC;
using Ambev.DeveloperEvaluation.ORM.Persistence;
using Ambev.DeveloperEvaluation.WebApi.Middleware;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Ambev.DeveloperEvaluation.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // 1) Configura Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
                    .Build())
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Starting web application");

                var builder = WebApplication.CreateBuilder(args);

                builder.Host.UseSerilog();

                // 2) Registra todos os módulos (Application, Infra, WebApi)
                builder.RegisterDependencies();

                builder.Services.AddControllers();
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
                builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(ApplicationLayer).Assembly);

                var app = builder.Build();

                // 3) Middleware de exceções de validação
                app.UseMiddleware<ValidationExceptionMiddleware>();

                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    });
                }

                // Comentado porque, no container, não há HTTPS dev‐cert
                // app.UseHttpsRedirection();
                app.UseAuthentication();
                app.UseAuthorization();
                app.MapControllers();

                // 4) Efetivamente recreia a base em DEV, ou aplica migrations em PROD
                using (var scope = app.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<DefaultContext>();

                    if (app.Environment.IsDevelopment())
                    {
                        // em dev local, rejeita e reaplica as migrations
                        db.Database.EnsureDeleted();
                        db.Database.Migrate();
                    }
                    else if (app.Environment.IsEnvironment("Testing"))
                    {
                        // em ambiente de teste, NÃO FAZ NADA
                    }
                    else
                    {
                        // em qualquer outro (produção, staging, etc.), apenas aplica migrations
                        db.Database.Migrate();
                    }
                }

                app.Run();
            }
            catch (Exception ex)
            {
                if (ex is AggregateException agg)
                {
                    Console.WriteLine("********** Falha ao inicializar a aplicação (AggregateException) **********");
                    foreach (var inner in agg.InnerExceptions)
                        Console.WriteLine(inner);
                }
                else
                {
                    Console.WriteLine("********** Falha ao inicializar a aplicação **********");
                    Console.WriteLine(ex);
                }

                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
