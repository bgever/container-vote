using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using ContainerVote.Shared.Primitives.DTOs;
using Dapper;
using Newtonsoft.Json.Serialization;

namespace ContainerVote.Store
{
    public class SqlWriter
    {
        public SqlWriter(IDbConnection db)
        {
            Db = db;
        }

        private IDbConnection Db { get; }

        /// <summary>
        /// Write the provided votes to the SQL database.
        /// </summary>
        /// <param name="votes">The votes to store.</param>
        public int Write(IEnumerable<RegisteredVote> votes)
        {
            var sw = new Stopwatch();
            sw.Start();
            int count;
            try
            {
                count = Db.Execute(
                    @"insert Votes(Voter, Amount, Nominee, IpAddress, Timestamp)" +
                    @" values (@Voter, @Amount, @Nominee, @IpAddress, @Timestamp)",
                    votes,
                    commandTimeout: 3 // seconds
                );
            }
            catch (SqlException e)
            {
                Console.WriteLine($"EXCEPTION: {e}");
                Console.WriteLine($"FAILED to store {votes.Count()} vote(s).");
                // TODO: Re-queue the votes for retry.
                return 0;
            }

            sw.Stop();
            Console.WriteLine($"Stored {count} vote(s) in {sw.Elapsed}");
            return count;
        }
    }
}