using System.ComponentModel.DataAnnotations;

namespace TrackerApi.Models;

public class FeedingHistory
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int AnimalId { get; set; }
    
    [Required]
    public DateTime FeedingDate { get; set; }
    
    public string? Notes { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public Animal Animal { get; set; } = null!;
}
