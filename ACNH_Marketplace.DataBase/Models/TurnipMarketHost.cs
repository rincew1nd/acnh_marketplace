using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ACNH_Marketplace.DataBase.Models
{
    public class TurnipMarketHoster
    {
        /// <summary>
        /// Host entry identifier
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// When entry was registered
        /// </summary>
        public DateTime RegistrationDate { get; set; }

        /// <summary>
        /// When entry will expire
        /// </summary>
        public DateTime ExpirationDate { get; set; }

        /// <summary>
        /// Entry description by user
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// How much turnip costs
        /// </summary>
        public int TurnipCost { get; set; }

        /// <summary>
        /// Creator user id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Creator user object
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Fees to enter
        /// </summary>
        public IList<TurnipEntryFee> Fee { get; set; }
    }
}
