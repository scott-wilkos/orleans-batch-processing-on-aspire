namespace BatchProcessing.Grains.Services;

public class DataService : IDataService
{
    static readonly Random Random = new Random();
    public async Task<SampleData> GetData()
    {
        // Delay to simulate a real service call
        await Task.Delay(Random.Next(50, 150));

        return new SampleData();
    }

    public async Task DoDataWork()
    {
        await Task.Delay(Random.Next(50, 150));
    }
}