namespace BatchProcessing.Domain.Models;

public class BatchProcess
{
    public Guid Id { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime? CompletedOn { get; set; }

    public BatchProcessStatusEnum Status { get; set; }

    public List<BatchProcessItem> Items { get; set; } = new List<BatchProcessItem>();
}