namespace BatchProcessing.Domain;

public class BatchProcessItem
{
    public Guid Id { get; set; }

    public Guid BatchProcessId { get; set; }

    public BatchProcessItemStatusEnum Status { get; set; }
}