using ACNH_Marketplace.DataBase.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACNH_Marketplace.DataBase.Models
{
    public class TurnipEntryFee
    {
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Identifier of hoster
        /// </summary>
        [ForeignKey("TurnipMarketHoster")]
        public Guid? TurnipMarketHosterId { get; set; }

        /// <summary>
        /// Identifier of visitor
        /// </summary>
        [ForeignKey("TurnipMarketVisitor")]
        public Guid? TurnipMarketVisitorId { get; set; }

        /// <summary>
        /// Type of fee
        /// </summary>
        public FeeType FeeType { get; set; }

        /// <summary>
        /// Description if needed
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// How much to pay
        /// </summary>
        public int Count { get; set; }
    }
}
