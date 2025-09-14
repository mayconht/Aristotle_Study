# Aristotle Study - User Management API

<div align="center">

[![Build, Test and Analyze](https://github.com/mayconht/Aristotle_Study/actions/workflows/build-test-analyze.yml/badge.svg)](https://github.com/mayconht/Aristotle_Study/actions/workflows/build-test-analyze.yml)
[![API tests](https://github.com/mayconht/Aristotle_Study/actions/workflows/bruno_tests.yml/badge.svg)](https://github.com/mayconht/Aristotle_Study/actions/workflows/bruno_tests.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=mayconht_Aristotle_Study&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=mayconht_Aristotle_Study)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=mayconht_Aristotle_Study&metric=bugs)](https://sonarcloud.io/summary/new_code?id=mayconht_Aristotle_Study)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=mayconht_Aristotle_Study&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=mayconht_Aristotle_Study)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=mayconht_Aristotle_Study&metric=coverage)](https://sonarcloud.io/summary/new_code?id=mayconht_Aristotle_Study)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=mayconht_Aristotle_Study&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=mayconht_Aristotle_Study)
[![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=mayconht_Aristotle_Study&metric=ncloc)](https://sonarcloud.io/summary/new_code?id=mayconht_Aristotle_Study)
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=mayconht_Aristotle_Study&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=mayconht_Aristotle_Study)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=mayconht_Aristotle_Study&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=mayconht_Aristotle_Study)
[![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=mayconht_Aristotle_Study&metric=sqale_index)](https://sonarcloud.io/summary/new_code?id=mayconht_Aristotle_Study)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=mayconht_Aristotle_Study&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=mayconht_Aristotle_Study)
[![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=mayconht_Aristotle_Study&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=mayconht_Aristotle_Study)

</div>

# Aristotle Study - User Management API

A simple ASP.NET Core Web API for learning Entity Framework Core with Clean Architecture principles.
Keep in mind that this is a simple project for educational purposes and many improvements can be made.

## Technologies Used

- **.NET 8.0** - Target framework
- **ASP.NET Core Web API** - Web framework
- **Entity Framework Core 9.0** - ORM with SQLite and PostgreSQL providers
- **SQLite** - Lightweight database (default)
- **PostgreSQL** - Alternative database (configurable)
- **AutoMapper** - Object-to-object mapping
- **DotNetEnv** - Environment variable loader
- **Swagger/OpenAPI** - API documentation (Swashbuckle.AspNetCore)
- **xUnit v3** - Unit testing framework
- **Moq** - Mocking framework for tests
- **Bogus** - Fake data generation for tests
- **Coverlet** - Code coverage analysis
- **SonarCloud** - Code quality and security analysis
- **Docker** - Containerization

## Architecture

The project follows Clean Architecture principles with the following layers:

- **Domain** - Entities, interfaces, and domain exceptions
- **Application** - Services and application-specific logic
- **Infrastructure** - Data access, repositories, and external concerns
- **Controllers** - API endpoints and HTTP concerns

## Features

- User CRUD operations (Create, Read, Update, Delete)
- Email uniqueness validation
- Input validation
- Global exception handling
- Logging with structured logging
- Unit tests with high coverage
- API documentation with Swagger
- Docker support for local development

## Domain Entities

### User

- **Id**: Guid (Primary Key)
- **Name**: string (Required, max 100 chars)
- **Email**: string (Required, max 200 chars, unique)
- **DateOfBirth**: DateTime? (Optional)

## API Endpoints

| Method | Endpoint                   | Description       |
|--------|----------------------------|-------------------|
| GET    | `/api/users`               | Get all users     |
| GET    | `/api/users/{id}`          | Get user by ID    |
| GET    | `/api/users/email/{email}` | Get user by email |
| POST   | `/api/users`               | Create new user   |
| PUT    | `/api/users/{id}`          | Update user       |
| DELETE | `/api/users/{id}`          | Delete user       |

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [JetBrains Rider](https://www.jetbrains.com/rider/) (recommended)
  or [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (optional, for SonarQube local setup)

### Running the Project

1. **Clone and Restore**
   ```bash
   git clone <repository-url>
   cd Aristotle_Study
   dotnet restore
   ```

2. **Run the Application**
   ```bash
   cd UserService
   dotnet run
   ```

3. **Or Run with Hot Reload**
   ```bash
   cd UserService
   dotnet watch run
   ```

### Application Access

Once running:

- **API Base URL**: `http://localhost:5000` (HTTP) or `https://localhost:5001` (HTTPS)
- **Swagger Documentation**: `http://localhost:5000/swagger/index.html`

The application will automatically open Swagger UI in your default browser when running in development mode.

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run from solution root
dotnet test UserServiceTests/UserService.UnitTests.csproj
```

### SonarCloud Integration

The project is integrated with SonarCloud for code quality analysis. To configure SonarCloud properly:

1. **Access SonarCloud Settings**:
    - Go to [SonarCloud](https://sonarcloud.io/) and log in
    - Select your project `mayconht_Aristotle_Study`
    - Navigate to "Administration" > "Analysis Method"

2. **Configure Analysis Method**:
    - Disable "Automatic Analysis" if you're using CI-based analysis
    - Or vice versa (don't run both simultaneously)

3. **Required GitHub Secrets**:
    - `SONAR_TOKEN`: Your SonarCloud API token (from SonarCloud user settings)

### Local SonarQube Setup (Optional)

For local code quality analysis, you can run SonarQube using Docker Compose:

```bash
# Start SonarQube and PostgreSQL
docker-compose up -d

# Access SonarQube at http://localhost:9000
# Default credentials: admin/admin
```

### Hot Reload Feature

The application supports hot reload with `dotnet watch` which provides:

- **Automatic recompilation** when you save code changes
- **Live reload** without manual restart
- **Real-time feedback** for development

> **Tip**: You'll see hot reload messages in the terminal when files are changed and recompiled.

### Available Endpoints

The API provides user management functionality. Check the Swagger documentation for complete API details:

- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get user by ID
- `POST /api/users` - Create new user
- `PUT /api/users/{id}` - Update user
- `DELETE /api/users/{id}` - Delete user

### Database

The project uses SQLite with Entity Framework Core:

- **Database file**: `users.db` (created automatically)
- **Migrations**: Located in `Migrations/` folder
- **Context**: `ApplicationDbContext` in Infrastructure layer

To create new migrations:

```bash
cd UserService
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

### Development Workflow

1. Make code changes in your IDE
2. Save the file (`Ctrl+S`)
3. Watch the terminal for hot reload confirmation (if using `dotnet watch`)
4. Test changes immediately in Swagger UI
5. Create unit tests
6. Run tests to make sure everything works
7. Repeat!

### Project Structure

```
Aristotle_Study/
├── Aristotle_Study.sln          # Solution file
├── coverlet.runsettings         # Code coverage settings
├── Dockerfile                   # Docker configuration
├── global.json                  # .NET SDK version
├── README.md                    # This file
├── UserService/                 # Main ASP.NET Core Web API project
│   ├── appsettings.Development.json
│   ├── appsettings.json
│   ├── Program.cs
│   ├── UserService.csproj
│   ├── Application/             # Application services and logic
│   │   ├── MappingProfile.cs
│   │   ├── UserValidator.cs
│   │   ├── DTOs/
│   │   ├── Exceptions/
│   │   └── Service/
│   ├── Controllers/             # API controllers
│   │   └── UserController.cs
│   ├── Domain/                  # Domain entities and interfaces
│   │   ├── Entities/
│   │   ├── Exceptions/
│   │   └── Interfaces/
│   ├── Infrastructure/          # Data access and external concerns
│   │   ├── ApplicationDbContext.cs
│   │   ├── Data/
│   │   ├── Exceptions/
│   │   └── Middleware/
│   ├── bin/
│   └── obj/
├── UserServiceTests/            # Unit tests for UserService
│   ├── UserService.UnitTests.csproj
│   ├── Application/
│   │   ├── MappingProfileTests.cs
│   │   ├── UserValidatorTests.cs
│   │   ├── Controllers/
│   │   ├── DTOs/
│   │   └── Service/
│   ├── Builders/
│   │   └── UserBuilder.cs
│   ├── Config/
│   │   └── TestConfig.cs
│   ├── Domain/
│   │   ├── Entities/
│   │   └── Exceptions/
│   ├── Infrastructure/
│   │   ├── ApplicationDbContextTests.cs
│   │   ├── Data/
│   │   └── Middleware/
│   ├── bin/
│   └── obj/
├── Tests/                       # Additional test project
│   ├── bin/
│   └── obj/
├── ISTQB/                       # ISTQB study materials
│   ├── StartHere.md
│   └── 1_Fundamentals-Of-Testing/
│       └── 1_Fundamentals.md
├── bruno/                       # API testing with Bruno
│   └── UserService API/
│       ├── bruno.json
│       ├── collection.bru
│       ├── environments/
│       └── User API Regression Test/
└── Aristotle_Project/           # Legacy project directory
    ├── bin/
    └── obj/
```

### Testing Strategy

The project includes unit tests covering:

- Controller endpoints with various scenarios
- Service layer business logic
- Domain entity validation
- Repository data access operations
- Exception handling
- Builder pattern for test data

Test frameworks used:

- xUnit for test execution
- Moq for mocking dependencies
- Bogus for generating fake test data
- Verify for snapshot testing

### API Testing with Bruno

The project includes API testing using Bruno, a CLI tool for testing APIs. The test collection is located in the `bruno/UserService API/` directory. To execute the tests, follow these steps:

1. **Install Bruno CLI**:
   ```bash
   npm install -g @usebruno/cli
   ```

2. **Run the Tests**:
   ```bash
   bruno run bruno/UserService\ API/collection.bru
   ```

3. **View Reports**:
   - Test results will be generated in the specified output directory if configured.

Bruno is also integrated into the CI/CD pipeline to ensure API functionality during automated builds.
To understand more about this outstanding tool, visit the [Bruno Documentation](https://docs.usebruno.com/).

### Contributing

This is an educational project, but contributions are welcome! Areas for improvement include:

- Adding more domain entities
- Implementing DTOs for API layer
- Adding authentication/authorization
- Implementing CQRS pattern
- Adding integration tests
- Improving error handling

### Troubleshooting

**Port Issues**: If default ports are in use, modify the URLs in `Properties/launchSettings.json`

**Database Issues**: Delete `users.db` file to reset the database (it will be recreated automatically)

**Build Issues**: Run `dotnet clean` followed by `dotnet restore` and `dotnet build`

---

*This project serves as a learning exercise comparing C#/.NET development patterns with Java/Spring Boot, focusing on
Clean Architecture principles and modern development practices.*