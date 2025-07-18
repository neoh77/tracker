using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackerApi.Models;

public class WeightHistory
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int AnimalId { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(10,2)")]
    public decimal Weight { get; set; }
    
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public Animal Animal { get; set; } = null!;
}
