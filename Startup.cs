using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using MVC_01.Data;
using MVC_01.Menu;
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
            services.AddOptions();
            var mailsetting = Configuration.GetSection("MailSettings");
            services.Configure<MailSettings>(mailsetting);
            services.AddSingleton<IEmailSender, SendMailService>();


            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddDbContext<AppDbContext>(options =>
            {
                string connectString = Configuration.GetConnectionString("AppDbContext");
                options.UseSqlServer(connectString);
            });
            // services.Configure<RazorViewEngineOptions>(options =>
            // {
            //     //View/Controller/Action.cshtml

            //     options.ViewLocationFormats.Add("/MyView/{1}/{0}" + RazorViewEngine.ViewExtension);

            // });
            services.AddSingleton<ProductService>();
            services.AddSingleton<PlanetService>();
            services.AddIdentity<AppUser, IdentityRole>()
                         .AddEntityFrameworkStores<AppDbContext>()
                         .AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                // Thiết lập về Password
                options.Password.RequireDigit = false; // Không bắt phải có số
                options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
                options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
                options.Password.RequireUppercase = false; // Không bắt buộc chữ in
                options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
                options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

                // Cấu hình Lockout - khóa user
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
                options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa
                options.Lockout.AllowedForNewUsers = true;

                // Cấu hình về User.
                options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;  // Email là duy nhất

                // Cấu hình đăng nhập.
                options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
                options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại

            });
            services.AddOptions();
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/login";
                options.LogoutPath = "/logout";
                options.AccessDeniedPath = "/khongduoctruycap.html";
            });
            services.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ViewManageMenu", builder =>
                {
                    builder.RequireAuthenticatedUser();
                    builder.RequireRole(RoleName.Administrator);
                });
            });
               services.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();

                services.AddAuthorization(options => {
                    options.AddPolicy("ViewManageMenu", builder => {
                        builder.RequireAuthenticatedUser();
                        builder.RequireRole(RoleName.Administrator);
                    });
                });



                services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
                services.AddTransient<AdminSidebarService>();


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
            // /contents/1
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "Uploads")
                ),
                RequestPath = "/contents"
            });
            app.UseStatusCodePages();// code 400-> 599

            app.UseRouting();
            app.UseAuthentication();
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
                endpoints.MapAreaControllerRoute(
                    name: "Contact",
                    pattern: "/{controller}/{action=Index}/{id?}",
                    areaName: "Contact"
                );
                endpoints.MapAreaControllerRoute(
               name: "Files",
               pattern: "/{controller}/{action=Index}/{id?}",
               areaName: "Files"
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

dotnet aspnet-codegenerator controller -name FileManagerController --relativeFolderPath Areas\Files\Controllers
 */
 //test