using BatchProcessing.Abstractions.Grains;

namespace BatchProcessing.Grains;

[GenerateSerializer]
public class EngineGovernorStateRecord(EngineStatusRecord engineStatus, DateTime createdOn, DateTime lastUpdated)
{
    public EngineStatusRecord EngineStatus { get; private set; } = engineStatus;

    public DateTime LastUpdated { get; private set; } = lastUpdated;

    public DateTime CreatedOn { get; private set; } = createdOn;

    public void SetLastUpdated(DateTime lastUpdated) => LastUpdated = lastUpdated;

    public void SetInProgress()
    {
        EngineStatus = EngineStatus with { Status = AnalysisStatusEnum.InProgress };
        SetLastUpdated(DateTime.UtcNow);
    }

    public void SetStatus(AnalysisStatusEnum newState)
    {
        EngineStatus = EngineStatus with { Status = newState };
        SetLastUpdated(DateTime.UtcNow);
    }
}