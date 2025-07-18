namespace TrackerApi.DTOs;

public class FeedingHistoryDto
{
    public int Id { get; set; }
    public int AnimalId { get; set; }
    public DateTime FeedingDate { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateFeedingHistoryDto
{
    public int AnimalId { get; set; }
    public DateTime FeedingDate { get; set; }
    public string? Notes { get; set; }
}
