using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContainerVote.Shared.Primitives;
using Dapper;
using StackExchange.Redis;
using CommandFlags = StackExchange.Redis.CommandFlags;

namespace ContainerVote.Calculate
{
    public class Calculator
    {
        public Calculator(IDatabase redisDb, IDbConnection sqlConnection)
        {
            RedisDb = redisDb;
            SqlConnection = sqlConnection;
        }

        private IDatabase RedisDb { get; }
        private IDbConnection SqlConnection { get; }
        public DateTime LastCalculation { get; private set; } = DateTime.MinValue;

        public async Task Calculate(string remark = null)
        {
            LastCalculation = DateTime.UtcNow;
            Console.WriteLine($"CALCULATING... ({LastCalculation:HH:mm:ss.fff})" +
                              $" #{Thread.CurrentThread.ManagedThreadId}" +
                              $"{(remark != null ? $" [{remark}]" : string.Empty)}");
            var sw = new Stopwatch();
            sw.Start();

            IEnumerable<dynamic> results = await SqlConnection.QueryAsync(
                "SELECT Nominee, sum(Amount) as Amount FROM Votes GROUP BY Nominee");

            HashEntry FetchHash(string nominee)
            {
                return new HashEntry(
                    nominee,
                    ((int?) results.FirstOrDefault(row => row.Nominee == nominee)?.Amount ?? 0).ToString());
            }

            RedisDb.HashSet(
                RedisKeys.Results,
                new[]
                {
                    FetchHash(Nominees.DotNet),
                    FetchHash(Nominees.DotNetCore),
                    FetchHash(Nominees.JavaScript)
                },
                CommandFlags.FireAndForget);
            
            // TODO: Fetch and store the results per Nominee in Redis.

            sw.Stop();
            Console.WriteLine($"Done.          (in {sw.Elapsed})");
        }
    }
}