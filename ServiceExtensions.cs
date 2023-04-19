using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using UserManagementApi.Interfaces;
using UserManagementApi.Model;
using UserManagementApi.Services.Models;
using UserManagementApi.Services.Services;

namespace UserManagementApi.Services
{
    public static class ServiceExtensions
    {

        public static void ConfigureCors(this IServiceCollection services) =>
          services.AddCors(options =>
          {
              options.AddPolicy("CorsPolicy", builder =>
              builder.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
          });


        public static void ConfigureIISIntegration(this IServiceCollection services) =>
          services.Configure<IISOptions>(options =>
          {

          });





        /*  public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
              services.AddDbContext<ApplicationDbContext>(opts =>
              opts.UseSqlServer(configuration.GetConnectionString("DefaultConn")));*/

        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetSection("ConnectionString")["DefaultConn"];
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
        }


        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {

            // For Identity
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //Add Config for Required Email
            services.Configure<IdentityOptions>(
                opts => opts.SignIn.RequireConfirmedEmail = true
                );

            //Add Email Configs
            var emailConfig = configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);



            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<IUserService, UserServices>();

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(x =>
            {
                var actionContext = x.GetRequiredService<IActionContextAccessor>().ActionContext;
                var factory = x.GetRequiredService<IUrlHelperFactory>();
                return factory.GetUrlHelper(actionContext);
            });




        }

    }
}
