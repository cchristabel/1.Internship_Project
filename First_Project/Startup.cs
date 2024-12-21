using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using SecureMVCApp.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SecureMVCApp.Services;
using Rsk.Enforcer;
using Rsk.Enforcer.AspNetCore;
using Rsk.Enforcer.PEP;
using SecureMVCApp.Models;
using System.Collections.Generic;
using System.IO;

namespace SecureMVCApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string licensee = "DEMO";
            string licenseKey = "eyJhdXRoIjoiREVNTyIsImV4cCI6IjIwMjQtMTEtMjdUMDE6MDA6MDIuNjA0MDc2NSswMDowMCIsImlhdCI6IjIwMjQtMTAtMjhUMDE6MDA6MDIiLCJvcmciOiJERU1PIiwiYXVkIjo3fQ==.VsUUPM1ejOTtpzeSrKLtB4Lx4vQwWcF3xlZhIw7uaVrRVXxvjh5dMb+MPG/R+agBCGBX4kJX/9LiLSpt5OiekFX9zWwde3NO49PYkiZTWwmC2MU4Pjdcp6QsM9++Z2XAVRfwgpK+WdnOwBSP9bAv69ztcuFZc/zLmylo96Wd6UxyQJeDP3bEihbrLzuGMz2SgFhArBwPO3wYFUu9loxg8kh9k2qpx//OYt80rdJogSi8JytNRg/BQRWZiUw5xrERMjnJQ/Xwh7qNS2+Brt+coY0lsbsZlOv0KDpwyZGCKfzTqNJT2wT7uUJnVxDA97X4jkrwuVQHn/+KZy3PkGJIr3qKT8erzE8SxNe1MEd4vnj74l/fbaIsXE9JjVzqhaj7ROAOTbOErX3ruhON+b8Xu8fYXRMRFNRC/YF++Zx0S65JH+Un462oknzlQ4BKTS1+GkV4YtflpwDUGL4gSyQ3SwZpcM9l9CDN+APD27f59Ezvf+5QAM3SegPJCjvcybBx/RmUhYVgPfKjbCmjQUi4rroqUhMULqAaW11bttUiXwC+nKHosVZEoXYsKwBEM1umqmd2CvfglL4mKte+OEuqDTZeG5gTjCOSqUdmszd1Z/6b8iaPCmkcZ5oUQQpJHQLbQUheV33w49i48YscI0JsQHdFOqDjCewapzcjAwLykEg=";

            services.AddLogging(lb =>
            {
                lb.SetMinimumLevel(LogLevel.Trace);
                lb.AddConsole();
            });
            
            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddSingleton<IManagePurchaseOrders, PurchaseOrderService>();
            services.AddSingleton<IManageProducts, ProductService>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("Identity"));
            services.AddDefaultIdentity<IdentityUser>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredUniqueChars = 0;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 1;

                    options.SignIn.RequireConfirmedAccount = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>();

            
            services.AddControllersWithViews();
            services.AddRazorPages();

            services
            .AddEnforcer("AcmeCorp.Global", options =>  {
                options.Licensee = licensee;
                options.LicenseKey = licenseKey;
            })
            .AddFileSystemPolicyStore("policies")
            .AddPolicyEnforcementPoint(o => o.Bias = PepBias.Deny)
            .AddClaimsAttributeValueProvider(o => { })
            .AddPolicyAttributeProvider<TimeBasedAttributeProvider>() // Register the time-based attribute provider
            .AddDefaultAdviceHandling()
            .AddPolicyAttributeProvider<PurchaseDepartmentAttributeProvider>()
            .AddEnforcerAuthorizationRazorViewDenyHandler<AuthorizationFailureAdvice>("~/Views/Shared/NotAuthorized.cshtml");


            // For user credentials
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("users.json", optional: false, reloadOnChange: true)
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            // Register CsvUserImportService
            services.AddScoped<CsvUserImportService>();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            await AddUsers(app.ApplicationServices);
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

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapRazorPages();
            });
        }
        
        private async Task AddUsers(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var userManager = scope.ServiceProvider.GetService<UserManager<IdentityUser>>();
            var configuration = scope.ServiceProvider.GetService<IConfiguration>();

            // Read the users from the JSON file
            var usersSection = configuration.GetSection("Users");
            var users = usersSection.Get<List<UserConfig>>();

            // Add each user
            foreach (var userConfig in users)
            {
                var result = await userManager.CreateAsync(new IdentityUser(userConfig.Email)
                {
                    Email = userConfig.Email
                }, userConfig.Password);

                if (result.Succeeded)
                {
                    var user = await userManager.FindByNameAsync(userConfig.Email);
                    foreach (var claim in userConfig.Claims)
                    {
                        await userManager.AddClaimAsync(user, new Claim(claim.Type, claim.Value));
                    }
                }
            }
        }

        private async Task AddUser( UserManager<IdentityUser> userManager,string email, string password, params Claim[] claims)
        {
            var result = await userManager.CreateAsync(new IdentityUser(email)
            {
                Email =  email
            }, password);
            
            var user = await userManager.FindByNameAsync(email);

            await userManager.AddClaimAsync(user, new Claim("email", email));
            
            foreach(Claim claim in claims )
            {
                await userManager.AddClaimAsync(user, claim);
            }
        }
    }
}
