namespace BatchProcessing.ApiService.Grains;

[GenerateSerializer]
public record EngineStatusRecord(Guid Id, AnalysisStatus Status, int RecordCount, int RecordsProcessed);