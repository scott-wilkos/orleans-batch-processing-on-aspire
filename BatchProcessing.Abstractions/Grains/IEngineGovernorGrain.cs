namespace BatchProcessing.Abstractions.Grains;

public interface IEngineGovernorGrain : IGrainWithIntegerKey
{
    Task<TryStartResponse> TryStartEngine(EngineStatusRecord engineStatus);

    Task UpdateStatus(EngineStatusRecord engineStatus);
}