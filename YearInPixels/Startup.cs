using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Variel.Web.Authentication;
using Variel.Web.Common;
using YearInPixels.Models.Data;
using YearInPixels.Services;

namespace YearInPixels
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
            services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddHttpContextAccessor();
            services.AddVarielAppSettings<DatabaseContext, Account>();
            services.AddVarielAccount<DatabaseContext, Account>();
            services.AddVarielAuthentication<DatabaseContext, Account>();
            services.AddVarielSession<Account>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, DatabaseContext database)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            database.Database.Migrate();

            app.UseSession(new SessionOptions{IdleTimeout = TimeSpan.FromDays(365)});
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.Use(async (ctx, next) =>
            {
                var did = ctx.Request.Cookies["deviceId"];
                if (!String.IsNullOrWhiteSpace(did))
                {
                    ctx.Response.Cookies.Append("deviceId", did,
                        new CookieOptions {Expires = DateTimeOffset.Now.AddYears(10)});
                }

                await next();
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
