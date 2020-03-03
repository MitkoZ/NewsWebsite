using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataAccess;
using DataAccess.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repositories;
using Repositories.Interfaces;
using Serilog;
using Services.CRUD;
using Services.CRUD.Interfaces;
using Services.SMTP;
using Services.SMTP.DTOs;
using Services.SMTP.Interfaces;

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

            services.AddIdentity<User, IdentityRole>()
                    .AddEntityFrameworkStores<NewsDbContext>()
                    .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredUniqueChars = 4;
                options.SignIn.RequireConfirmedEmail = true;
            });

            services.AddScoped<IUsersRepository, UsersRepository>();
            services.AddScoped<IUsersService, UsersService>();
            services.AddScoped<INewsRepository, NewsRepository>();
            services.AddScoped<INewsService, NewsService>();

            services.AddControllersWithViews();
            services.AddLogging(logging =>
            {
                ILogger logger = new LoggerConfiguration()
                                        .WriteTo.Console()
                                        .WriteTo.File(Configuration["logsVolumePath"] + Configuration["Logging:FileName"])
                                        .MinimumLevel.Is(Serilog.Events.LogEventLevel.Error)
                                        .CreateLogger();

                logging.AddSerilog(logger);
            });

            ConfigureOptions(services);
            services.AddSingleton<ISMTPService, SMTPService>();
        }

        private void ConfigureOptions(IServiceCollection services) // we use this method in order not to make a dependency on the Options framework. (the IOptions<T> wrapper around the options object https://stackoverflow.com/questions/53424593/services-configure-or-services-addsingleton-get (exact answer: https://stackoverflow.com/a/53431790))
        {
            services.AddSingleton(Configuration.GetSection("SMTP").Get<SMTPConfigDTO>());
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

            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                NewsDbContext newsDbContext = serviceScope.ServiceProvider.GetService<NewsDbContext>();

                Thread.Sleep(10000); // The setting of the database password takes some time to change. We need to wait until it changes.
                newsDbContext.Database.Migrate();
                this.Seed(newsDbContext);
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void Seed(NewsDbContext newsDbContext)
        {
            if (newsDbContext.Users.Count() == 0)
            {
                newsDbContext.Users.Add(new User
                {
                    Id = "66ce45c8-fc73-48f4-8091-f9afecb63ede",
                    UserName = "admin",
                    NormalizedUserName = "ADMIN",
                    Email = "admin@newswebsite.com",
                    NormalizedEmail = "ADMIN@NEWSWEBSITE.COM",
                    EmailConfirmed = true,
                    PasswordHash = "AQAAAAEAACcQAAAAEFniTtxsJTggycel405/tvGlAobWL4X601brrlGscsR1m62OejtsZSHlR3Dnuh5uCg==", // In plaintext the password is adminpass
                    SecurityStamp = "HTO5NGBRE4GHVUGNVPKUFEVJ3OBBWWT2",
                    ConcurrencyStamp = "45cd1fb7-2deb-4749-8d47-30fc15aaf3b8",
                    PhoneNumber = null,
                    PhoneNumberConfirmed = true,
                    TwoFactorEnabled = false,
                    LockoutEnd = null,
                    LockoutEnabled = false,
                    AccessFailedCount = 0,
                });
                // When seeding the users, set the emails and phones to confirmed, so we don't have to deal with locked seeded users

                // TODO: add admin role to the admin user
            }

            if (newsDbContext.Roles.Count() == 0)
            {
                newsDbContext.Roles.Add(new IdentityRole
                {
                    Id = "164c0c3e-7f24-438f-9708-b7e69c6d2b4a",
                    Name = "Reporter",
                    NormalizedName = "REPORTER",
                    ConcurrencyStamp = "e1728ad7-2b42-41a7-a579-06c38f20a112"
                });
            }

            //TODO: Extract magic strings
            newsDbContext.SaveChanges();

        }
    }
}
