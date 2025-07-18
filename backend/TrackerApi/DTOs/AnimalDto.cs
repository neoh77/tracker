namespace TrackerApi.DTOs;

public class AnimalDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Breed { get; set; }
    public string? Morph { get; set; }
    public decimal? Weight { get; set; }
    public DateTime? LastFeedingDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateAnimalDto
{
    public string Name { get; set; } = string.Empty;
    public string? Breed { get; set; }
    public string? Morph { get; set; }
    public decimal? Weight { get; set; }
    public DateTime? LastFeedingDate { get; set; }
}

public class UpdateAnimalDto
{
    public string? Name { get; set; }
    public string? Breed { get; set; }
    public string? Morph { get; set; }
    public decimal? Weight { get; set; }
    public DateTime? LastFeedingDate { get; set; }
}
