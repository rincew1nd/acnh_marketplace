// <copyright file="MarketplaceContext.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.DataBase
{
    using ACNH_Marketplace.DataBase.Models;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Database context.
    /// </summary>
    public class MarketplaceContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MarketplaceContext"/> class.
        /// </summary>
        /// <param name="options"><see cref="DbContextOptions">Database creation options</see>.</param>
        public MarketplaceContext(DbContextOptions<MarketplaceContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets users collection.
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Gets or sets user contacts collection.
        /// </summary>
        public DbSet<UserContact> UserContacts { get; set; }

        /// <summary>
        /// Gets or sets user reviews collection.
        /// </summary>
        public DbSet<UserReview> UserReviews { get; set; }

        /// <summary>
        /// Gets or sets turnip market hosters collection.
        /// </summary>
        public DbSet<TurnipMarketHoster> TurnipMarketHosters { get; set; }

        /// <summary>
        /// Gets or sets turnip market visitors collection.
        /// </summary>
        public DbSet<TurnipMarketVisitor> TurnipMarketVisitors { get; set; }

        /// <summary>
        /// Gets or sets island entry fee collection.
        /// </summary>
        public DbSet<TurnipMarketEntryFee> TurnipMarketEntryFees { get; set; }

        /// <summary>
        /// Gets or sets user report collection.
        /// </summary>
        public DbSet<UserReport> UserReports { get; set; }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserContact>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserContacts)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserReview>()
                .HasOne(ur => ur.Reviewed)
                .WithMany(u => u.UserReviews)
                .HasForeignKey(ur => ur.ReviewedId);

            modelBuilder.Entity<UserReport>()
                .HasKey(ur => new { ur.OperationType, ur.OperationId });
        }
    }
}
