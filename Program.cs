using Microsoft.AspNetCore.Localization;
using System.Globalization;
using System.Reflection;
using bilanguage.resources;
using bilanguage;
using Microsoft.Extensions.Options;

namespace bilanguage
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddSingleton<LocService>();
            builder.Services.AddLocalization(opt => { opt.ResourcesPath = "resources"; });

            // Add services to the container.
            builder.Services.AddControllersWithViews().AddViewLocalization()
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        var assemblyName = new AssemblyName(typeof(SharedResource).GetTypeInfo().Assembly.FullName);
                        return factory.Create("a", assemblyName.Name);
                    };
                });

            builder.Services.AddRazorPages();
            builder.Services.Configure<RequestLocalizationOptions>(
                options =>
                {
                    var supportCultures = new List<CultureInfo>
                    {
                        new CultureInfo("en-US"),
                        new CultureInfo("ar-SA")
                    };

                    options.DefaultRequestCulture = new RequestCulture(culture: "en-US", uiCulture: "en-US");
                    options.SupportedCultures = supportCultures;
                    options.SupportedUICultures = supportCultures;

                    options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
                });

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(5);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            var LocOptions = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(LocOptions.Value);
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCookiePolicy();

            app.UseAuthorization();
            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}