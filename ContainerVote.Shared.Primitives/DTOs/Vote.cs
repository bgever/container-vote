using System.ComponentModel.DataAnnotations;

namespace ContainerVote.Shared.Primitives.DTOs
{
    public class Vote
    {
        /// <summary>
        /// Optional display name of the voter.
        /// </summary>
        [StringLength(100)]
        public string Voter { get; set; }
        
        /// <summary>
        /// The number of votes.
        /// </summary>
        [Required]
        [Range(0, 100)]
        public int Amount { get; set; }

        /// <summary>
        /// Votes counted towards this nominee.
        /// </summary>
        [Required]
        [RegularExpression("(DotNetCore|DotNet|JavaScript)", ErrorMessage = "Invalid Nominee.")]
        public virtual string Nominee { get; set; }
    }
}