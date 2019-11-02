using Dios.Controllers;
using Dios.Data;
using Dios.Helpers;
using Dios.Models;
using Dios.Repositories;
using Dios.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dios
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add repos as scoped dependency so they are shared per request.
            services.AddScoped<IAddressesRepository, AddressesRepository>();
            services.AddScoped<IFlatsRepository, FlatsRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IParametersRepository, ParametersRepository>();
            services.AddScoped<IAddressHostsRepository, AddressHostsRepository>();
            services.AddScoped<IErrorReportsRepository, ErrorReportsRepository>();

            services.AddScoped<IRequestUserProvider, RequestUserProvider>();
            services.AddScoped<IRequestSignInProvider, RequestSignInProvider>();
            services.AddScoped<INewUser, NewUser>();
            services.AddScoped<ILog<AccountController>, Log<AccountController>>();

            services.AddScoped<IExport, ExportWrapper>();
            services.AddScoped<IZipFile, ZipFileWrapper>();

            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
