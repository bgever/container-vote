using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using ContainerVote.Shared.Primitives;
using ContainerVote.Shared.Primitives.DTOs;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace ContainerVote.Store
{
    public class QueueDrain
    {
        private const int BatchSize = 10;

        public QueueDrain(IDatabase db, SqlWriter writer)
        {
            Db = db;
            Writer = writer;
        }

        private IDatabase Db { get; }
        private SqlWriter Writer { get; }

        /// <summary>
        /// Drains the Redis queue in batches, and passes them to the database writer.
        /// </summary>
        /// <returns>The number of votes that were persisted by the writer.</returns>
        public int ProcessQueue()
        {
            var sw = new Stopwatch();
            sw.Start();
            var writeCount = 0;
            var votes = new List<RegisteredVote>(BatchSize);
            RedisValue value;
            while ((value = Db.ListLeftPop(RedisKeys.Votes)).HasValue)
            {
                Console.WriteLine("Vote: " + value);
                var vote = JsonConvert.DeserializeObject<RegisteredVote>(value);
                votes.Add(vote);
                if (votes.Count != BatchSize) continue;
                // Process this batch and clear the buffer.
                writeCount += Writer.Write(votes);
                votes.Clear();
            }

            // Process the votes not handled in the last batch.
            if (votes.Count > 0)
            {
                writeCount += Writer.Write(votes);
            }

            sw.Stop();
            Console.WriteLine($"Processed {writeCount} vote(s) in {sw.Elapsed}");
            return writeCount;
        }
    }
}