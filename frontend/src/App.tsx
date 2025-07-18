import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import AnimalList from './components/AnimalList';
import AnimalForm from './components/AnimalForm';
import AnimalDetail from './components/AnimalDetail';
import './App.css';

function App() {
  return (
    <Router>
      <div className="App">
        <Routes>
          <Route path="/" element={<AnimalList />} />
          <Route path="/add" element={<AnimalForm mode="add" />} />
          <Route path="/edit/:id" element={<AnimalForm mode="edit" />} />
          <Route path="/animal/:id" element={<AnimalDetail />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;
