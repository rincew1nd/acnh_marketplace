using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACNH_Marketplace.DataBase;
using ACNH_Marketplace.Telegram;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ACNH_Marketplace.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MarketplaceContext>(options =>
                options.UseMySQL(Configuration.GetConnectionString("MarketplaceDatabase")));

            services.AddRazorPages();

            var botConfiguration = new BotConfiguration();
            Configuration.GetSection("TelegramBot").Bind(botConfiguration);
            services.AddSingleton(botConfiguration);

            services.AddSingleton<MarketplaceBot>();
            services.AddHostedService<TelegramBotService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
