namespace BatchProcessing.Domain.Models;

public record BatchProcessAggregateResult(
    Guid BatchProcessId,
    DateTime AnalysisTimestamp,
    double AverageAge,
    int TotalDependents,
    double AverageHouseholdSize,
    Dictionary<string, int> MaritalStatusCounts
);