using ContainerVote.Shared.Primitives.DTOs;
using Newtonsoft.Json;

namespace ContainerVote.Api.DTOs
{
    public class UnnominatedVote : RegisteredVote
    {
        /// <summary>
        /// Votes counted towards this nominee.
        /// </summary>
        /// <remarks>
        /// Overrides <see cref="Vote"/> base class to return always null.
        /// </remarks>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public override string Nominee => null;
    }
}