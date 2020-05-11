using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ACNH_Marketplace.DataBase.Models
{
    public class User
    {
        /// <summary>
        /// Telegram user id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// In game name
        /// </summary>
        public string InGameName { get; set; }

        /// <summary>
        /// Game island name
        /// </summary>
        public string IslandName { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        public string UserName { get; set; }
        
        /// <summary>
        /// User GTM
        /// </summary>
        public int Timezone { get; set; }

        /// <summary>
        /// When user was active last time
        /// </summary>
        public DateTime LastActiveDate { get; set; }

        /// <summary>
        /// User rating as a island hoster
        /// </summary>
        public float HosterRating { get; set; }

        /// <summary>
        /// User rating as an island visitor
        /// </summary>
        public float VisitorRating { get; set; }

        /// <summary>
        /// User rating as exchange partisipant
        /// </summary>
        public float ExchangeRating { get; set; }

        public IList<UserReview> UserReviews { get; set; }
        public IList<TurnipMarketHoster> Hosts { get; set; }
        public IList<TurnipMarketVisitor> Visits { get; set; }
    }
}
