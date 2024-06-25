namespace BatchProcessing.Web.Services;

public record EngineStatusRecord
{
    public Guid Id { get; set; }

    public AnalysisStatus Status { get; set; }

    public int RecordCount { get; set; }

    public int RecordsProcessed { get; set; }
}