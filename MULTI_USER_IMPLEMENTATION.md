# Multi-User Authentication Implementation

This document outlines the changes made to implement multi-user authentication in the Animal Tracker application.

## Backend Changes

### 1. Authentication Infrastructure
- **JWT Authentication**: Added JWT Bearer token authentication using `Microsoft.AspNetCore.Authentication.JwtBearer`
- **Password Hashing**: Implemented secure password hashing using SHA256
- **Token Service**: Created service for generating and validating JWT tokens
- **Auth Controller**: Added endpoints for user registration and login

### 2. Database Schema Changes
- **Users Table**: Added `users` table with fields: id, username, email, password_hash, created_at, updated_at
- **Animal Ownership**: Added `user_id` foreign key to `animals` table to establish ownership
- **Indexes**: Added appropriate database indexes for performance
- **Sample Data**: Created demo users for testing (username: `demo_user`, password: `password`)

### 3. API Security
- **Protected Endpoints**: All animal-related endpoints now require authentication
- **User Isolation**: Users can only access their own animals
- **Authorization Attributes**: Added `[Authorize]` attributes to controllers
- **User Context**: Controllers extract user ID from JWT claims to filter data

### 4. Database Models
- **User Model**: New entity with username, email, and password fields
- **Updated Animal Model**: Added foreign key relationship to User
- **Navigation Properties**: Configured EF Core relationships between Users and Animals

## Frontend Changes

### 1. Authentication Context
- **AuthProvider**: React context for managing authentication state
- **Token Management**: Automatic token storage and retrieval from localStorage
- **Auth Service**: Centralized authentication API calls

### 2. Authentication Components
- **Login Component**: User login form with demo account information
- **Register Component**: User registration form with validation
- **Protected Routes**: Route wrapper that redirects unauthenticated users to login

### 3. UI/UX Improvements
- **Navigation Bar**: Added user info and logout functionality to all pages
- **Automatic Token Handling**: Axios interceptors for adding auth headers
- **Error Handling**: Automatic redirect to login on 401 responses

### 4. API Integration
- **Updated API Service**: All API calls now include JWT tokens
- **Auth Endpoints**: Added login and register API integration
- **Type Definitions**: Added TypeScript types for authentication

## Security Features

### 1. Backend Security
- **JWT Tokens**: Secure token-based authentication with configurable expiration
- **Password Hashing**: One-way SHA256 password hashing
- **User Isolation**: Database-level isolation ensuring users only see their data
- **CORS Configuration**: Properly configured CORS for frontend integration

### 2. Frontend Security
- **Token Storage**: Secure token storage in localStorage
- **Route Protection**: Client-side route guards
- **Automatic Logout**: Token expiration handling
- **Error Boundaries**: Graceful error handling for auth failures

## Deployment Configuration

### 1. Database Migration
- Updated `init.sql` to include users table and user relationships
- Sample users created automatically on database initialization
- Foreign key constraints ensure data integrity

### 2. Environment Configuration
- JWT configuration in `appsettings.json`
- Database connection strings updated for multi-user support
- Docker Compose configuration remains compatible

## API Endpoints

### Authentication Endpoints
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration

### Protected Animal Endpoints
- `GET /api/animals` - Get user's animals (requires auth)
- `POST /api/animals` - Create animal for authenticated user
- `PUT /api/animals/{id}` - Update user's animal
- `DELETE /api/animals/{id}` - Delete user's animal
- `GET /api/animals/{id}/weight-history` - Get animal's weight history
- `GET /api/animals/{id}/feeding-history` - Get animal's feeding history

### Protected Feeding Endpoints  
- `POST /api/feedinghistory` - Add feeding record for user's animal
- `DELETE /api/feedinghistory/{id}` - Delete user's feeding record

## Testing

### Demo Account
- **Username**: `demo_user`
- **Password**: `password`
- **Purpose**: Pre-populated with sample animals for testing

### Registration
- Users can create new accounts with unique usernames and emails
- Password requirements: minimum 6 characters
- Email validation included

## Migration from Single-User

For existing single-user installations:
1. All existing animals will need to be associated with a user
2. A migration script would be needed to assign existing animals to a default user
3. Database schema updates are handled by the updated `init.sql`

## Benefits

1. **Data Isolation**: Each user has their own private set of animals
2. **Scalability**: Application can support multiple users without conflicts
3. **Security**: Proper authentication and authorization
4. **User Experience**: Personalized experience with user-specific data
5. **Future Features**: Foundation for advanced features like sharing, collaboration

This implementation provides a complete multi-user authentication system that maintains the existing functionality while adding secure user management.