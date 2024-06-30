namespace BatchProcessing.Grains.Services;

public interface IDataService
{
    Task<SampleData> GetData();

    Task DoDataWork();
}