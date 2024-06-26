using BatchProcessing.Abstractions.Grains;
using Microsoft.Extensions.Logging;

namespace BatchProcessing.Grains
{
    internal class EngineWorkerGrain(ILogger<EngineWorkerGrain> logger) : Grain, IEngineWorkerGrain
    {
        public async Task DoWork()
        {
            // Would do some work here - Simulate varying delays
            var random = new Random();
            var workTime = random.Next(500, 1000);

            await Task.Delay(workTime);
        }
    }
}
