namespace BatchProcessing.Abstractions.Grains;

public interface IBatchProcessManagerGrain : IGrainWithIntegerKey
{
    public Task<IEnumerable<BatchProcessRecord>> GetBatchProcesses();

    public Task<BatchProcessRecord?> GetBatchProcess(Guid engineId);
}