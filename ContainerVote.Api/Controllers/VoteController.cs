using System;
using System.Threading.Tasks;
using ContainerVote.Shared.Primitives;
using ContainerVote.Shared.Primitives.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace ContainerVote.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoteController : ControllerBase
    {
        public VoteController(IConnectionMultiplexer redis, ILogger<VoteController> logger)
        {
            Redis = redis;
            Logger = logger;
        }

        private IConnectionMultiplexer Redis { get; }
        private ILogger<VoteController> Logger { get; }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Vote vote)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var registeredVote = new RegisteredVote()
            {
                Timestamp = DateTime.UtcNow,
                IpAddress = "", //TODO
                Voter = vote.Voter,
                Amount = vote.Amount,
                Nominee = vote.Nominee
            };

            // Store the vote in the Redis cache, and wait for it to be confirmed.
            IDatabase db = Redis.GetDatabase();
            await db.ListRightPushAsync(RedisKeys.Votes, JsonConvert.SerializeObject(registeredVote));

            // Notify that a vote has been placed, for the store to pick up.
            ISubscriber sub = Redis.GetSubscriber();
            sub.Publish(RedisChannels.Vote, string.Empty, CommandFlags.FireAndForget);
            
            Logger.LogInformation($"Vote: {vote.Amount} for {vote.Nominee} by '{vote.Voter}'.");

            return Accepted();
        }
    }
}