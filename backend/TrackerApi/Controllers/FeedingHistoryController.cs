using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TrackerApi.Data;
using TrackerApi.DTOs;
using TrackerApi.Models;
using TrackerApi.Extensions;

namespace TrackerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FeedingHistoryController : ControllerBase
{
    private readonly TrackerDbContext _context;

    public FeedingHistoryController(TrackerDbContext context)
    {
        _context = context;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            throw new UnauthorizedAccessException("Invalid user ID");
        }
        return userId;
    }

    // GET: api/FeedingHistory
    [HttpGet]
    [ResponseCache(Duration = 30, VaryByQueryKeys = new[] { "animalId" })]
    public async Task<ActionResult<IEnumerable<FeedingHistoryDto>>> GetFeedingHistory([FromQuery] int? animalId = null)
    {
        var userId = GetUserId();
        var query = _context.FeedingHistories
            .Join(_context.Animals, fh => fh.AnimalId, a => a.Id, (fh, a) => new { FeedingHistory = fh, Animal = a })
            .Where(x => x.Animal.UserId == userId)
            .Select(x => x.FeedingHistory);

        if (animalId.HasValue)
        {
            query = query.Where(fh => fh.AnimalId == animalId.Value);
        }

        var feedingHistory = await query
            .OrderByDescending(fh => fh.FeedingDate)
            .ToListAsync();

        var feedingHistoryDtos = feedingHistory.Select(fh => fh.ToDto()).ToList();

        return Ok(feedingHistoryDtos);
    }

    // POST: api/FeedingHistory
    [HttpPost]
    public async Task<ActionResult<FeedingHistoryDto>> PostFeedingHistory(CreateFeedingHistoryDto createFeedingHistoryDto)
    {
        var userId = GetUserId();
        // Find animal and check if it exists and belongs to user
        var animal = await _context.Animals.FirstOrDefaultAsync(a => a.Id == createFeedingHistoryDto.AnimalId && a.UserId == userId);
        if (animal == null)
        {
            return BadRequest("Animal not found or access denied");
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
        animal.LastFeedingDate = createFeedingHistoryDto.FeedingDate;
        animal.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetFeedingHistory), new { id = feedingHistory.Id }, feedingHistory.ToDto());
    }

    // DELETE: api/FeedingHistory/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFeedingHistory(int id)
    {
        var userId = GetUserId();
        var feedingHistory = await _context.FeedingHistories
            .Join(_context.Animals, fh => fh.AnimalId, a => a.Id, (fh, a) => new { FeedingHistory = fh, Animal = a })
            .Where(x => x.FeedingHistory.Id == id && x.Animal.UserId == userId)
            .Select(x => x.FeedingHistory)
            .FirstOrDefaultAsync();
            
        if (feedingHistory == null)
        {
            return NotFound();
        }

        _context.FeedingHistories.Remove(feedingHistory);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
