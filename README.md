Personal Development API

This is an ASP.NET Core Web API project with a PostgreSQL database for managing personal development data.

The API provides features for:

User registration & login (with JWT authentication)
Notes (CRUD per user)
Habits (CRUD per user)
Habit logs (tracking daily habit completion)

Project Structure:

PersonalDevelopment/
│
├── Api/                     # ASP.NET Core Web API controllers
│   ├── Controllers/
│   │   ├── AuthController.cs        # Handles registration & login
│   │   ├── NotesController.cs       # Notes CRUD
│   │   ├── HabitsController.cs      # Habits CRUD
│   │   └── HabitLogsController.cs   # Habit logs CRUD
│   └── Middleware/
│       └── ErrorHandlingMiddleware.cs
│
├── Application/             # Business logic (services, interfaces, helpers)
├── Domain/                  # Entities & repository interfaces
├── Infrastructure/          # Database and repository implementations
├── Database/
│   └── database.sql           # PostgreSQL schema (tables, constraints)
│
├── PersonalDevelopment.sln  # Visual Studio solution
├── PersonalDevelopment.csproj
├── appsettings.json         # Configuration (logging, connection string placeholder)
└── PersonalDevelopmentTests/ # Unit tests for controllers

Technical Features:

This project is built following Clean Architecture principles and includes:
Authentication & Authorization
Logging (login, CRUD actions, unauthorized attempts)
Error Handling
Database Access

Getting Started:

1. Clone the repository
git clone https://github.com/rinanu/PersonalDevelopment.git
cd PersonalDevelopment

2. Setup PostgreSQL

Make sure PostgreSQL is installed and running.

Create a database:

createdb PersonalDevelopment

Run the schema script:

psql -U postgres -d PersonalDevelopment -f Database/database.sql

3. Configure the connection string

Edit appsettings.json:

{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=PersonalDevelopment;Username=postgres;Password=yourpassword"
  },
    "Jwt": {
    "Key": "insertyourkeyhere",
    "Issuer": "PersonalDevelopment",
    "Audience": "PersonalDevelopmentAppUsers"
  }
}

4. Run the API

In the project root:

dotnet run --project PersonalDevelopment

How the API Works:

Authentication flow:
Register
POST /api/auth/register
{
  "username": "testuser",
  "email": "test@mail.com",
  "password": "password123"
}
Response:
{ "UserId": 1 }

Login
POST /api/auth/login
{
  "username": "testuser",
  "password": "password123"
}
Response:
{ "Token": "eyJhbGciOi..." }

Authorize in Swagger:
Click the Authorize button in Swagger UI.

Enter:
Bearer <your-token-here>

Now all secured endpoints will recognize you as the logged-in user.

Notes API:

GET /api/notes → get your notes
POST /api/notes → create a note
PUT /api/notes/{id} → update a note
DELETE /api/notes/{id} → delete a note

Habits API:

GET /api/habits → get your habits
POST /api/habits → create a habit
PUT /api/habits/{id} → update a habit
DELETE /api/habits/{id} → delete a habit

Habit Logs API:

GET /api/habitlogs/{habitId} → get logs for a habit
POST /api/habitlogs → create a log
PUT /api/habitlogs → update a log
DELETE /api/habitlogs/{habitId}/{date} → delete a log

Running Tests:

From the project root:

dotnet test

This runs all unit tests.