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
            .Select(x => new BatchProcessRecord(x.Id, x.CreatedOn, x.CompletedOn, x.Status, null)).ToListAsync();

        return batchProcessRecords;
    }

    [ReadOnly]
    public async Task<BatchProcessRecord?> GetBatchProcess(Guid engineId)
    {
        await using var context = contextFactory.Create();

        var fullList= await context.BatchProcesses
            .Select(bp => new
            {
                bp.Id,
                bp.CreatedOn,
                bp.CompletedOn,
                bp.Status,
                AggregateResult = bp.AggregateResult == null ? null : new {
                    bp.Id,
                    bp.AggregateResult.AnalysisTimestamp,
                    bp.AggregateResult.AverageAge,
                    bp.AggregateResult.AverageDependents,
                    bp.AggregateResult.AverageHouseholdSize,
                    //bp.AggregateResult.MaritalStatusCounts.Select(x =>
                    //    new MaritalStatusRecordAverageRecord(x.MaritalStatus, x.AverageCount)))
                    MaritalStatusCounts = bp.AggregateResult.MaritalStatusCounts.Select(x =>
                        new
                        {
                            x.MaritalStatus,
                            x.AverageCount
                        })
                }
            })
            .ToListAsync();

        var batchProcess = fullList.FirstOrDefault(x => x.Id == engineId);

        if (batchProcess is null)
        {
            return null;
        }



        var aggregateResult = batchProcess.AggregateResult == null ? null : new BatchProcessAggregateResultRecord(
            batchProcess.Id,
            batchProcess.AggregateResult.AnalysisTimestamp,
            batchProcess.AggregateResult.AverageAge,
            batchProcess.AggregateResult.AverageDependents,
            batchProcess.AggregateResult.AverageHouseholdSize,
            batchProcess.AggregateResult.MaritalStatusCounts.Select(x =>
                new MaritalStatusRecordAverageRecord(x.MaritalStatus, x.AverageCount)).ToList());

        return new BatchProcessRecord(batchProcess.Id, batchProcess.CreatedOn, batchProcess.CompletedOn, batchProcess.Status, aggregateResult);

    }
}