# LimitlessFit Server

LimitlessFit-Server is the backend API for the LimitlessFit web application, developed with C# and .NET Core 9. It provides secure authentication, notifications, localization, real-time communication, user management, and order management using MySQL as the database.

## Features

- **Authentication**: Secure login and registration system with token-based authentication (JWT).
- **Real-Time Notifications**: Push real-time notifications to clients for updates and alerts.
- **Localization Support**: Multi-language backend support for localized content.
- **Websockets**: Real-time communication using websockets for instant updates.
- **User Management**: Admin-controlled user role management and account management.
- **Order Management**: API endpoints for managing fitness-related orders and transactions.

## Technologies Used

- **C#** and **.NET Core 9** for the backend framework
- **MySQL** for database storage
- **JWT** (JSON Web Tokens) for authentication
- **SignalR** for real-time communication (or other Websockets libraries)
- **Entity Framework Core** for database management and ORM

## Installation

### Prerequisites

- .NET Core 9 SDK
- MySQL Server
- MySQL Workbench or another MySQL client (optional for database management)
- Visual Studio or another C# IDE

### Clone the Repository

```bash
git clone https://github.com/lJason9/LimitlessFit-Server.git
cd LimitlessFit-Server
```

### Install Dependencies

Make sure you have the required dependencies in your `csproj` file. You can install them using NuGet:

```bash
dotnet restore
```

### Environment Variables

Create a `.env` file or set environment variables for configuration:

```bash
DB_PASSWORD=your-mysql-password-string
JWT_KEY=your-jwt-secret
```
### Running the Application

To start the server in development mode:

```bash
dotnet run
```

To run the server with hot reload:

```bash
dotnet watch run
```

### Building for Production

To build the application for production:

```bash
dotnet publish -c Release -o ./publish
```

## Contributing

If you'd like to contribute

1. Fork the repository
2. Create a new branch for your feature or bug fix
3. Implement changes and write tests
4. Submit a pull request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
