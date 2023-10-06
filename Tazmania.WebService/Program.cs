using Microsoft.AspNetCore.Authentication;
using Serilog.Enrichers;
using Serilog.Events;
using Serilog;
using Tazmania.WebService;
using System.Reflection;

public class Program
{
    public static void Main(string[] args)
    {
        // template singola linea di log
        string loggerTemplate = @"{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u4}]<{ThreadId}> [{SourceContext:l}] {Message:lj}{NewLine}{Exception}";

        // creo la path del file di log (il nome viene autogenerato con il datetime seguito da .log)
        string logPath = Path.Combine(AppContext.BaseDirectory, "logs", Assembly.GetExecutingAssembly().GetName().Name + ".log");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.With(new ThreadIdEnricher())
            .Enrich.FromLogContext()
            .WriteTo.File(logPath, LogEventLevel.Information, loggerTemplate, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 90)
            .CreateLogger();

        try
        {
            CreateHostBuilder(args).Build().Run();
            Log.Information("Application started");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application start-up failed");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }



    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}