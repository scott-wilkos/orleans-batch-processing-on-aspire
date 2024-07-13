namespace BatchProcessing.Abstractions.Grains;

[GenerateSerializer]
public record MaritalStatusRecordAverageRecord(string MaritalStatus, double AverageCount);