namespace TrackerApi.DTOs;

public class WeightHistoryDto
{
    public int Id { get; set; }
    public int AnimalId { get; set; }
    public decimal Weight { get; set; }
    public DateTime RecordedAt { get; set; }
}

public class CreateWeightHistoryDto
{
    public int AnimalId { get; set; }
    public decimal Weight { get; set; }
    public DateTime? RecordedAt { get; set; }
}
