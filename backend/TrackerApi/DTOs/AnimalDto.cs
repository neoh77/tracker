using System.ComponentModel.DataAnnotations;

namespace TrackerApi.DTOs;

public class AnimalDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Breed { get; set; }
    public string? Morph { get; set; }
    public decimal? Weight { get; set; }
    public DateTime? LastFeedingDate { get; set; }
    public int FeedingFrequencyDays { get; set; } = 7;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateAnimalDto
{
    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(255)]
    public string? Breed { get; set; }
    
    [StringLength(255)]
    public string? Morph { get; set; }
    
    [Range(0.01, 9999999.99)]
    public decimal? Weight { get; set; }
    
    public DateTime? LastFeedingDate { get; set; }
    
    [Range(1, 365)]
    public int FeedingFrequencyDays { get; set; } = 7;
}

public class UpdateAnimalDto
{
    [StringLength(255, MinimumLength = 1)]
    public string? Name { get; set; }
    
    [StringLength(255)]
    public string? Breed { get; set; }
    
    [StringLength(255)]
    public string? Morph { get; set; }
    
    [Range(0.01, 9999999.99)]
    public decimal? Weight { get; set; }
    
    public DateTime? LastFeedingDate { get; set; }
    
    [Range(1, 365)]
    public int? FeedingFrequencyDays { get; set; }
}
