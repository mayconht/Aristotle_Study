# Aristotle Study - User Management API
[![.NET Build](https://github.com/mayconht/Aristotle_Study/actions/workflows/dotnet.yml/badge.svg)](https://github.com/mayconht/Aristotle_Study/actions/workflows/dotnet.yml)

simple ASP.NET Core Web API for learning Entity Framework Core and NSwagger with SQlite.
Keep in mind that this is a simple project for educational purposes and many many improvements can be made.

### Prerequisites

- [Visual Studio Code](https://code.visualstudio.com/)
- [Dev Containers extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Running the Project

1. **Start the Dev Container**
    - VS Code will detect the `.devcontainer` configuration
    - Click "Reopen in Container" when prompted
    - Or use Command Palette (`Ctrl+Shift+P`) → "Dev Containers: Reopen in Container"

2. **Automatic Setup**
   The dev container will automatically:
    - Install .NET 9.0 SDK
    - Restore NuGet packages
    - Install extensions
    - Build the solution
    - Start the application with hot reload

### Application Access

Once the container is running:

- **API Base URL**: `http://localhost:3000`
- **Swagger Documentation**: `http://localhost:3000/swagger/index.html` or `http://localhost:3001/swagger/index.html

If the port is unavailable it might map to the next available one.

> **Important**: The application automatically attempts to open Swagger in your browser. If it doesn't open
> automatically, manually navigate to the Swagger URL above. (only happens outside the dev container or running from
> IDE)

### Hot Reload Feature

The application runs with `dotnet watch` which provides:

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

- **Database file**: `users.db` (created automatically with migrations)
- **Migrations**: Applied automatically on startup

### Development Workflow

1. Make code changes in VS Code
2. Save the file (`Ctrl+S`)
3. Watch the terminal for hot reload confirmation
4. Test changes immediately in Swagger UI
5. Repeat!

### Troubleshooting

**Port Issues**: If port 3000 is already in use, stop other services or modify the port in
`Properties/launchSettings.json`

**Container Issues**: Rebuild the dev container using Command Palette (ctrl + f1) → "Dev Containers: Rebuild Container"

**Database Issues**: Delete `users.db` file to reset the database (it will be recreated automatically)

---

Yes, I am a tester and I am still lacking the Unit tests, well, well, it is friday and I need to play some games.
At least I managed to do a parallel with Java and Spring Boot.
