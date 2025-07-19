using TrackerApi.DTOs;
using TrackerApi.Models;

namespace TrackerApi.Extensions;

public static class EntityExtensions
{
    public static AnimalDto ToDto(this Animal animal)
    {
        return new AnimalDto
        {
            Id = animal.Id,
            Name = animal.Name,
            Breed = animal.Breed,
            Morph = animal.Morph,
            Weight = animal.Weight,
            LastFeedingDate = animal.LastFeedingDate,
            FeedingFrequencyDays = animal.FeedingFrequencyDays,
            CreatedAt = animal.CreatedAt,
            UpdatedAt = animal.UpdatedAt
        };
    }

    public static WeightHistoryDto ToDto(this WeightHistory weightHistory)
    {
        return new WeightHistoryDto
        {
            Id = weightHistory.Id,
            AnimalId = weightHistory.AnimalId,
            Weight = weightHistory.Weight,
            RecordedAt = weightHistory.RecordedAt
        };
    }

    public static FeedingHistoryDto ToDto(this FeedingHistory feedingHistory)
    {
        return new FeedingHistoryDto
        {
            Id = feedingHistory.Id,
            AnimalId = feedingHistory.AnimalId,
            FeedingDate = feedingHistory.FeedingDate,
            Notes = feedingHistory.Notes,
            CreatedAt = feedingHistory.CreatedAt
        };
    }
}