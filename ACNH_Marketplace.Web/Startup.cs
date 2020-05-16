// <copyright file="Startup.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Web
{
    using ACNH_Marketplace.DataBase;
    using ACNH_Marketplace.Telegram;
    using ACNH_Marketplace.Telegram.Commands;
    using ACNH_Marketplace.Telegram.Services;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Application startup.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration"><see cref="IConfiguration">Application configuration</see>.</param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets application configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Service configuration.
        /// </summary>
        /// <param name="services"><see cref="IServiceCollection">DI service</see>.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MarketplaceContext>(options =>
                options.UseMySQL(this.Configuration.GetConnectionString("MarketplaceDatabase")));

            services.AddControllers().AddNewtonsoftJson();

            var botConfiguration = new BotConfiguration();
            this.Configuration.GetSection("TelegramBot").Bind(botConfiguration);
            services.AddSingleton(botConfiguration);

            services.AddSingleton<IUserContextService, UserContextService>();
            services.AddSingleton<ICommandRouterService, CommandRouterService>();
            services.AddSingleton<TelegramBot>();
            services.AddScoped<IBotUpdateService, BotUpdateService>();

            services.AddScoped<UserProfileCommand>();

            // Hack for webhook activation of IBotService on startup
            services.AddHostedService<ActivatorService>();
        }

        /// <summary>
        /// Request pipeline.
        /// </summary>
        /// <param name="app"><see cref="IApplicationBuilder"/>.</param>
        /// <param name="env"><see cref="IWebHostEnvironment"/>.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseMiddleware<RequestLoggingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
