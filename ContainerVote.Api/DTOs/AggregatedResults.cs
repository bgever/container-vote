namespace ContainerVote.Api.DTOs
{
    /// <summary>
    /// Results of all votes.
    /// </summary>
    public class AggregatedResults
    {
        /// <summary>
        /// Whether the results could be calculated.
        /// </summary>
        /// <remarks>
        /// Could be false when the calculation service hasn't performed.
        /// </remarks>
        public bool Calculated { get; set; }
        
        /// <summary>
        /// Number of votes for .NET Framework.
        /// </summary>
        public int DotNet { get; set; }

        /// <summary>
        /// Number of votes for .NET Core.
        /// </summary>
        public int DotNetCore { get; set; }

        /// <summary>
        /// Number of votes for JavaScript.
        /// </summary>
        public int JavaScript { get; set; }
    }
}