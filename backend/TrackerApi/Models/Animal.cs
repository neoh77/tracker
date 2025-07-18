using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackerApi.Models;

public class Animal
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;
    
    [StringLength(255)]
    public string? Breed { get; set; }
    
    [StringLength(255)]
    public string? Morph { get; set; }
    
    [Column(TypeName = "decimal(10,2)")]
    public decimal? Weight { get; set; }
    
    public DateTime? LastFeedingDate { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<WeightHistory> WeightHistories { get; set; } = new List<WeightHistory>();
    public ICollection<FeedingHistory> FeedingHistories { get; set; } = new List<FeedingHistory>();
}
