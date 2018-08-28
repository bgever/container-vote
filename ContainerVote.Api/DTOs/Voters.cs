using System.Collections.Generic;

namespace ContainerVote.Api.DTOs
{
    /// <summary>
    /// Listing of all voters for a nominee.
    /// </summary>
    public class Voters
    {
        /// <summary>
        /// Votes counted towards this nominee.
        /// </summary>
        public string Nominee { get; set; }

        /// <summary>
        /// List of votes from all voters.
        /// </summary>
        public IEnumerable<UnnominatedVote> Votes { get; set; }
    }
}