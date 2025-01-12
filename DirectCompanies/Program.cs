using DirectCompanies.Components;
using DirectCompanies.Helper;
using DirectCompanies.Localization;
using DirectCompanies.Models;
using DirectCompanies.Security;
using DirectCompanies.Services;
using DirectCompanies.Shared;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Xml;

namespace DirectCompanies
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddHostedService<StartingService>();
            builder.Services.AddLocalization();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
            });



            builder.Services.AddIdentity<ApplicationUser, IdentityRole<decimal>>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;

                options.SignIn.RequireConfirmedPhoneNumber = false;

                options.User.RequireUniqueEmail = false;

                options.Lockout.AllowedForNewUsers = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 4; 
                options.Password.RequiredUniqueChars = 0; 
            })
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<ISetupKeyValueService, SetupKeyValueService>();
            builder.Services.AddScoped<IExcelService, ExcelService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IOutBoxEventService, OutBoxEventService>();
            //builder.Services.AddScoped<IPasswordValidator<ApplicationUser>, CustomPasswordValidator>();

            builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options => options.DetailedErrors = true);



            var app = builder.Build();
            ServiceProviderHelper.ServiceProvider = app.Services;
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseAuthentication();
            app.UseAuthorization();
            


       



            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();
            app.MapControllers();

            var localizationOptions = new RequestLocalizationOptions();
            var enUsCulture = new CultureInfo("en-US");
            var arEgCulture = new CultureInfo("ar-EG") { NumberFormat = { NumberDecimalSeparator = "." } };
            var supportedCultures = new[] { enUsCulture, arEgCulture };
            localizationOptions.DefaultRequestCulture = new RequestCulture(arEgCulture, arEgCulture);
            localizationOptions.SupportedCultures = supportedCultures;
            localizationOptions.SupportedUICultures = supportedCultures;
            app.UseRequestLocalization(localizationOptions);



            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            app.Run();
        }
    }
}
