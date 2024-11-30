using AllBirds.Application.Contracts;
using AllBirds.Application.Mapper;
using AllBirds.Application.Services.AccountServices;
using AllBirds.Application.Services.CategoryProductServices;
using AllBirds.Application.Services.CategoryServices;
using AllBirds.Application.Services.ColorServices;
using AllBirds.Application.Services.CouponServices;
using AllBirds.Application.Services.OrderDetailServices;
using AllBirds.Application.Services.OrderMasterServices;
using AllBirds.Application.Services.OrderStateServices;
using AllBirds.Application.Services.ProductColorImageServices;
using AllBirds.Application.Services.ProductColorServices;
using AllBirds.Application.Services.ProductColorSizeServices;
using AllBirds.Application.Services.ProductDetailService;
using AllBirds.Application.Services.ProductServices;
using AllBirds.Application.Services.ProductSpecificationServices;
using AllBirds.Application.Services.SizeServices;
using AllBirds.Application.Services.SpecificationServices;
using AllBirds.Context;
using AllBirds.Infrastructure;
using AllBirds.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace AllBirds.AdminDashboard
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<AllBirdsContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
            // Account
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IAccountRoleRepository, AccountRoleRepository>();
            // Category
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            // CategoryProduct
            builder.Services.AddScoped<ICategoryProductService, CategoryProductService>();
            builder.Services.AddScoped<ICategoryProductRepository, CategoryProductRepository>();
            // ClientFavorite

            // Color
            builder.Services.AddScoped<IColorService, ColorService>();
            builder.Services.AddScoped<IColorRepository, ColorRepository>();
            // Coupon
            builder.Services.AddScoped<ICouponService, CouponService>();
            builder.Services.AddScoped<ICouponRepository, CouponRepository>();
            // OrderDetail
            builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();
            builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
            // OrderMaster
            builder.Services.AddScoped<IOrderMasterService, OrderMasterService>();
            builder.Services.AddScoped<IOrderMasterRepository, OrderMasterRepository>();
            // OrderState
            builder.Services.AddScoped<IOrderStateService, OrderStateService>();
            builder.Services.AddScoped<IOrderStateRepository, OrderStateRepository>();
            // Product
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            // ProductColor
            builder.Services.AddScoped<IProductColorService, ProductColorService>();
            builder.Services.AddScoped<IProductColorRepository, ProductColorRepository>();
            // ProductColorImage
            builder.Services.AddScoped<IProductColorImageRepository, ProductColorImageRepository>();
            builder.Services.AddScoped<IProductColotImageService, ProductColorImageService>();
            // ProductColorSize
            builder.Services.AddScoped<IProductColorSizeService, ProductColorSizeService>();
            builder.Services.AddScoped<IProductColorSizeRepository, ProductColorSizeRepository>();
            // ProductDetail
            builder.Services.AddScoped<IProductDetailsService, ProductDetailsService>();
            builder.Services.AddScoped<IProductDetailsRepository, ProductDetailsRepository>();
            // ProductReview
            //builder.Services.AddScoped<IProductReviewService, ProductReviewService>();
            //builder.Services.AddScoped<IProductReviewRepository, ProductReviewRepository>();
            // ProductSpecification
            builder.Services.AddScoped<IProductSpecificationService, ProductSpecificationService>();
            builder.Services.AddScoped<IProductSpecificationRepository, ProductSpecificationRepository>();
            // Size
            builder.Services.AddScoped<ISizeService, SizeService>();
            builder.Services.AddScoped<ISizeRepository, SizeRepository>();
            // Specification
            builder.Services.AddScoped<ISpecificationService, SpecificationService>();
            builder.Services.AddScoped<ISpecificationRepository, SpecificationRepository>();

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddDefaultIdentity<CustomUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.User.RequireUniqueEmail = true;
            })
                .AddRoles<IdentityRole<int>>()
            //builder.Services.AddIdentity<CustomUser, IdentityRole<int>>(options =>
            //{
            //    options.SignIn.RequireConfirmedAccount = true;
            //    options.Password.RequireDigit = true;
            //    options.Password.RequiredLength = 6;
            //    options.Password.RequireNonAlphanumeric = false;
            //    options.Password.RequireUppercase = false;
            //    options.Password.RequireLowercase = false;
            //    options.User.RequireUniqueEmail = true;
            //})
                .AddEntityFrameworkStores<AllBirdsContext>();
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.Cookie.Name = "MVCAdminCookie";
                //options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromDays(3);
                options.LoginPath = "/Account/Login";
                //options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                //options.SlidingExpiration = true;
            });
            //builder.Services.AddRazorPages();
            builder.Services.AddControllersWithViews();
            
            var app = builder.Build();
            
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
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

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            //app.MapRazorPages();

            app.Run();
        }
    }
}
