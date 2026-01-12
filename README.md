# SMS Notification Service

A vendor-agnostic SMS notification service for sending order confirmation messages, designed to integrate with the existing TixTrack order flow while maintaining flexibility to swap SMS providers.

## Solution Structure

```
SmsService.sln
├── SmsService.Core/                 # Domain models and interfaces (no external dependencies)
│   ├── Models/                      # Domain entities and DTOs
│   ├── Interfaces/                  # Repository and service interfaces
│   └── Services/                    # Domain services
├── SmsService.Infrastructure/       # External services and data access (EF Core, Twilio, etc)
│   ├── Data/                        # DbContext and migrations
│   ├── Repositories/                # Repository implementations
│   ├── Services/                    # Infrastructure services
│   └── Configurations/              # EF configurations
├── SmsService.Api/                  # REST API layer
│   ├── Endpoints/                   # API endpoints
│   ├── Middleware/                  # Custom middleware
│   └── Configuration/               # Dependency injection setup
└── SmsService.Tests/                # Unit and integration tests
    ├── Unit/                        # Unit tests
    └── Integration/                 # Integration tests
```

## Getting Started

### Prerequisites

- .NET 10 SDK
- Docker and Docker Compose
- Git

### Local Development Setup

1. **Clone the repository**
   ```bash
   git clone <repo-url>
   cd nliven-sms
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run with Docker Compose** (no Azure infrastructure needed)
   ```bash
   docker-compose up
   ```
   
   The API will be available at `http://localhost:5000`

4. **Run locally without Docker**
   ```bash
   cd SmsService.Api
   dotnet run
   ```

### Health Check Endpoints

- `GET /health` - Overall health status
- `GET /health/live` - Liveness probe
- `GET /health/ready` - Readiness probe

### Running Tests

```bash
dotnet test
```

## Architecture Decisions

See [sms-architecture-design.md](sms-architecture-design.md) for detailed architecture documentation.

### Key Principles

- **Event-Driven**: Uses Azure Service Bus for decoupling
- **Vendor-Agnostic**: Port/Adapter pattern for SMS provider abstraction
- **Multi-Tenant**: Each venue gets dedicated phone number
- **Consent-First**: Check consent before publishing to service bus

## Development Workflow

1. Create feature branch from `main`
2. Make changes with tests
3. Ensure coverage >80%
4. Push to GitHub (triggers CI/CD)
5. Create Pull Request
6. Code review and merge to `main`
7. Automatic deploy to dev environment
8. Manual approval to deploy to production

## Deployment

### CI/CD Pipeline

GitHub Actions automatically:
- Builds on every commit
- Runs unit tests
- Checks coverage (must be >80%)
- Builds Docker image
- Pushes to Azure Container Registry
- Deploys to dev environment (auto)
- Requires manual approval for production

### Infrastructure

See Terraform configuration in `/terraform` directory for:
- Azure Container Apps
- Azure Container Registry
- Key Vault
- Log Analytics

## Documentation

- [High-Level Architecture](sms-architecture-high-level.md)
- [Detailed Design](sms-architecture-design.md)
- [Implementation Plan](implementation-plan-jira-tickets.md)

## Contributing

- Follow clean architecture principles
- Write tests for all new features
- Use meaningful commit messages
- Ensure code coverage stays >80%

## Support

For issues or questions, contact the development team.
