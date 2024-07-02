using BatchProcessing.Abstractions.Configuration;
using BatchProcessing.Abstractions.Grains;
using BatchProcessing.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;

using Orleans.Concurrency;

namespace BatchProcessing.Grains;

/// <summary>
/// The EngineGrain class is responsible for simulating the processing of records.
/// It coordinates with the EngineGovernorGrain to ensure that the number of concurrently running engines does not exceed the allowed capacity.
/// </summary>
internal class EngineGrain(ContextFactory contextFactory, IOptions<EngineConfig> config, ILogger<EngineGrain> logger) : Grain, IEngineGrain
{
    // This is only used to allow for varying process sizes
    private int _recordsToSimulate;

    private readonly CancellationTokenSource _shutdownCancellation = new();
    private Task? _backgroundTask;

    // Worker Count would be set through configuration and drives how many workers we have processing records
    private readonly int _workerCount = config.Value.WorkerCount;

    private int _recordCount;
    private int _recordsProcessed;
    private readonly DateTime _createdOn = DateTime.UtcNow;

    private AnalysisStatusEnum _status = AnalysisStatusEnum.NotStarted;

    /// <summary>
    /// Initiates the analysis process with the specified number of records to simulate.
    /// </summary>
    /// <param name="recordsToSimulate">The number of records to simulate processing.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public async Task RunAnalysis(int recordsToSimulate)
    {
        if (_backgroundTask is null or { IsCompleted: true })
        {
            await CreateRecords(recordsToSimulate);

            _status = AnalysisStatusEnum.NotStarted;

            _recordsToSimulate = recordsToSimulate;
            _backgroundTask = ProcessBackgroundTask();
        }
    }

    private async Task CreateRecords(int recordsToSimulate)
    {
        await using var context = contextFactory.Create();

        var batchProcess = new BatchProcess
        {
            Id = this.GetPrimaryKey(),
            Status = BatchProcessStatusEnum.Created,
            CreatedOn = DateTime.UtcNow
        };
        context.BatchProcesses.Add(batchProcess);

        await context.SaveChangesAsync();

        var items = new List<BatchProcessItem>();

        for (var i = 0; i < recordsToSimulate; i++)
        {
            var item = new BatchProcessItem
            {
                Id = Guid.NewGuid(),
                BatchProcessId = batchProcess.Id,
                Status = BatchProcessItemStatusEnum.Created,
                CreatedOnUtc = DateTime.UtcNow
            };
            items.Add(item);
        }

        await context.BulkInsert(items);

        Console.WriteLine($"Created {recordsToSimulate} records for batch process {batchProcess.Id}");
        logger.LogInformation("Created {RecordCount} records for batch process {BatchProcessId}", recordsToSimulate, batchProcess.Id);

        var records = context.BatchProcessItems.Count();
        logger.LogInformation("Total records: {RecordCount}", records);
    }

    /// <summary>
    /// Retrieves the current status of the engine.
    /// </summary>
    /// <returns>A Task containing the EngineStatusRecord with the current status.</returns>
    [ReadOnly]
    public Task<EngineStatusRecord> GetStatus()
    {
        var status = new EngineStatusRecord(this.GetPrimaryKey(), _status, _recordCount, _recordsProcessed, _createdOn);

        return Task.FromResult(status);
    }

    /// <summary>
    /// Processes the background task for record analysis.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    private async Task ProcessBackgroundTask()
    {
        await Task.Yield();

        _recordCount = await GetRecordCount();

        var workerTasks = new List<Task>();

        var governor = GrainFactory.GetGrain<IEngineGovernorGrain>(0);

        while (!_shutdownCancellation.IsCancellationRequested)
        {
            // Check with the Governor to see if we can start
            if (_status == AnalysisStatusEnum.NotStarted)
            {
                var response = await governor.TryStartEngine(new EngineStatusRecord(this.GetPrimaryKey(), _status, _recordCount, _recordsProcessed, _createdOn));

                if (!response.Success)
                {
                    logger.LogInformation("{ProcessId} - Unable to start processing: {Reason}", this.GetPrimaryKey(), response.Reason);
                    await Task.Delay(1000);
                    continue;
                }

                _status = AnalysisStatusEnum.InProgress;
            }

            var batch = await GetBatch();

            if (!batch.Any())
            {
                break;
            }

            logger.LogInformation("{ProcessId} - Processing batch of {Count} records", this.GetPrimaryKey(), batch.Count);

            for (var i = 0; i < _workerCount; i++)
            {
                var worker = GrainFactory.GetGrain<IEngineWorkerGrain>($"{this.GetGrainId()}-{i}");
                workerTasks.Add(worker.DoWork());
            }

            await Task.WhenAll(workerTasks);

            _recordsProcessed += batch.Count;

            // Update Governor with status
            await governor.UpdateStatus(new EngineStatusRecord(this.GetPrimaryKey(), _status, _recordCount, _recordsProcessed, _createdOn));
        }

        _status = AnalysisStatusEnum.Completed;
        // Update Governor with status
        await governor.UpdateStatus(new EngineStatusRecord(this.GetPrimaryKey(), _status, _recordCount, _recordsProcessed, _createdOn));
    }

    /// <summary>
    /// Retrieves the total number of records to be processed.
    /// </summary>
    /// <returns>A Task containing the number of records to be processed.</returns>
    private Task<int> GetRecordCount()
    {
        // Would call and get # of records to be processed
        return Task.FromResult(_recordsToSimulate);
    }

    /// <summary>
    /// Retrieves a batch of records to process.
    /// </summary>
    /// <returns>A Task containing a list of records to process.</returns>
    private Task<List<AnalysisRecord>> GetBatch()
    {
        // Would call and get a batch of records to process
        // Short circuit if we have gotten "all" of the records
        if (_recordsProcessed >= _recordCount)
            return Task.FromResult(new List<AnalysisRecord>());

        return Task.FromResult(Enumerable.Range(0, 10).Select(_ => new AnalysisRecord(Guid.NewGuid())).ToList());
    }

    /// <summary>
    /// Handles the deactivation of the grain, ensuring that the background task is properly canceled and awaited.
    /// </summary>
    /// <param name="reason">The reason for deactivation.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        _shutdownCancellation.Cancel();
        if (_backgroundTask is { } task && !task.IsCompleted)
        {
            // Wait for the background task to complete, but don't wait indefinitely.
            await task.WaitAsync(cancellationToken);
        }
    }
}
