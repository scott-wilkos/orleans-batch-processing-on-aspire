using BatchProcessing.Abstractions.Grains;
using BatchProcessing.Domain;
using BatchProcessing.Domain.Models;
using Microsoft.Extensions.Logging;
using Orleans.Concurrency;

namespace BatchProcessing.Grains;

[StatelessWorker]
internal class EngineWorkerGrain(ContextFactory contextFactory, ILogger<EngineWorkerGrain> logger) : Grain, IEngineWorkerGrain
{
    private static readonly Random Random = new();

    public async Task DoWork(Guid id)
    {
        try
        {
            await using var context = contextFactory.Create();

            var item = await context.BatchProcessItems.FindAsync(id);

            if (item is not null)
            {
                item.Status = BatchProcessItemStatusEnum.Running;
                await context.SaveChangesAsync();

                // TODO: This is where we'll do some "analysis work" on the item
                // Simulate some work delay between 125 and 275 milliseconds
                await Task.Delay(Random.Next(125, 275));

                item.Status = BatchProcessItemStatusEnum.Completed;
                await context.SaveChangesAsync();
            }
        }
        finally
        {
            // Deactivate the grain as soon as the work is over to free up resources
            DeactivateOnIdle();
        }
    }
}