using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ACNH_Marketplace.DataBase.Models
{
    public class TurnipMarketVisitor
    {
        /// <summary>
        /// Visitor entry identifier
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Description of visitor entry
        /// </summary>
        public string Message { get; set; }

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
