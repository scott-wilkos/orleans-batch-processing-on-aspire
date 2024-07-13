namespace BatchProcessing.Domain.Models;

public class MaritalStatusRecordAverage
{
    public string MaritalStatus { get; set; }

    public double AverageCount { get; set; }

    protected MaritalStatusRecordAverage()
    {
        // Parameterless constructor
    }

    public MaritalStatusRecordAverage(string maritalStatus, double averageCount)
    {
        MaritalStatus = maritalStatus;
        AverageCount = averageCount;
    }
}