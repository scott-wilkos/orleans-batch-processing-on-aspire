namespace BatchProcessing.ApiService.Grains;

public interface IEngineWorkerGrain : IGrainWithStringKey
{
    Task DoWork();
}