using BatchProcessing.Abstractions.Grains;

namespace BatchProcessing.Grains;

/// <summary>
/// The EngineGovernorGrain class is responsible for managing the number of engines that can run concurrently.
/// It ensures that the number of engines in the "InProgress" state does not exceed the maximum capacity.
/// </summary>
internal class EngineGovernorGrain : IEngineGovernorGrain
{
    // List to maintain the state of each engine
    private List<EngineGovernorStateRecord> _state = new();

    // Maximum number of engines that can run concurrently
    private const int MaxCapacity = 3;

    /// <summary>
    /// Attempts to start an engine. If the engine is already in the state list, it updates the last updated time.
    /// If the number of engines in the "InProgress" state is less than the maximum capacity, it sets the engine state to "InProgress".
    /// </summary>
    /// <param name="engineStatus">The status record of the engine to be started.</param>
    /// <returns>A TryStartResponse indicating whether the engine was successfully started or not.</returns>
    public Task<TryStartResponse> TryStartEngine(EngineStatusRecord engineStatus)
    {
        // Check to see if this engine is already in the state list
        if (_state.All(x => x.EngineStatus.Id != engineStatus.Id))
        {
            _state.Add(new EngineGovernorStateRecord(engineStatus, engineStatus.CreatedOnUtc, DateTime.UtcNow));
        }

        // Check if we have capacity based on what is running
        if (_state.Count(x => x.EngineStatus.Status == AnalysisStatusEnum.InProgress) >= MaxCapacity)
        {
            // Update last updated time for the engine
            var state = _state.FirstOrDefault(x => x.EngineStatus.Id == engineStatus.Id);
            if (state is not null)
            {
                state.SetLastUpdated(DateTime.UtcNow);
            }

            return Task.FromResult(new TryStartResponse(engineStatus.Id, false, "Engine at capacity"));
        }

        // check to see if this is the next available item to run based on created time
        var nextAvailable = _state
            .Where(x => x.EngineStatus.Status == AnalysisStatusEnum.NotStarted)
            .MinBy(x => x.CreatedOn);

        if (nextAvailable is not null && nextAvailable.EngineStatus.Id != engineStatus.Id)
        {
            // Update last updated time for the engine
            var state = _state.FirstOrDefault(x => x.EngineStatus.Id == engineStatus.Id);
            if (state is not null)
            {
                state.SetLastUpdated(DateTime.UtcNow);
            }

            return Task.FromResult(new TryStartResponse(engineStatus.Id, false, "Engine not next available"));
        }

        // Set the engine state to "InProgress"
        var stateInProgress = _state.FirstOrDefault(x => x.EngineStatus.Id == engineStatus.Id);
        if (stateInProgress is not null)
        {
            stateInProgress.SetState(AnalysisStatusEnum.InProgress);
        }

        return Task.FromResult(new TryStartResponse(engineStatus.Id, true, string.Empty));
    }

    /// <summary>
    /// Updates the status of an engine in the state list.
    /// </summary>
    /// <param name="engineStatus">The status record of the engine to be updated.</param>
    /// <returns>A Task representing the asynchronous operation.</returns>
    public Task UpdateStatus(EngineStatusRecord engineStatus)
    {
        var state = _state.FirstOrDefault(x => x.EngineStatus.Id == engineStatus.Id);
        if (state is not null)
        {
            state.SetState(engineStatus.Status);
        }

        return Task.CompletedTask;
    }
}
