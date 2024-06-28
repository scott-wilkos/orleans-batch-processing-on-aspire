using BatchProcessing.Abstractions.Grains;
using BatchProcessing.Grains.Services;
using Microsoft.Extensions.Logging;

namespace BatchProcessing.Grains
{
    internal class EngineWorkerGrain(IDataService dataService, ILogger<EngineWorkerGrain> logger) : Grain, IEngineWorkerGrain
    {
        public async Task DoWork()
        {
            // Would do some work here - Simulate varying delays
            await dataService.DoDataWork();

            var results = await dataService.GetData();
        }
    }
}
