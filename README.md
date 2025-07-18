# Animal Tracker

A web application for tracking animals including their weight history and feeding schedules.

## Features

- **Add Animal**: Form to input animal details (name, breed, morph, weight, last feeding date)
- **View Animals**: List view of all animals with their details
- **Weight History**: Track and display weight changes over time
- **Feeding Report**: Generate a report of feeding days for each animal
- **Search Functionality**: Search for animals by name or breed
- **Responsive Design**: Mobile-friendly interface

## Tech Stack

- **Frontend**: React.js with TypeScript
- **Backend**: C# ASP.NET Core
- **Database**: PostgreSQL
- **Containerization**: Docker Compose

## Project Structure

```
tracker/
├── backend/
│   └── TrackerApi/              # ASP.NET Core Web API
│       ├── Controllers/         # API Controllers
│       ├── Data/               # Database context
│       ├── DTOs/               # Data Transfer Objects
│       ├── Models/             # Entity models
│       └── Dockerfile
├── frontend/                    # React TypeScript app
│   ├── src/
│   │   ├── components/         # React components
│   │   ├── services/           # API services
│   │   └── types/              # TypeScript types
│   ├── Dockerfile
│   └── nginx.conf
├── database/
│   └── init.sql                # Database initialization
└── docker-compose.yml
```

## Getting Started

### Prerequisites

- Docker and Docker Compose
- .NET 8.0 SDK (for local development)
- Node.js 18+ (for local development)

### Running with Docker Compose

1. Clone the repository:
```bash
git clone <repository-url>
cd tracker
```

2. Start all services:
```bash
docker-compose up -d
```

3. Access the application:
   - Frontend: http://localhost:3000
   - Backend API: http://localhost:5000
   - Database: localhost:5432

### Local Development

#### Backend Setup

1. Navigate to backend directory:
```bash
cd backend/TrackerApi
```

2. Install dependencies and run:
```bash
dotnet restore
dotnet run
```

The API will be available at https://localhost:7272 or http://localhost:5272

#### Frontend Setup

1. Navigate to frontend directory:
```bash
cd frontend
```

2. Install dependencies and run:
```bash
npm install
npm start
```

The React app will be available at http://localhost:3000

#### Database Setup

Make sure PostgreSQL is running locally with the connection string specified in appsettings.json, or use the Docker Compose setup.

## API Endpoints

### Animals
- `GET /api/animals` - Get all animals (with optional search parameter)
- `GET /api/animals/{id}` - Get specific animal
- `POST /api/animals` - Create new animal
- `PUT /api/animals/{id}` - Update animal
- `DELETE /api/animals/{id}` - Delete animal
- `GET /api/animals/{id}/weight-history` - Get animal's weight history
- `GET /api/animals/{id}/feeding-history` - Get animal's feeding history

### Feeding History
- `POST /api/feedinghistory` - Add feeding record
- `DELETE /api/feedinghistory/{id}` - Delete feeding record

## Database Schema

### Animals Table
- `id` (Serial, Primary Key)
- `name` (VARCHAR(255), NOT NULL)
- `breed` (VARCHAR(255))
- `morph` (VARCHAR(255))
- `weight` (DECIMAL(10,2))
- `last_feeding_date` (TIMESTAMP)
- `created_at` (TIMESTAMP)
- `updated_at` (TIMESTAMP)

### Weight History Table
- `id` (Serial, Primary Key)
- `animal_id` (INTEGER, Foreign Key)
- `weight` (DECIMAL(10,2), NOT NULL)
- `recorded_at` (TIMESTAMP)

### Feeding History Table
- `id` (Serial, Primary Key)
- `animal_id` (INTEGER, Foreign Key)
- `feeding_date` (TIMESTAMP, NOT NULL)
- `notes` (TEXT)
- `created_at` (TIMESTAMP)

## Features Overview

### Animal Management
- Add new animals with basic information
- Edit existing animal details
- Delete animals (cascades to history records)
- Search animals by name or breed

### Weight Tracking
- Automatic weight history when adding/updating weight
- View weight changes over time
- Visual indication of weight trends

### Feeding Management
- Record feeding dates with optional notes
- View feeding history chronologically
- Automatic update of last feeding date

### User Interface
- Clean, responsive design
- Mobile-friendly interface
- Intuitive navigation
- Real-time search functionality

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## License

This project is licensed under the MIT License.
