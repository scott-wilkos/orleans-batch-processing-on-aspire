using BatchProcessing.Abstractions.Grains;
using BatchProcessing.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orleans.Concurrency;

namespace BatchProcessing.Grains;

[StatelessWorker]
public class BatchProcessManagerGrain(ContextFactory contextFactory, ILogger<BatchProcessManagerGrain> logger)
    : IBatchProcessManagerGrain
{

    public async Task<IEnumerable<BatchProcessRecord>> GetBatchProcesses()
    {
        await using var context = contextFactory.Create();

        var batchProcessRecords = await context.BatchProcesses
            .Select(x => new BatchProcessRecord(x.Id, x.CreatedOn, x.CompletedOn, x.Status)).ToListAsync();

        return batchProcessRecords;
    }

    [ReadOnly]
    public async Task<BatchProcessRecord?> GetBatchProcess(Guid engineId)
    {
        await using var context = contextFactory.Create();

        var batchProcess = await context.BatchProcesses
            .FirstOrDefaultAsync(x => x.Id == engineId);

        return batchProcess is null ? null : new BatchProcessRecord(batchProcess.Id, batchProcess.CreatedOn, batchProcess.CompletedOn, batchProcess.Status);
    }
}