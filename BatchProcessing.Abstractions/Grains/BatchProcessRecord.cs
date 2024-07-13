using BatchProcessing.Domain.Models;
using BatchProcessing.Shared;

namespace BatchProcessing.Abstractions.Grains;

[GenerateSerializer]
public record BatchProcessRecord(
    Guid Id,
    DateTime CreatedOn,
    DateTime? CompletedOn,
    BatchProcessStatusEnum Status,
    BatchProcessAggregateResultRecord? AggregateResult = null);

[GenerateSerializer]
public record BatchProcessAggregateResultRecord(
    Guid BatchProcessId,
    DateTime AnalysisTimestamp,
    double AverageAge,
    double AverageDependents,
    double AverageHouseholdSize,
    List<MaritalStatusRecordAverageRecord> MaritalStatusCounts);

[GenerateSerializer]
public record MaritalStatusRecordAverageRecord(string MaritalStatus, double AverageCount);