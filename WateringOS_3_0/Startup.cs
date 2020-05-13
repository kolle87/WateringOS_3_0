using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WateringOS_3_0.Controllers;
using WateringOS_3_0.Models;

namespace WateringOS_3_0
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)  // This method gets called by the runtime. Use this method to add services to the container.
        {
            SettingsController.ReadFromFileStatic();
            
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.Cookie.Name = ".WateringSystem.SessionID";
                options.IdleTimeout = TimeSpan.FromMinutes(Settings.System.DLY_Auto_LogOff);
                options.Cookie.IsEssential = true;
            });
            services.AddControllersWithViews();
        }

        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)   // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Shared/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Watering}/{action=Index}/{id?}");
            });
        }
    }
}
