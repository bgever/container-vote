using System;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Threading;
using StackExchange.Redis;

namespace ContainerVote.Shared.Primitives
{
    /// <summary>
    /// Console application base for connecting to SQL Server and Redis servers. 
    /// </summary>
    public abstract class WorkerProgram
    {
        private const string SqlConnStrEnvVar = "SQL_CONNECTION_STRING";
        
        private static readonly ManualResetEvent Block = new ManualResetEvent(false);

        /// <summary>
        /// Connects to both SQL Server and Redis, and exposes them to the worker. 
        /// The SQL Server connection uses the connection string from the <see cref="SqlConnStrEnvVar"/> environment
        /// variable. If it fails to connect, it will attempt to reconnect up to 60 times with a 1 second delay.
        /// </summary>
        /// <param name="worker">The worker logic.</param>
        protected static void Run(Action<IDatabase, ISubscriber, IDbConnection> worker)
        {
            string sqlConnStr = Environment.GetEnvironmentVariable(SqlConnStrEnvVar)
                                ?? throw new ApplicationException(
                                    $"Environment variable {SqlConnStrEnvVar} undefined.");
            IDbConnection sqlConn = new SqlConnection(sqlConnStr);
            OpenSqlConnection(sqlConn);
            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("redis");
            IDatabase db = redis.GetDatabase();
            ISubscriber sub = redis.GetSubscriber();

            worker(db, sub, sqlConn);

            Block.WaitOne();
        }
        
        private const int SqlReconnectAttempts = 60;

        static void OpenSqlConnection(IDbConnection conn, int attempt = 1)
        {
            if (attempt > SqlReconnectAttempts)
            {
                throw new ApplicationException($"Cannot connect to database after {SqlReconnectAttempts} attempts.");
            }

            try
            {
                conn.Open();
                Console.WriteLine("Connected to the database.");
            }
            catch (SqlException e)
            {
                Console.WriteLine("ERROR connecting to the database: " + e.Message);
                attempt++;
                Console.WriteLine($"Retrying attempt {attempt} in 1s...");
                Thread.Sleep(1000);
                OpenSqlConnection(conn, attempt);
            }
        }
    }
}