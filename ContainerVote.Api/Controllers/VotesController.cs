using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContainerVote.Api.DTOs;
using ContainerVote.Shared.Primitives;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;

namespace ContainerVote.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VotesController : ControllerBase
    {
        public VotesController(IConnectionMultiplexer redis)
        {
            Redis = redis;
        }

        private IConnectionMultiplexer Redis { get; }
        
        /// <summary>
        /// Gets the vote results.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<AggregatedResults>> Get()
        {
            IDatabase db = Redis.GetDatabase();
            HashEntry[] hashes = await db.HashGetAllAsync(RedisKeys.Results);

            return new AggregatedResults
            {
                Calculated = hashes.Length != 0,
                DotNet = (int)hashes.FirstOrDefault(entry => entry.Name == Nominees.DotNet).Value,
                DotNetCore = (int)hashes.FirstOrDefault(entry => entry.Name == Nominees.DotNetCore).Value,
                JavaScript = (int)hashes.FirstOrDefault(entry => entry.Name == Nominees.JavaScript).Value
            };
        }

        [HttpGet("{nominee}")]
        public ActionResult<string> Get(int nominee)
        {
            return "value"; //TODO
        }
    }
}
