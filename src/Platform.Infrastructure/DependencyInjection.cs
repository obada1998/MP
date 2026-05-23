using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Platform.Application.Abstractions;
using Platform.Application.Auth;
using Platform.Application.Categories;
using Platform.Application.Orders;
using Platform.Application.Pages;
using Platform.Application.Products;
using Platform.Application.Rendering;
using Platform.Application.Storefront;
using Platform.Application.Stores;
using Platform.Infrastructure.Data;
using Platform.Infrastructure.Identity;
using Platform.Infrastructure.Services;

namespace Platform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=(localdb)\\MSSQLLocalDB;Database=MultiTenantECommerce;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddSingleton<PageLayoutValidator>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IStoreAccessService, StoreAccessService>();
        services.AddScoped<IStoreService, StoreService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IPageService, PageService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IStorefrontService, StorefrontService>();

        return services;
    }
}
