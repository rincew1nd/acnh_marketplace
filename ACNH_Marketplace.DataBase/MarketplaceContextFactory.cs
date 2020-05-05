using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ACNH_Marketplace.DataBase
{
    public class MarketplaceContextFactory : IDesignTimeDbContextFactory<MarketplaceContext>
    {
        public MarketplaceContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MarketplaceContext>();
            optionsBuilder.UseMySQL("Server=95.179.153.250;Port=3306;Database=acnh_marketplace;Uid=acnh_marketplace;Pwd=06vlrjp2lhRsAAfb;");
            return new MarketplaceContext(optionsBuilder.Options);
        }
    }
}
