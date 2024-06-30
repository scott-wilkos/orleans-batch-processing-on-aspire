using BatchProcessing.Abstractions.Grains;

namespace BatchProcessing.Grains;

[GenerateSerializer]
public class EngineGovernorStateRecord(EngineStatusRecord engineStatus, DateTime lastUpdated)
{
    public EngineStatusRecord EngineStatus { get; private set; } = engineStatus;

    public DateTime LastUpdated { get; private set; } = lastUpdated;

    public void SetLastUpdated(DateTime lastUpdated) => LastUpdated = lastUpdated;

    public void SetState(AnalysisStatusEnum newState)
    {
        EngineStatus = EngineStatus with { Status = newState };
        SetLastUpdated(DateTime.UtcNow);
    }
}