using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Enrichers;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System.Reflection;
using Tazmania.Automation;
using Tazmania.BackgroundServices;
using Tazmania.EntityFramework.Contexts;
using Tazmania.EntityFramework.Repositories;
using Tazmania.Interfaces.Automation;
using Tazmania.Interfaces.Repositories;
using Tazmania.Interfaces.Services;
using Tazmania.Services;

namespace Tazmania.Service
{
    public class Program
    {
        // template singola linea di log
        const string LOG_TEMPLATE = @"{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u4}]<{ThreadId}> [{SourceContext:l}] {Message:lj}{NewLine}{Exception}";

        public static void Main(string[] args)
        {
            // creo la path del file di log (il nome viene autogenerato con il datetime seguito da .log)
            string logPath = Path.Combine(AppContext.BaseDirectory, "logs", Assembly.GetExecutingAssembly().GetName().Name + ".log");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.With(new ThreadIdEnricher())
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate: LOG_TEMPLATE, theme: AnsiConsoleTheme.Literate)
                .WriteTo.File(logPath, outputTemplate: LOG_TEMPLATE, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 90)
                .CreateLogger();

            try
            {
                Log.Information("====================================================================");
                Log.Information($"Application Starts. Version: {Assembly.GetEntryAssembly()?.GetName().Version}");
                Log.Information($"Application Directory: {AppDomain.CurrentDomain.BaseDirectory}");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Application terminated unexpectedly");
            }
            finally
            {
                Log.Information("====================================================================\r\n");
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureAppConfiguration((context, config) =>
                {
                    // configure the app here.
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient<IIORepository, IORepository>();
                    services.AddTransient<IRequestRepository, RequestRepository>();
                    services.AddTransient<IHeatingRepository,HeatingRepository>();
                    services.AddTransient<IIrrigationRepository, IrrigationRepository>();
                    services.AddTransient<ISecurityRepository, SecurityRepository>();
                    services.AddTransient<ISchedulerRepository, SchedulerRepository>();
                    services.AddTransient<INotifyRepository, NotifyRepository>();
                    services.AddTransient<IUserRepository, UserRepository>();
                    services.AddSingleton<IDuemmegiService, DuemmegiService>();
                    services.AddSingleton<IMemoryService, MemoryService>();
                    services.AddSingleton<IMailService, MailService>();
                    services.AddSingleton<INotifyService, NotifyService>();

                    services.AddSingleton<DbContextOptionsBuilder<TazmaniaDbContext>>(
                        new DbContextOptionsBuilder<TazmaniaDbContext>().UseSqlServer(hostContext.Configuration.GetConnectionString("Tazmania"),
                        sqlOptions =>
                        {
                            sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 10,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                        })
                    );

                    // se non esiste crea il DB prima dell'avvio dei background services
                    using (var ctx = new TazmaniaDbContext(new DbContextOptionsBuilder()
                                        .UseSqlServer(hostContext.Configuration.GetConnectionString("Tazmania")).Options))
                    {
                        ctx.Database.EnsureCreated();
                    }

                    services.AddHostedService<AutomationService>(sp => 
                    {
                        var scope = sp.GetRequiredService<ILifetimeScope>().BeginLifetimeScope();
                        
                        return new AutomationService(scope.Resolve<ILogger<AutomationService>>(),
                                                     scope.Resolve<IDuemmegiService>(),
                                                     sp.GetRequiredService<IIORepository>(),
                                                     sp.GetRequiredService<IMemoryService>());
                    });

                    services.AddHostedService<DispatcherService>(sp =>
                    {
                        var scope = sp.GetRequiredService<ILifetimeScope>().BeginLifetimeScope();

                        return new DispatcherService(scope.Resolve<ILogger<DispatcherService>>(),
                                                     sp.GetRequiredService<IMemoryService>(),
                                                     sp.GetRequiredService<IRequestRepository>(),
                                                     sp.GetRequiredService<IHeatingRepository>(),
                                                     sp.GetRequiredService<IIrrigationRepository>(),
                                                     sp.GetRequiredService<ISecurityRepository>());
                    });

                    services.AddHostedService<HeatingService>(sp =>
                    {
                        var scope = sp.GetRequiredService<ILifetimeScope>().BeginLifetimeScope();

                        return new HeatingService(scope.Resolve<ILogger<HeatingService>>(),
                                                  sp.GetRequiredService<IHeatingRepository>(),
                                                  sp.GetRequiredService<IMemoryService>());
                    });

                    services.AddHostedService<SchedulerService>(sp =>
                    {
                        var scope = sp.GetRequiredService<ILifetimeScope>().BeginLifetimeScope();

                        return new SchedulerService(scope.Resolve<ILogger<SchedulerService>>(),
                                                    sp.GetRequiredService<ISchedulerRepository>(),
                                                    sp.GetRequiredService<IMemoryService>());
                    });

                    services.AddHostedService<SecurityService>(sp =>
                    {
                        var scope = sp.GetRequiredService<ILifetimeScope>().BeginLifetimeScope();

                        return new SecurityService(scope.Resolve<ILogger<SecurityService>>(),
                                                   sp.GetRequiredService<ISecurityRepository>(),
                                                   sp.GetRequiredService<IMemoryService>(),
                                                   sp.GetRequiredService<IMailService>(),
                                                   sp.GetRequiredService<INotifyService>());
                    });

                    services.AddHostedService<IrrigationService>(sp =>
                    {
                        var scope = sp.GetRequiredService<ILifetimeScope>().BeginLifetimeScope();

                        return new IrrigationService(scope.Resolve<ILogger<IrrigationService>>(),
                                                     sp.GetRequiredService<IMemoryService>(),
                                                     sp.GetRequiredService<IIrrigationRepository>());
                    });

                })
                .UseSerilog();
        }
    }
}
