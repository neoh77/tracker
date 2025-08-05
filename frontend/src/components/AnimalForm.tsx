import React, { useState, useEffect } from 'react';
import { useNavigate, useParams, Link } from 'react-router-dom';
import { CreateAnimalDto, UpdateAnimalDto } from '../types';
import { animalService } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import './AnimalForm.css';

interface AnimalFormProps {
  mode: 'add' | 'edit';
}

const AnimalForm: React.FC<AnimalFormProps> = ({ mode }) => {
  const navigate = useNavigate();
  const { id } = useParams<{ id: string }>();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const { user, logout } = useAuth();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };
  
  const [formData, setFormData] = useState({
    name: '',
    breed: '',
    morph: '',
    weight: '',
    lastFeedingDate: '',
    feedingFrequencyDays: '7'
  });

  useEffect(() => {
    if (mode === 'edit' && id) {
      fetchAnimal(parseInt(id));
    }
  }, [mode, id]);

  const fetchAnimal = async (animalId: number) => {
    try {
      setLoading(true);
      const animal = await animalService.getAnimal(animalId);
      setFormData({
        name: animal.name,
        breed: animal.breed || '',
        morph: animal.morph || '',
        weight: animal.weight ? animal.weight.toString() : '',
        lastFeedingDate: animal.lastFeedingDate ? 
          new Date(animal.lastFeedingDate).toISOString().split('T')[0] : '',
        feedingFrequencyDays: animal.feedingFrequencyDays.toString()
      });
    } catch (err) {
      setError('Failed to fetch animal details');
      console.error('Error fetching animal:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!formData.name.trim()) {
      setError('Animal name is required');
      return;
    }

    try {
      setLoading(true);
      setError(null);

      const animalData = {
        name: formData.name.trim(),
        breed: formData.breed.trim() || undefined,
        morph: formData.morph.trim() || undefined,
        weight: formData.weight ? parseFloat(formData.weight) : undefined,
        lastFeedingDate: formData.lastFeedingDate ? new Date(formData.lastFeedingDate).toISOString() : undefined,
        feedingFrequencyDays: formData.feedingFrequencyDays ? parseInt(formData.feedingFrequencyDays) : 7
      };

      if (mode === 'add') {
        await animalService.createAnimal(animalData as CreateAnimalDto);
      } else if (mode === 'edit' && id) {
        await animalService.updateAnimal(parseInt(id), animalData as UpdateAnimalDto);
      }

      navigate('/');
    } catch (err) {
      setError(`Failed to ${mode} animal`);
      console.error(`Error ${mode}ing animal:`, err);
    } finally {
      setLoading(false);
    }
  };

  if (loading && mode === 'edit') {
    return <div className="loading">Loading animal details...</div>;
  }

  return (
    <div className="animal-form">
      <nav className="top-nav">
        <div className="nav-left">
          <h1>Animal Tracker</h1>
        </div>
        <div className="nav-right">
          <span className="user-info">Welcome, {user?.username}</span>
          <button onClick={handleLogout} className="logout-button">Logout</button>
        </div>
      </nav>
      
      <div className="form-header">
        <h2>{mode === 'add' ? 'Add New Animal' : 'Edit Animal'}</h2>
        <Link to="/" className="back-button">
          Back to List
        </Link>
      </div>

      {error && <div className="error">{error}</div>}

      <form onSubmit={handleSubmit} className="form">
        <div className="form-group">
          <label htmlFor="name">Name *</label>
          <input
            type="text"
            id="name"
            name="name"
            value={formData.name}
            onChange={handleChange}
            required
            placeholder="Enter animal name"
          />
        </div>

        <div className="form-group">
          <label htmlFor="breed">Breed</label>
          <input
            type="text"
            id="breed"
            name="breed"
            value={formData.breed}
            onChange={handleChange}
            placeholder="Enter breed (optional)"
          />
        </div>

        <div className="form-group">
          <label htmlFor="morph">Morph</label>
          <input
            type="text"
            id="morph"
            name="morph"
            value={formData.morph}
            onChange={handleChange}
            placeholder="Enter morph (optional)"
          />
        </div>

        <div className="form-group">
          <label htmlFor="weight">Weight (grams)</label>
          <input
            type="number"
            id="weight"
            name="weight"
            value={formData.weight}
            onChange={handleChange}
            step="0.1"
            min="0"
            placeholder="Enter weight in grams"
          />
        </div>

        <div className="form-group">
          <label htmlFor="lastFeedingDate">Last Feeding Date</label>
          <input
            type="date"
            id="lastFeedingDate"
            name="lastFeedingDate"
            value={formData.lastFeedingDate}
            onChange={handleChange}
          />
        </div>

        <div className="form-group">
          <label htmlFor="feedingFrequencyDays">Feeding Frequency (days)</label>
          <input
            type="number"
            id="feedingFrequencyDays"
            name="feedingFrequencyDays"
            value={formData.feedingFrequencyDays}
            onChange={handleChange}
            min="1"
            max="365"
            placeholder="How often to feed (in days)"
          />
        </div>

        <div className="form-actions">
          <button 
            type="button" 
            onClick={() => navigate('/')}
            className="cancel-button"
          >
            Cancel
          </button>
          <button 
            type="submit" 
            disabled={loading}
            className="submit-button"
          >
            {loading ? 'Saving...' : mode === 'add' ? 'Add Animal' : 'Update Animal'}
          </button>
        </div>
      </form>
    </div>
  );
};

export default AnimalForm;
