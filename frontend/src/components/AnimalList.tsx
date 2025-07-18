import React, { useState, useEffect } from 'react';
import { Animal } from '../types';
import { animalService } from '../services/api';
import { Link } from 'react-router-dom';
import './AnimalList.css';

const AnimalList: React.FC = () => {
  const [animals, setAnimals] = useState<Animal[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchAnimals = async (search?: string) => {
    try {
      setLoading(true);
      const data = await animalService.getAnimals(search);
      setAnimals(data);
      setError(null);
    } catch (err) {
      setError('Failed to fetch animals');
      console.error('Error fetching animals:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchAnimals();
  }, []);

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    fetchAnimals(searchTerm || undefined);
  };

  const handleDelete = async (id: number) => {
    if (window.confirm('Are you sure you want to delete this animal?')) {
      try {
        await animalService.deleteAnimal(id);
        fetchAnimals(searchTerm || undefined);
      } catch (err) {
        setError('Failed to delete animal');
        console.error('Error deleting animal:', err);
      }
    }
  };

  const formatDate = (dateString?: string) => {
    if (!dateString) return 'Never';
    return new Date(dateString).toLocaleDateString();
  };

  if (loading) {
    return <div className="loading">Loading animals...</div>;
  }

  if (error) {
    return <div className="error">{error}</div>;
  }

  return (
    <div className="animal-list">
      <div className="header">
        <h1>Animal Tracker</h1>
        <Link to="/add" className="add-button">Add New Animal</Link>
      </div>
      
      <form onSubmit={handleSearch} className="search-form">
        <input
          type="text"
          placeholder="Search by name or breed..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          className="search-input"
        />
        <button type="submit" className="search-button">Search</button>
        {searchTerm && (
          <button 
            type="button" 
            onClick={() => {
              setSearchTerm('');
              fetchAnimals();
            }}
            className="clear-button"
          >
            Clear
          </button>
        )}
      </form>

      {animals.length === 0 ? (
        <div className="no-animals">
          No animals found. <Link to="/add">Add your first animal</Link>
        </div>
      ) : (
        <div className="animal-grid">
          {animals.map((animal) => (
            <div key={animal.id} className="animal-card">
              <div className="animal-header">
                <h3>{animal.name}</h3>
                <div className="animal-actions">
                  <Link to={`/animal/${animal.id}`} className="view-button">View</Link>
                  <Link to={`/edit/${animal.id}`} className="edit-button">Edit</Link>
                  <button 
                    onClick={() => handleDelete(animal.id)}
                    className="delete-button"
                  >
                    Delete
                  </button>
                </div>
              </div>
              <div className="animal-details">
                {animal.breed && <p><strong>Breed:</strong> {animal.breed}</p>}
                {animal.morph && <p><strong>Morph:</strong> {animal.morph}</p>}
                {animal.weight && <p><strong>Weight:</strong> {animal.weight}g</p>}
                <p><strong>Last Fed:</strong> {formatDate(animal.lastFeedingDate)}</p>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default AnimalList;
