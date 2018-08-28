using System;

namespace ContainerVote.Shared.Primitives.DTOs
{
    public class RegisteredVote : Vote
    {
        /// <summary>
        /// When the vote happened.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The IP address of the voter.
        /// </summary>
        public string IpAddress { get; set; }
    }
}