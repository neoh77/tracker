-- Create the animals table
CREATE TABLE IF NOT EXISTS animals (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    breed VARCHAR(255),
    morph VARCHAR(255),
    weight DECIMAL(10, 2),
    last_feeding_date TIMESTAMP,
    feeding_frequency_days INTEGER DEFAULT 7,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Create weight history table for tracking weight changes
CREATE TABLE IF NOT EXISTS weight_history (
    id SERIAL PRIMARY KEY,
    animal_id INTEGER REFERENCES animals(id) ON DELETE CASCADE,
    weight DECIMAL(10, 2) NOT NULL,
    recorded_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Create feeding history table for tracking feeding days
CREATE TABLE IF NOT EXISTS feeding_history (
    id SERIAL PRIMARY KEY,
    animal_id INTEGER REFERENCES animals(id) ON DELETE CASCADE,
    feeding_date TIMESTAMP NOT NULL,
    notes TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Create indexes for better performance
CREATE INDEX IF NOT EXISTS idx_animals_name ON animals(name);
CREATE INDEX IF NOT EXISTS idx_animals_breed ON animals(breed);
CREATE INDEX IF NOT EXISTS idx_weight_history_animal_id ON weight_history(animal_id);
CREATE INDEX IF NOT EXISTS idx_feeding_history_animal_id ON feeding_history(animal_id);
CREATE INDEX IF NOT EXISTS idx_feeding_history_date ON feeding_history(feeding_date);

-- Insert sample data
INSERT INTO animals (name, breed, morph, weight, last_feeding_date, feeding_frequency_days) VALUES
    ('Bella', 'Ball Python', 'Normal', 1500.00, '2025-07-15 10:00:00', 14),
    ('Rex', 'Corn Snake', 'Anerythristic', 350.50, '2025-07-16 14:30:00', 7),
    ('Luna', 'Ball Python', 'Pastel', 1200.75, '2025-07-17 09:15:00', 10)
ON CONFLICT DO NOTHING;
