import axios from 'axios';
import { Animal, CreateAnimalDto, UpdateAnimalDto, WeightHistory, FeedingHistory, CreateFeedingHistoryDto, LoginDto, RegisterDto, AuthResponse } from '../types';

const API_BASE_URL = process.env.REACT_APP_API_URL || 'http://localhost:5000';

const apiClient = axios.create({
  baseURL: `${API_BASE_URL}/api`,
  headers: {
    'Content-Type': 'application/json',
  },
});

// Add request interceptor to include auth token
apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('authToken');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

// Add response interceptor to handle auth errors
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Clear auth token and redirect to login
      localStorage.removeItem('authToken');
      localStorage.removeItem('authUser');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export const authService = {
  async login(credentials: LoginDto): Promise<AuthResponse> {
    const response = await apiClient.post('/auth/login', credentials);
    const authData = response.data;
    
    // Store token and user data
    localStorage.setItem('authToken', authData.token);
    localStorage.setItem('authUser', JSON.stringify(authData.user));
    
    return authData;
  },

  async register(userData: RegisterDto): Promise<AuthResponse> {
    const response = await apiClient.post('/auth/register', userData);
    const authData = response.data;
    
    // Store token and user data
    localStorage.setItem('authToken', authData.token);
    localStorage.setItem('authUser', JSON.stringify(authData.user));
    
    return authData;
  },

  logout(): void {
    localStorage.removeItem('authToken');
    localStorage.removeItem('authUser');
  },

  getCurrentUser(): any {
    const userData = localStorage.getItem('authUser');
    return userData ? JSON.parse(userData) : null;
  },

  getToken(): string | null {
    return localStorage.getItem('authToken');
  },

  isAuthenticated(): boolean {
    return !!this.getToken();
  }
};

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
