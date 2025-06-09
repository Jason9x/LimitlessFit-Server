# LimitlessFit Server

LimitlessFit-Server is the C# and .NET Core 9 implementation of the LimitlessFit web application server-side API with secure authentication, notifications, localization, real-time communication, user management, and order management using MySQL as a database.

## Features

- **Authentication**: Token-based secure login and signup system with JWT.
- **Real-Time Notifications**: Push real-time notification to clients for update and alerts.
- **Localization Support**: Multi-language backend support for localized content.
- **Websockets**: Live updates through websockets for immediate updates.
- **User Management**: Admin-controlled user role handling and account handling.
- **Order Management**: API routes for order and transaction handling in relation to fitness.

## Used Technologies

- **.NET Core 9** and **C#** for the backend
- **MySQL** for database
- **JWT** (JSON Web Tokens) for authentication
- **SignalR** for real-time functionality
- **Entity Framework Core** for database management and ORM

## Installation

### Prerequisites

- .NET Core 9 SDK
- MySQL Server
- MySQL Workbench or other MySQL client (optional for db management)
- Visual Studio or other C# IDE

### Clone the Repository

```bash
git clone https://github.com/lJason9/LimitlessFit-Server.git
cd LimitlessFit-Server
```

### Install Dependencies

Make sure you have the needed dependencies in your `csproj` file. You may install them using NuGet:

```bash
dotnet restore
```

### Environment Variables

Add a `.env` file or use environment variables to configure:

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

### Building for Production

To build the application for production:

```bash
dotnet publish -c Release -o ./publish
```

## License

This project is released under the MIT License - see the [LICENSE](LICENSE) file for more details.
