using ACNH_Marketplace.DataBase.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ACNH_Marketplace.DataBase
{
    public class MarketplaceContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserReview> UserRevies { get; set; }
        public DbSet<TurnipMarketHoster> TurnipMarketHosters { get; set; }
        public DbSet<TurnipMarketVisitor> TurnipMarketVisitors { get; set; }
        public DbSet<TurnipEntryFee> TurnipEntryFees { get; set; }

        public MarketplaceContext(DbContextOptions<MarketplaceContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserReview>()
                .HasOne(ur => ur.Reviewed)
                .WithMany(u => u.UserReviews)
                .HasForeignKey(ur => ur.ReviewedId);
        }
    }
}
