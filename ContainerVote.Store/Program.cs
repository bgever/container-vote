using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.Metadata;
using System.Threading;
using ContainerVote.Shared.Primitives;
using StackExchange.Redis;

namespace ContainerVote.Store
{
    class Program : WorkerProgram
    {
        const string RedisChannel = RedisChannels.Vote;

        static void Main(string[] args)
        {
            Run((redisDb, redisSubscriber, sqlConnection) =>
            {
                var drain = new QueueDrain(redisDb, new SqlWriter(sqlConnection));

                redisSubscriber.Subscribe(RedisChannel, (channel, value) =>
                {
                    Console.WriteLine("Channel event");
                    if (drain.ProcessQueue() > 0)
                    {
                        // Signal to the calculate channel that new results can be calculated.
                        redisSubscriber.Publish(RedisChannels.Calculate, string.Empty, CommandFlags.FireAndForget);
                    }
                });
                Console.WriteLine($"Subscribed to Redis channel '{RedisChannel}'.");
            });
        }
    }
}