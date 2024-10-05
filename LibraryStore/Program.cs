using Azure.Identity;
using LibraryStore.Core.Interface.IServices;
using LibraryStore.Core.Service;
using LibraryStore.Interface.IRepositories;
using LibraryStore.Storage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace LibraryStore;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

       
        
        var keyVaultEndpoint = new Uri(builder.Configuration["VaultUri"]);
        builder.Configuration.AddAzureKeyVault(keyVaultEndpoint, new DefaultAzureCredential());

        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration["Development"]));
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration["RedisConnectionString"];
            options.InstanceName = "Redis";
        });
        builder.Services.AddTransient<IRepository, Repository>();
        builder.Services.AddTransient<IBookService, BookService>();
        
        builder.Services.AddControllersWithViews();
        
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Register";
                options.LogoutPath = "/User/Logout";
            });
        
        builder.Services.AddAuthorization();
        
        var app = builder.Build();

        app.UseAuthentication();
        app.UseAuthorization();
        
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        
        
        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}