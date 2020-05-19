// <copyright file="MarketplaceContextFactory.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Web
{
    using System;
    using ACNH_Marketplace.DataBase;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Database context for migrations.
    /// </summary>
    public class MarketplaceContextFactory : IDesignTimeDbContextFactory<MarketplaceContext>
    {
        /// <inheritdoc/>
        public MarketplaceContext CreateDbContext(string[] args)
        {
            string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new string[] { @"bin\" }, StringSplitOptions.None)[0];
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(projectPath)
                .AddJsonFile("appsettings.json")
#if DEBUG
                .AddJsonFile("appsettings.Development.json")
#elif RELEASE
                .AddJsonFile("appsettings.Production.json")
#endif
                .Build();
            string connectionString = configuration.GetConnectionString("EFDesignerDatabase");

            var builder = new DbContextOptionsBuilder<MarketplaceContext>();
            builder.UseNpgsql(connectionString);

            return new MarketplaceContext(builder.Options);
        }
    }
}
