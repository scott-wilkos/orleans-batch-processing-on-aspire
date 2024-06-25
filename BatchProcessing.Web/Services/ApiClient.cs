namespace BatchProcessing.Web.Services;

public class ApiClient(HttpClient httpClient)
{

    public async Task<Guid> StartBatchProcessingAsync(int records, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync($"/batchProcessing/{records}", records, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Guid>(cancellationToken: cancellationToken);
    }

    public async Task<EngineStatusRecord?> GetBatchProcessingStatusAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var response =
            await httpClient.GetFromJsonAsync<EngineStatusRecord>($"/batchProcessing/{id}/status", cancellationToken);

        return response;
    }
}