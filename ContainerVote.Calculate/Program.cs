using System;
using System.Threading.Tasks;
using ContainerVote.Shared.Primitives;

namespace ContainerVote.Calculate
{
    class Program : WorkerProgram
    {
        const string RedisChannel = RedisChannels.Calculate;

        static void Main(string[] args)
        {
            Run((redisDb, redisSubscriber, sqlConnection) =>
            {
                var calculator = new Calculator(redisDb, sqlConnection);
                Task scheduled = Task.CompletedTask;
                
                async Task ScheduleCalculation()
                {
                    await Task.Delay(1000);
                    await calculator.Calculate("scheduled");
                }

                redisSubscriber.Multiplexer.PreserveAsyncOrder = false;
                redisSubscriber.Subscribe(RedisChannel, async (channel, value) =>
                {
                    DateTime now = DateTime.UtcNow;

                    // Skip if a calculation has been scheduled, to reduce load.
                    if (!scheduled.IsCompleted)
                    {
                        Console.WriteLine($"Skipping.      ({now:HH:mm:ss.fff})");    
                        return;
                    }

                    // Calculate immediately if last calculation was more than a second ago
                    if ((now - calculator.LastCalculation).TotalSeconds > 1)
                    {
                        await calculator.Calculate();
                    }
                    else // Schedule it in a second from now. 
                    {
                        Console.WriteLine($"Scheduled.     ({now:HH:mm:ss.fff})");
                        scheduled = ScheduleCalculation();
                    }
                });
                Console.WriteLine($"Subscribed to Redis channel '{RedisChannel}'.");
                calculator.Calculate("startup");
            });
        }
    }
}