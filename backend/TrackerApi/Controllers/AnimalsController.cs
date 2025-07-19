using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackerApi.Data;
using TrackerApi.DTOs;
using TrackerApi.Models;
using TrackerApi.Extensions;

namespace TrackerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnimalsController : ControllerBase
{
    private readonly TrackerDbContext _context;

    public AnimalsController(TrackerDbContext context)
    {
        _context = context;
    }

    // GET: api/Animals
    [HttpGet]
    [ResponseCache(Duration = 30, VaryByQueryKeys = new[] { "search" })]
    public async Task<ActionResult<IEnumerable<AnimalDto>>> GetAnimals([FromQuery] string? search = null)
    {
        var query = _context.Animals.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(a => EF.Functions.ILike(a.Name, $"%{search}%") || 
                                   (a.Breed != null && EF.Functions.ILike(a.Breed, $"%{search}%")));
        }

        var animals = await query
            .OrderBy(a => a.Name)
            .ToListAsync();

        var animalDtos = animals.Select(a => a.ToDto()).ToList();

        return Ok(animalDtos);
    }

    // GET: api/Animals/5
    [HttpGet("{id}")]
    [ResponseCache(Duration = 60)]
    public async Task<ActionResult<AnimalDto>> GetAnimal(int id)
    {
        var animal = await _context.Animals.FindAsync(id);

        if (animal == null)
        {
            return NotFound();
        }

        return Ok(animal.ToDto());
    }

    // POST: api/Animals
    [HttpPost]
    public async Task<ActionResult<AnimalDto>> PostAnimal(CreateAnimalDto createAnimalDto)
    {
        var animal = new Animal
        {
            Name = createAnimalDto.Name,
            Breed = createAnimalDto.Breed,
            Morph = createAnimalDto.Morph,
            Weight = createAnimalDto.Weight,
            LastFeedingDate = createAnimalDto.LastFeedingDate,
            FeedingFrequencyDays = createAnimalDto.FeedingFrequencyDays,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            _context.Animals.Add(animal);
            await _context.SaveChangesAsync();

            // Add weight history if weight is provided
            if (animal.Weight.HasValue)
            {
                var weightHistory = new WeightHistory
                {
                    AnimalId = animal.Id,
                    Weight = animal.Weight.Value,
                    RecordedAt = DateTime.UtcNow
                };
                _context.WeightHistories.Add(weightHistory);
                await _context.SaveChangesAsync();
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        return CreatedAtAction(nameof(GetAnimal), new { id = animal.Id }, animal.ToDto());
    }

    // PUT: api/Animals/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAnimal(int id, UpdateAnimalDto updateAnimalDto)
    {
        var animal = await _context.Animals.FindAsync(id);
        
        if (animal == null)
        {
            return NotFound();
        }

        var previousWeight = animal.Weight;

        // Update fields if provided
        if (!string.IsNullOrWhiteSpace(updateAnimalDto.Name))
            animal.Name = updateAnimalDto.Name;
        
        if (updateAnimalDto.Breed != null)
            animal.Breed = updateAnimalDto.Breed;
        
        if (updateAnimalDto.Morph != null)
            animal.Morph = updateAnimalDto.Morph;
        
        if (updateAnimalDto.Weight.HasValue)
            animal.Weight = updateAnimalDto.Weight;
        
        if (updateAnimalDto.LastFeedingDate.HasValue)
            animal.LastFeedingDate = updateAnimalDto.LastFeedingDate;

        if (updateAnimalDto.FeedingFrequencyDays.HasValue)
            animal.FeedingFrequencyDays = updateAnimalDto.FeedingFrequencyDays.Value;

        animal.UpdatedAt = DateTime.UtcNow;

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _context.SaveChangesAsync();

            // Add weight history if weight changed
            if (updateAnimalDto.Weight.HasValue && updateAnimalDto.Weight != previousWeight)
            {
                var weightHistory = new WeightHistory
                {
                    AnimalId = animal.Id,
                    Weight = updateAnimalDto.Weight.Value,
                    RecordedAt = DateTime.UtcNow
                };
                _context.WeightHistories.Add(weightHistory);
                await _context.SaveChangesAsync();
            }

            await transaction.CommitAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync();
            if (!AnimalExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

        return NoContent();
    }

    // DELETE: api/Animals/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAnimal(int id)
    {
        var animal = await _context.Animals.FindAsync(id);
        if (animal == null)
        {
            return NotFound();
        }

        _context.Animals.Remove(animal);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/Animals/5/weight-history
    [HttpGet("{id}/weight-history")]
    [ResponseCache(Duration = 60)]
    public async Task<ActionResult<IEnumerable<WeightHistoryDto>>> GetAnimalWeightHistory(int id)
    {
        var animal = await _context.Animals.FindAsync(id);
        if (animal == null)
        {
            return NotFound();
        }

        var weightHistory = await _context.WeightHistories
            .Where(wh => wh.AnimalId == id)
            .OrderBy(wh => wh.RecordedAt)
            .ToListAsync();

        var weightHistoryDtos = weightHistory.Select(wh => wh.ToDto()).ToList();

        return Ok(weightHistoryDtos);
    }

    // GET: api/Animals/5/feeding-history
    [HttpGet("{id}/feeding-history")]
    [ResponseCache(Duration = 60)]
    public async Task<ActionResult<IEnumerable<FeedingHistoryDto>>> GetAnimalFeedingHistory(int id)
    {
        var animal = await _context.Animals.FindAsync(id);
        if (animal == null)
        {
            return NotFound();
        }

        var feedingHistory = await _context.FeedingHistories
            .Where(fh => fh.AnimalId == id)
            .OrderByDescending(fh => fh.FeedingDate)
            .ToListAsync();

        var feedingHistoryDtos = feedingHistory.Select(fh => fh.ToDto()).ToList();

        return Ok(feedingHistoryDtos);
    }

    private bool AnimalExists(int id)
    {
        return _context.Animals.Any(e => e.Id == id);
    }
}
