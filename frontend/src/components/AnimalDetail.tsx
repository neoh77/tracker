import React, { useState, useEffect } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { Animal, WeightHistory, FeedingHistory, CreateFeedingHistoryDto } from '../types';
import { animalService, feedingService } from '../services/api';
import './AnimalDetail.css';

const AnimalDetail: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [animal, setAnimal] = useState<Animal | null>(null);
  const [weightHistory, setWeightHistory] = useState<WeightHistory[]>([]);
  const [feedingHistory, setFeedingHistory] = useState<FeedingHistory[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showFeedingForm, setShowFeedingForm] = useState(false);
  const [feedingFormData, setFeedingFormData] = useState({
    feedingDate: new Date().toISOString().split('T')[0],
    notes: ''
  });

  useEffect(() => {
    if (id) {
      fetchAnimalDetails(parseInt(id));
    }
  }, [id]);

  const fetchAnimalDetails = async (animalId: number) => {
    try {
      setLoading(true);
      const [animalData, weightData, feedingData] = await Promise.all([
        animalService.getAnimal(animalId),
        animalService.getAnimalWeightHistory(animalId),
        animalService.getAnimalFeedingHistory(animalId)
      ]);
      
      setAnimal(animalData);
      setWeightHistory(weightData);
      setFeedingHistory(feedingData);
      setError(null);
    } catch (err) {
      setError('Failed to fetch animal details');
      console.error('Error fetching animal details:', err);
    } finally {
      setLoading(false);
    }
  };

  const handleFeedingSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!animal) return;

    try {
      const feedingData: CreateFeedingHistoryDto = {
        animalId: animal.id,
        feedingDate: new Date(feedingFormData.feedingDate).toISOString(),
        notes: feedingFormData.notes.trim() || undefined
      };

      await feedingService.createFeedingHistory(feedingData);
      await fetchAnimalDetails(animal.id);
      setShowFeedingForm(false);
      setFeedingFormData({
        feedingDate: new Date().toISOString().split('T')[0],
        notes: ''
      });
    } catch (err) {
      setError('Failed to add feeding record');
      console.error('Error adding feeding record:', err);
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString();
  };

  if (loading) {
    return <div className="loading">Loading animal details...</div>;
  }

  if (error) {
    return <div className="error">{error}</div>;
  }

  if (!animal) {
    return <div className="error">Animal not found</div>;
  }

  return (
    <div className="animal-detail">
      <div className="detail-header">
        <div className="header-left">
          <h1>{animal.name}</h1>
          <button 
            onClick={() => navigate('/')}
            className="back-button"
          >
            ← Back to List
          </button>
        </div>
        <div className="header-actions">
          <Link to={`/edit/${animal.id}`} className="edit-button">
            Edit Animal
          </Link>
          <button 
            onClick={() => setShowFeedingForm(true)}
            className="feed-button"
          >
            Record Feeding
          </button>
        </div>
      </div>

      <div className="animal-info">
        <div className="info-card">
          <h3>Basic Information</h3>
          <div className="info-grid">
            <div><strong>Name:</strong> {animal.name}</div>
            {animal.breed && <div><strong>Breed:</strong> {animal.breed}</div>}
            {animal.morph && <div><strong>Morph:</strong> {animal.morph}</div>}
            {animal.weight && <div><strong>Current Weight:</strong> {animal.weight}g</div>}
            <div><strong>Last Fed:</strong> {animal.lastFeedingDate ? formatDate(animal.lastFeedingDate) : 'Never'}</div>
            <div><strong>Added:</strong> {formatDate(animal.createdAt)}</div>
          </div>
        </div>
      </div>

      {showFeedingForm && (
        <div className="modal-overlay">
          <div className="modal">
            <h3>Record Feeding</h3>
            <form onSubmit={handleFeedingSubmit}>
              <div className="form-group">
                <label htmlFor="feedingDate">Feeding Date:</label>
                <input
                  type="date"
                  id="feedingDate"
                  value={feedingFormData.feedingDate}
                  onChange={(e) => setFeedingFormData(prev => ({
                    ...prev,
                    feedingDate: e.target.value
                  }))}
                  required
                />
              </div>
              <div className="form-group">
                <label htmlFor="notes">Notes (optional):</label>
                <input
                  type="text"
                  id="notes"
                  placeholder="Any notes about the feeding..."
                  value={feedingFormData.notes}
                  onChange={(e) => setFeedingFormData(prev => ({
                    ...prev,
                    notes: e.target.value
                  }))}
                />
              </div>
              <div className="modal-actions">
                <button 
                  type="button" 
                  onClick={() => setShowFeedingForm(false)}
                  className="cancel-button"
                >
                  Cancel
                </button>
                <button type="submit" className="submit-button">
                  Add Feeding Record
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      <div className="history-section">
        <div className="history-card">
          <h3>Weight History</h3>
          {weightHistory.length === 0 ? (
            <p className="no-data">No weight records available</p>
          ) : (
            <div className="history-list">
              {weightHistory.map((record) => (
                <div key={record.id} className="history-item">
                  <span className="date">{formatDate(record.recordedAt)}</span>
                  <span className="value">{record.weight}g</span>
                </div>
              ))}
            </div>
          )}
        </div>

        <div className="history-card">
          <h3>Feeding History</h3>
          {feedingHistory.length === 0 ? (
            <p className="no-data">No feeding records available</p>
          ) : (
            <div className="history-list">
              {feedingHistory.map((record) => (
                <div key={record.id} className="history-item">
                  <div className="feeding-item">
                    <span className="date">{formatDate(record.feedingDate)}</span>
                    {record.notes && <span className="notes">{record.notes}</span>}
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default AnimalDetail;
