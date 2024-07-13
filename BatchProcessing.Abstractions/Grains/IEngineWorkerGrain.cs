namespace BatchProcessing.Abstractions.Grains;

public interface IEngineWorkerGrain : IGrainWithStringKey
{
    Task DoWork(Guid id);
}
