using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackerApi.Data;
using TrackerApi.DTOs;
using TrackerApi.Models;

namespace TrackerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeedingHistoryController : ControllerBase
{
    private readonly TrackerDbContext _context;

    public FeedingHistoryController(TrackerDbContext context)
    {
        _context = context;
    }

    // GET: api/FeedingHistory
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FeedingHistoryDto>>> GetFeedingHistory([FromQuery] int? animalId = null)
    {
        var query = _context.FeedingHistories.AsQueryable();

        if (animalId.HasValue)
        {
            query = query.Where(fh => fh.AnimalId == animalId.Value);
        }

        var feedingHistory = await query
            .OrderByDescending(fh => fh.FeedingDate)
            .Select(fh => new FeedingHistoryDto
            {
                Id = fh.Id,
                AnimalId = fh.AnimalId,
                FeedingDate = fh.FeedingDate,
                Notes = fh.Notes,
                CreatedAt = fh.CreatedAt
            })
            .ToListAsync();

        return Ok(feedingHistory);
    }

    // POST: api/FeedingHistory
    [HttpPost]
    public async Task<ActionResult<FeedingHistoryDto>> PostFeedingHistory(CreateFeedingHistoryDto createFeedingHistoryDto)
    {
        // Check if animal exists
        var animalExists = await _context.Animals.AnyAsync(a => a.Id == createFeedingHistoryDto.AnimalId);
        if (!animalExists)
        {
            return BadRequest("Animal not found");
        }

        var feedingHistory = new FeedingHistory
        {
            AnimalId = createFeedingHistoryDto.AnimalId,
            FeedingDate = createFeedingHistoryDto.FeedingDate,
            Notes = createFeedingHistoryDto.Notes,
            CreatedAt = DateTime.UtcNow
        };

        _context.FeedingHistories.Add(feedingHistory);

        // Update animal's last feeding date
        var animal = await _context.Animals.FindAsync(createFeedingHistoryDto.AnimalId);
        if (animal != null)
        {
            animal.LastFeedingDate = createFeedingHistoryDto.FeedingDate;
            animal.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        var feedingHistoryDto = new FeedingHistoryDto
        {
            Id = feedingHistory.Id,
            AnimalId = feedingHistory.AnimalId,
            FeedingDate = feedingHistory.FeedingDate,
            Notes = feedingHistory.Notes,
            CreatedAt = feedingHistory.CreatedAt
        };

        return CreatedAtAction(nameof(GetFeedingHistory), new { id = feedingHistory.Id }, feedingHistoryDto);
    }

    // DELETE: api/FeedingHistory/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFeedingHistory(int id)
    {
        var feedingHistory = await _context.FeedingHistories.FindAsync(id);
        if (feedingHistory == null)
        {
            return NotFound();
        }

        _context.FeedingHistories.Remove(feedingHistory);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
