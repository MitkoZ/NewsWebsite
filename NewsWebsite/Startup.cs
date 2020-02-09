using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repositories;
using Repositories.Interfaces;
using Services.CRUD;
using Services.CRUD.Interfaces;
using Services.Security;
using Services.Security.Interfaces;

namespace NewsWebsite
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

            string server = Configuration["DatabaseServer"];
            string port = Configuration["DatabasePort"];
            string user = Configuration["DatabaseUser"];
            string password = Configuration["DatabasePassword"];
            string databaseName = Configuration["DatabaseName"];
            //TODO: extract in a helper class

            services.AddDbContext<NewsDbContext>(options =>
            {
                options.UseSqlServer($"Server={server},{port};Database={databaseName};User Id={user};Password={password}",
                    assembly => assembly.MigrationsAssembly(typeof(NewsDbContext).Assembly.FullName));
            });

            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            services.AddControllersWithViews();
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

            //TODO: write seed method

            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                NewsDbContext newsDbContext = serviceScope.ServiceProvider.GetService<NewsDbContext>();

                Thread.Sleep(10000); // The setting of the database password takes some time to change. We need to wait until it changes.
                newsDbContext.Database.Migrate();
                //newsDbContext.Blogs.Add(new Blog
                //{
                //    Url = "testurl"
                //});
                //TODO: add admin user
                newsDbContext.SaveChanges();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
