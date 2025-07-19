import React, { useState, useEffect } from 'react';
import { Animal } from '../types';
import { animalService } from '../services/api';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import './AnimalList.css';

const AnimalList: React.FC = () => {
  const [animals, setAnimals] = useState<Animal[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

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
      <nav className="top-nav">
        <div className="nav-left">
          <h1>Animal Tracker</h1>
        </div>
        <div className="nav-right">
          <span className="user-info">Welcome, {user?.username}</span>
          <button onClick={handleLogout} className="logout-button">Logout</button>
        </div>
      </nav>
      
      <div className="header">
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

      {animals.length > 0 && (
        <div className="feeding-summary">
          <h2>Upcoming Feedings</h2>
          <div className="upcoming-feedings">
            {(() => {
              const upcomingFeedings = animals
                .map((animal) => {
                  const daysSinceLastFed = animal.lastFeedingDate 
                    ? Math.floor((new Date().getTime() - new Date(animal.lastFeedingDate).getTime()) / (1000 * 3600 * 24))
                    : null;
                  
                  const feedingFrequency = animal.feedingFrequencyDays || 7;
                  const daysOverdue = daysSinceLastFed !== null ? daysSinceLastFed - feedingFrequency : null;
                  
                  const getStatus = (daysOverdue: number | null) => {
                    if (daysSinceLastFed === null) return 'never-fed';
                    if (daysOverdue === null) return 'never-fed';
                    if (daysOverdue >= 0) return 'overdue';
                    if (daysOverdue >= -1) return 'due-today';
                    if (daysOverdue >= -2) return 'due-soon';
                    return 'recent';
                  };

                  const getPriority = (daysOverdue: number | null) => {
                    if (daysSinceLastFed === null) return 4; // Never fed - highest priority
                    if (daysOverdue === null) return 4;
                    if (daysOverdue >= 1) return 3; // Overdue
                    if (daysOverdue >= 0) return 2; // Due today
                    if (daysOverdue >= -2) return 1; // Due soon
                    return 0; // Recent, don't show
                  };

                  const isDueToday = daysOverdue !== null && daysOverdue >= 0;

                  return {
                    ...animal,
                    daysSinceLastFed,
                    daysOverdue,
                    isDueToday,
                    status: getStatus(daysOverdue),
                    priority: getPriority(daysOverdue)
                  };
                })
                .filter(animal => animal.priority >= 1) // Show animals that need feeding soon or are overdue
                .sort((a, b) => b.priority - a.priority); // Sort by priority (highest first)

              if (upcomingFeedings.length === 0) {
                return (
                  <div className="no-upcoming-feedings">
                    <p>🎉 All animals have been fed recently!</p>
                  </div>
                );
              }

              return (
                <div className="feeding-cards">
                  {upcomingFeedings.map((animal) => (
                    <div key={animal.id} className={`feeding-card priority-${animal.priority}`}>
                      <div className="feeding-card-header">
                        <h3>
                          <Link to={`/animal/${animal.id}`} className="animal-link">
                            {animal.name}
                          </Link>
                          {animal.isDueToday && <span className="feed-today-badge">FEED TODAY</span>}
                        </h3>
                        <span className={`status-badge status-${animal.status}`}>
                          {animal.daysSinceLastFed === null ? 'Never Fed' : 
                           animal.status === 'overdue' ? 'Overdue' :
                           animal.status === 'due-today' ? 'Due Today' :
                           animal.status === 'due-soon' ? 'Due Soon' : 'Recent'}
                        </span>
                      </div>
                      <div className="feeding-card-details">
                        {animal.breed && <p><strong>Breed:</strong> {animal.breed}</p>}
                        <p><strong>Feeding Schedule:</strong> Every {animal.feedingFrequencyDays} days</p>
                        <p><strong>Last Fed:</strong> {formatDate(animal.lastFeedingDate)}</p>
                        <p><strong>Days Since:</strong> {
                          animal.daysSinceLastFed !== null ? 
                          `${animal.daysSinceLastFed} days ago` : 'Never'
                        }</p>
                        {animal.daysOverdue !== null && (
                          <p><strong>Status:</strong> {
                            animal.daysOverdue >= 0 ? 
                            `${animal.daysOverdue} days overdue` : 
                            `Due in ${Math.abs(animal.daysOverdue)} days`
                          }</p>
                        )}
                      </div>
                      <div className="feeding-card-actions">
                        <Link to={`/animal/${animal.id}`} className="feed-now-button">
                          Record Feeding
                        </Link>
                      </div>
                    </div>
                  ))}
                </div>
              );
            })()}
          </div>
        </div>
      )}
    </div>
  );
};

export default AnimalList;
