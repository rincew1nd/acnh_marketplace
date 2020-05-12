using ACNH_Marketplace.DataBase;
using ACNH_Marketplace.Telegram;
using ACNH_Marketplace.Telegram.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

            services.AddControllers().AddNewtonsoftJson();

            var botConfiguration = new BotConfiguration();
            Configuration.GetSection("TelegramBot").Bind(botConfiguration);
            services.AddSingleton(botConfiguration);

            services.AddSingleton<UserContextService>();
            services.AddSingleton<CommandRouterService>();
            services.AddSingleton<IBotService, BotService>();
            services.AddScoped<IBotUpdateService, BotUpdateService>();

            services.AddHostedService<ActivatorService>(); //Activate IBotService
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseMiddleware<RequestLoggingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
