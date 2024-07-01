using BatchProcessing.Abstractions.Grains;

namespace BatchProcessing.Grains;

/// <summary>
/// The EngineGovernorGrain class is responsible for managing the number of engines that can run concurrently.
/// It ensures that the number of engines in the "InProgress" state does not exceed the maximum capacity.
/// </summary>
internal class EngineGovernorGrain : IEngineGovernorGrain
{
    // Dictionary to maintain the state of each engine for faster lookups
    private readonly Dictionary<Guid, EngineGovernorStateRecord> _state = new();

    // Maximum number of engines that can run concurrently
    private const int MaxCapacity = 3;

    /// <summary>
    /// Attempts to start an engine. If the engine is already in the state dictionary, it updates the last updated time.
    /// If the number of engines in the "InProgress" state is less than the maximum capacity, it sets the engine state to "InProgress".
    /// </summary>
    /// <param name="engineStatus">The status record of the engine to be started.</param>
    /// <returns>A TryStartResponse indicating whether the engine was successfully started or not.</returns>
    public Task<TryStartResponse> TryStartEngine(EngineStatusRecord engineStatus)
    {
        if (!_state.TryGetValue(engineStatus.Id, out var state))
        {
            state = new EngineGovernorStateRecord(engineStatus, engineStatus.CreatedOnUtc, DateTime.UtcNow);
            _state[engineStatus.Id] = state;
        }

        // Check if we have capacity based on what is running
        if (_state.Values.Count(x => x.EngineStatus.Status == AnalysisStatusEnum.InProgress) >= MaxCapacity)
        {
            state.SetLastUpdated(DateTime.UtcNow);
            return Task.FromResult(new TryStartResponse(engineStatus.Id, false, "Engine at capacity"));
        }

        // Check to see if this is the next available item to run based on created time
        var nextAvailable = _state.Values
            .Where(x => x.EngineStatus.Status == AnalysisStatusEnum.NotStarted).MinBy(x => x.CreatedOn);

        if (nextAvailable is not null && nextAvailable.EngineStatus.Id != engineStatus.Id)
        {
            state.SetLastUpdated(DateTime.UtcNow);
            return Task.FromResult(new TryStartResponse(engineStatus.Id, false, "Not next in queue"));
        }

        // Set the engine state to "InProgress"
        state.SetInProgress();

        return Task.FromResult(new TryStartResponse(engineStatus.Id, true, string.Empty));
    }

    /// <summary>
    /// Updates the status of an engine in the state dictionary.
    /// </summary>
    /// <param name="engineStatus">The status record of the engine to be updated.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public Task UpdateStatus(EngineStatusRecord engineStatus)
    {
        if (_state.TryGetValue(engineStatus.Id, out var state))
        {
            state.SetStatus(engineStatus.Status);
        }

        return Task.CompletedTask;
    }
}
