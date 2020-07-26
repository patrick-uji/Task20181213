using System;
using Task20181213.Common.DB;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SpaServices.AngularCli;
namespace Task20181213_P2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private bool InDocker
        {
            get
            {
                return Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddDbContext<CurrencyMarketContext>(options => {
                if (this.InDocker)
                {
                    string user = Configuration["DBUser"] ?? "SA"; //Probably should default to a different account...
                    string port = Configuration["DBPort"] ?? "1433";
                    string server = Configuration["DBServer"] ?? "ms-sql-server";
                    string password = Configuration["DBPassword"] ?? "ST0RE_ELSEWHERE!";
                    options.UseSqlServer("Server=" + server + "," + port + ";Initial Catalog=CurrencyMarket;User ID=" + user + ";Password=" + password);
                }
            });
            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                CurrencyMarketContext currencyMarket = serviceScope.ServiceProvider.GetService<CurrencyMarketContext>();
                currencyMarket.Database.Migrate();
            }
        }
    }
}
