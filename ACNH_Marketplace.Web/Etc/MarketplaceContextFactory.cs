using ACNH_Marketplace.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;

namespace ACNH_Marketplace.Web
{
    public class MarketplaceContextFactory : IDesignTimeDbContextFactory<MarketplaceContext>
    {
        public MarketplaceContext CreateDbContext(string[] args)
        {
            string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new String[] { @"bin\" }, StringSplitOptions.None)[0];
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(projectPath)
                .AddJsonFile("appsettings.json")
#if (DEBUG)
                .AddJsonFile("appsettings.Development.json")
#elif (RELEASE)
                .AddJsonFile("appsettings.Production.json")
#endif
                .Build();
            string connectionString = configuration.GetConnectionString("EFDesignerDatabase");

            var builder = new DbContextOptionsBuilder<MarketplaceContext>();
            builder.UseMySQL(connectionString);

            return new MarketplaceContext(builder.Options);
        }
    }
}
