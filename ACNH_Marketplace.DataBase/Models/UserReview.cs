using ACNH_Marketplace.DataBase.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACNH_Marketplace.DataBase.Models
{
    public class UserReview
    {
        /// <summary>
        /// Review entry identifier
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Type of review
        /// </summary>
        public ReviewType Type { get; set; }

        /// <summary>
        /// Review message about user
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Review rating
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// Reviewer user id
        /// </summary>
        public int ReviewerId { get; set; }

        /// <summary>
        /// Reviewer user object
        /// </summary>
        public User Reviewer { get; set; }

        /// <summary>
        /// User id who got reviewed
        /// </summary>
        public int ReviewedId { get; set; }

        /// <summary>
        /// User object who got reviewed
        /// </summary>
        public User Reviewed { get; set; }
    }
}
