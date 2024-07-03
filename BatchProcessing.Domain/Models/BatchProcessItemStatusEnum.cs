namespace BatchProcessing.Domain.Models;

public enum BatchProcessItemStatusEnum
{
    Created = 0,
    Running = 1,
    Completed = 2,
    Error = 3
}