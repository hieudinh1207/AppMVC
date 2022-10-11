using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MVC_01.Models;
using MVC_01.Services;

namespace MVC_01
{
    public class Startup
    {
        public static string ContentRootPath { set; get; }
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            ContentRootPath = env.ContentRootPath;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddRazorPages();
            // services.Configure<RazorViewEngineOptions>(options =>
            // {
            //     //View/Controller/Action.cshtml

            //     options.ViewLocationFormats.Add("/MyView/{1}/{0}" + RazorViewEngine.ViewExtension);

            // });
            services.AddSingleton<ProductService>();
            services.AddSingleton<PlanetService>();
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseStatusCodePages();// code 400-> 599

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/sayhi", async (context) =>
                {
                    await context.Response.WriteAsync($"Hello ASP.NET MVC {DateTime.Now}");
                });

                endpoints.MapControllerRoute(
                    name: "first",
                    pattern: "{url}/{id?}",
                    defaults: new
                    {
                        controller = "First",
                        action = "ViewProduct"
                    },
                    constraints: new
                    {
                        url = new StringRouteConstraint("xemsanpham"),
                        id = new RangeRouteConstraint(2, 4)
                    }
                );
                endpoints.MapAreaControllerRoute(
                    name: "ProductManage",
                    pattern: "/{controller}/{action=Index}/{id?}",
                    areaName: "ProductManage"
                );
                // endpoints.MapControllerRoute(
                //    name: "firstroute",
                //    pattern: "start-here/{controller = Home}/{action=Index}/{id?}"

                // );
                // endpoints.MapAreaControllerRoute(
                //     name: "ProductManage",
                //     pattern: "ProductManage/{controller}",
                //     areaName: "ProductManage"
                // );
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "/{controller=Home}/{action=Index}/{id?}"
                );

                endpoints.MapRazorPages();
            });
        }
    }
}

/* 
Areas
- là tên dùng để routing 


 */