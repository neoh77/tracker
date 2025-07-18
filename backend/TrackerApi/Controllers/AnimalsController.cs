using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackerApi.Data;
using TrackerApi.DTOs;
using TrackerApi.Models;

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
    public async Task<ActionResult<IEnumerable<AnimalDto>>> GetAnimals([FromQuery] string? search = null)
    {
        var query = _context.Animals.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(a => a.Name.Contains(search) || (a.Breed != null && a.Breed.Contains(search)));
        }

        var animals = await query
            .OrderBy(a => a.Name)
            .Select(a => new AnimalDto
            {
                Id = a.Id,
                Name = a.Name,
                Breed = a.Breed,
                Morph = a.Morph,
                Weight = a.Weight,
                LastFeedingDate = a.LastFeedingDate,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            })
            .ToListAsync();

        return Ok(animals);
    }

    // GET: api/Animals/5
    [HttpGet("{id}")]
    public async Task<ActionResult<AnimalDto>> GetAnimal(int id)
    {
        var animal = await _context.Animals.FindAsync(id);

        if (animal == null)
        {
            return NotFound();
        }

        var animalDto = new AnimalDto
        {
            Id = animal.Id,
            Name = animal.Name,
            Breed = animal.Breed,
            Morph = animal.Morph,
            Weight = animal.Weight,
            LastFeedingDate = animal.LastFeedingDate,
            CreatedAt = animal.CreatedAt,
            UpdatedAt = animal.UpdatedAt
        };

        return Ok(animalDto);
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
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

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

        var animalDto = new AnimalDto
        {
            Id = animal.Id,
            Name = animal.Name,
            Breed = animal.Breed,
            Morph = animal.Morph,
            Weight = animal.Weight,
            LastFeedingDate = animal.LastFeedingDate,
            CreatedAt = animal.CreatedAt,
            UpdatedAt = animal.UpdatedAt
        };

        return CreatedAtAction(nameof(GetAnimal), new { id = animal.Id }, animalDto);
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

        animal.UpdatedAt = DateTime.UtcNow;

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
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AnimalExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
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
            .Select(wh => new WeightHistoryDto
            {
                Id = wh.Id,
                AnimalId = wh.AnimalId,
                Weight = wh.Weight,
                RecordedAt = wh.RecordedAt
            })
            .ToListAsync();

        return Ok(weightHistory);
    }

    // GET: api/Animals/5/feeding-history
    [HttpGet("{id}/feeding-history")]
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

    private bool AnimalExists(int id)
    {
        return _context.Animals.Any(e => e.Id == id);
    }
}
