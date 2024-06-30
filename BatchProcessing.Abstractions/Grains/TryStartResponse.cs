namespace BatchProcessing.Abstractions.Grains;

[GenerateSerializer]
public record TryStartResponse(Guid Id, bool Success, string Reason);