# Aristotle Study - User Management API

## About This Project

Aristotle Study is an ASP.NET Core Web API designed for learning Entity Framework Core and Clean Architecture principles. This project serves as a study package and is intended for educational purposes. It demonstrates best practices in API design, testing, and architecture.

## Technology Stack
- .NET 8.0
- ASP.NET Core Web API
- Entity Framework Core 8.0 (SQLite for development, PostgreSQL for Docker/production)
- Swagger/OpenAPI (API documentation)
- xUnit v3, Moq, Verify.XunitV3 (testing)
- Coverlet (code coverage)
- Docker & Docker Compose

## Architecture
The project follows Clean Architecture principles with the following layers:
- **Domain**: Entities, interfaces, domain exceptions
- **Application**: Services, business logic
- **Infrastructure**: Data access, repositories
- **Controllers**: API endpoints

## Features
- User CRUD operations (Create, Read, Update, Delete)
- API documentation via Swagger
- Unit and integration tests

## Database Switching
The project supports both SQLite and PostgreSQL. The database provider is selected automatically based on the connection string format:
- **Development**: Uses SQLite (`Data Source=users.db` in `appsettings.Development.json`)
- **Docker/Production**: Uses PostgreSQL (`Host=...` or `postgresql://...` in `appsettings.json` or via environment variables)

To switch databases, simply update the `DefaultConnection` string in your configuration file:
- For SQLite: `Data Source=users.db`
- For PostgreSQL: `Host=localhost;Port=5432;Database=user;Username=user;Password=user` or `postgresql://user:user@localhost:5432/user`

## Running Locally (Development)
1. Ensure you have .NET 8 SDK installed.
2. Run the API:
   ```sh
   dotnet run --project UserService/UserService.csproj
   ```
3. Access Swagger UI at [http://localhost:3000/swagger/index.html](http://localhost:3000/swagger/index.html)

## Running with Docker (Production)
1. Ensure Docker and Docker Compose are installed.
2. Start the services:
   ```sh
   docker-compose up --build
   ```
3. The API will use PostgreSQL and be available at [http://localhost:3000/swagger/index.html](http://localhost:3000/swagger/index.html)

## Testing
Run unit tests with:
```sh
dotnet test Tests/Aristotle.UnitTests.csproj
```

## Project Structure
- `UserService/` - Main API project
- `Tests/` - Unit and integration tests
- `ISTQB/` - Additional study resources

## Contributing
This project is for study purposes. Contributions are welcome for improvements, bug fixes, or new features.

## License
MIT License
