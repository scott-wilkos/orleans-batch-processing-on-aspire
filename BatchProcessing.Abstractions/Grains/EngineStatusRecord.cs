namespace BatchProcessing.Abstractions.Grains;

[GenerateSerializer]
public record EngineStatusRecord(
    Guid Id,
    AnalysisStatusEnum Status,
    int RecordCount,
    int RecordsProcessed,
    DateTime CreatedOnUtc);