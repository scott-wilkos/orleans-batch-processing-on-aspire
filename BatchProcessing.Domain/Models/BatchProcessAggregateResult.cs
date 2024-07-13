namespace BatchProcessing.Domain.Models;

public class BatchProcessAggregateResult
{
    public Guid BatchProcessId { get; set; }
    public DateTime AnalysisTimestamp { get; set; }
    public double AverageAge { get; set; }
    public double AverageDependents { get; set; }
    public double AverageHouseholdSize { get; set; }

    public IEnumerable<MaritalStatusRecordAverage> MaritalStatusCounts { get; set; } = new List<MaritalStatusRecordAverage>();

    public BatchProcessAggregateResult()
    {
    }

    public BatchProcessAggregateResult(Guid batchProcessId, DateTime analysisTimestamp, double averageAge, double averageDependents, double averageHouseholdSize, ICollection<MaritalStatusRecordAverage> maritalStatusCounts)
    {
        BatchProcessId = batchProcessId;
        AnalysisTimestamp = analysisTimestamp;
        AverageAge = averageAge;
        AverageDependents = averageDependents;
        AverageHouseholdSize = averageHouseholdSize;
        MaritalStatusCounts = maritalStatusCounts;
    }
}

public class MaritalStatusRecordAverage
{
    public string MaritalStatus { get; set; }

    public double AverageCount { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected MaritalStatusRecordAverage()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    {
        // Parameterless constructor
    }

    public MaritalStatusRecordAverage(string maritalStatus, double averageCount)
    {
        MaritalStatus = maritalStatus;
        AverageCount = averageCount;
    }
}
