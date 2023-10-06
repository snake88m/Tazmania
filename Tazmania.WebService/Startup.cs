using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Tazmania.EntityFramework.Contexts;
using Tazmania.EntityFramework.Repositories;
using Tazmania.Interfaces.Automation;
using Tazmania.Interfaces.Repositories;
using Tazmania.Interfaces.Services;
using Tazmania.Services;
using Tazmania.WebService.Handlers;

namespace Tazmania.WebService
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IIORepository, IORepository>();
            services.AddTransient<IRequestRepository, RequestRepository>();
            services.AddTransient<IHeatingRepository, HeatingRepository>();
            services.AddTransient<ISchedulerRepository, SchedulerRepository>();
            services.AddTransient<ISecurityRepository, SecurityRepository>();
            services.AddTransient<IIrrigationRepository, IrrigationRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IDatabankService, DatabankService>();

            services.AddSingleton<DbContextOptionsBuilder<TazmaniaDbContext>>(
                new DbContextOptionsBuilder<TazmaniaDbContext>().UseSqlServer(Configuration.GetConnectionString("Tazmania"),
                sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 10,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
                })
            );

            services.AddControllers();
            services.AddMvc(option => option.EnableEndpointRouting = false);

            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // intercetta e logga le eccezioni non gestite
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    var logger = context.RequestServices.GetService<ILogger<Startup>>();
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        // risponde con errore 500 e una scritta generica
                        logger.LogError(contextFeature.Error, "Something went wrong");
                        await context.Response.WriteAsync("See application log for further details");
                    }
                });
            });

            // abilito il redirect automatico alla stessa pagina in https
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            //app.UseAuthorization();

            // abilito il CORS per comunicare con qualsiasi applicazione esterna
            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "empty",
                    template: "/",
                    defaults: new { controller = "Diagnostics", action = "index" });

                routes.MapRoute(
                    name: "default",
                    template: "api/{controller}/{action}/{id?}");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
