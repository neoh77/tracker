export interface Animal {
  id: number;
  name: string;
  breed?: string;
  morph?: string;
  weight?: number;
  lastFeedingDate?: string;
  createdAt: string;
  updatedAt: string;
}

export interface CreateAnimalDto {
  name: string;
  breed?: string;
  morph?: string;
  weight?: number;
  lastFeedingDate?: string;
}

export interface UpdateAnimalDto {
  name?: string;
  breed?: string;
  morph?: string;
  weight?: number;
  lastFeedingDate?: string;
}

export interface WeightHistory {
  id: number;
  animalId: number;
  weight: number;
  recordedAt: string;
}

export interface CreateWeightHistoryDto {
  animalId: number;
  weight: number;
  recordedAt?: string;
}

export interface FeedingHistory {
  id: number;
  animalId: number;
  feedingDate: string;
  notes?: string;
  createdAt: string;
}

export interface CreateFeedingHistoryDto {
  animalId: number;
  feedingDate: string;
  notes?: string;
}
