namespace BatchProcessing.Domain.Models;

public class BatchProcessItem
{
    public Guid Id { get; set; }

    public Guid BatchProcessId { get; set; }

    public BatchProcessItemStatusEnum Status { get; set; }

    public DateTime CreatedOnUtc { get; set; }

    public Person Person { get; set; }
}