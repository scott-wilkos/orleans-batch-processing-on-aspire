﻿using BatchProcessing.Abstractions.Configuration;
using BatchProcessing.Abstractions.Grains;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans.Concurrency;

namespace BatchProcessing.Grains;

internal class EngineGrain(IOptions<EngineConfig> config, ILogger<EngineGrain> logger) : Grain, IEngineGrain
{
    // This is only used to allow for varying process sizes
    private int _recordsToSimulate;
    
    private readonly CancellationTokenSource _shutdownCancellation = new();
    private Task? _backgroundTask;

    // Worker Count would be set through configuration and drives how many workers we have processing records
    private readonly int _workerCount = config.Value.WorkerCount;

    private int _recordCount;
    private int _recordsProcessed;

    private AnalysisStatusEnum _status = AnalysisStatusEnum.NotStarted;

    public Task RunAnalysis(int recordsToSimulate)
    {
        if (_backgroundTask is null or { IsCompleted: true })
        {
            _status = AnalysisStatusEnum.InProgress;

            _recordsToSimulate = recordsToSimulate;   
            _backgroundTask = ProcessBackgroundTask();
        }

        return Task.CompletedTask;
    }

    [ReadOnly]
    public Task<EngineStatusRecord> GetStatus()
    {
        var status = new EngineStatusRecord(this.GetPrimaryKey(), _status, _recordCount, _recordsProcessed);

        return Task.FromResult(status);
    }

    private async Task ProcessBackgroundTask()
    {
        await Task.Yield();

        _recordCount = await GetRecordCount();

        var workerTasks = new List<Task>();

        while (!_shutdownCancellation.IsCancellationRequested)
        {
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

            logger.LogInformation("{ProcessId} - Processed {Count} of {Total} records", this.GetPrimaryKey(), _recordsProcessed, _recordCount);
        }

        _status = AnalysisStatusEnum.Completed;
    }

    private Task<int> GetRecordCount()
    {
        // Would call and get # of records to be processed
        return Task.FromResult(_recordsToSimulate);
    }

    private Task<List<AnalysisRecord>> GetBatch()
    {
        // Would call and get a batch of records to process
        // Short circuit if we have gotten "all" of the records
        if (_recordsProcessed >= _recordCount)
            return Task.FromResult(new List<AnalysisRecord>());

        return Task.FromResult(Enumerable.Range(0, 10).Select(_ => new AnalysisRecord(Guid.NewGuid())).ToList());
    }

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