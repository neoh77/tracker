import axios from 'axios';
import { Animal, CreateAnimalDto, UpdateAnimalDto, WeightHistory, FeedingHistory, CreateFeedingHistoryDto } from '../types';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000';

const apiClient = axios.create({
  baseURL: `${API_BASE_URL}/api`,
  headers: {
    'Content-Type': 'application/json',
  },
});

export const animalService = {
  async getAnimals(search?: string): Promise<Animal[]> {
    const params = search ? { search } : {};
    const response = await apiClient.get('/animals', { params });
    return response.data;
  },

  async getAnimal(id: number): Promise<Animal> {
    const response = await apiClient.get(`/animals/${id}`);
    return response.data;
  },

  async createAnimal(animal: CreateAnimalDto): Promise<Animal> {
    const response = await apiClient.post('/animals', animal);
    return response.data;
  },

  async updateAnimal(id: number, animal: UpdateAnimalDto): Promise<void> {
    await apiClient.put(`/animals/${id}`, animal);
  },

  async deleteAnimal(id: number): Promise<void> {
    await apiClient.delete(`/animals/${id}`);
  },

  async getAnimalWeightHistory(id: number): Promise<WeightHistory[]> {
    const response = await apiClient.get(`/animals/${id}/weight-history`);
    return response.data;
  },

  async getAnimalFeedingHistory(id: number): Promise<FeedingHistory[]> {
    const response = await apiClient.get(`/animals/${id}/feeding-history`);
    return response.data;
  },
};

export const feedingService = {
  async createFeedingHistory(feeding: CreateFeedingHistoryDto): Promise<FeedingHistory> {
    const response = await apiClient.post('/feedinghistory', feeding);
    return response.data;
  },

  async deleteFeedingHistory(id: number): Promise<void> {
    await apiClient.delete(`/feedinghistory/${id}`);
  },
};
