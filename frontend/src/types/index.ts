export interface Animal {
  id: number;
  name: string;
  breed: string;
  morph?: string;
  weight?: number;
  lastFeedingDate?: string;
  feedingFrequencyDays: number;
  createdAt: string;
  updatedAt: string;
}

export interface CreateAnimalDto {
  name: string;
  breed: string;
  morph?: string;
  weight?: number;
  lastFeedingDate?: string;
  feedingFrequencyDays?: number;
}

export interface UpdateAnimalDto {
  name?: string;
  breed?: string;
  morph?: string;
  weight?: number;
  lastFeedingDate?: string;
  feedingFrequencyDays?: number;
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

// Authentication types
export interface User {
  id: number;
  username: string;
  email: string;
  createdAt: string;
}

export interface LoginDto {
  username: string;
  password: string;
}

export interface RegisterDto {
  username: string;
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  user: User;
}
