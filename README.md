# Insurance Wrapper API

A production-ready .NET 8 wrapper API that intelligently routes insurance quotes and binds across multiple Lines of Business (LOB) and third-party providers.

## Features

✅ **Automatic LOB Detection** - Analyzes input to determine insurance type  
✅ **Multi-Provider Support** - Integrates with Dyad, Herald, and Zywave APIs  
✅ **Clean Architecture** - Layered design with separation of concerns  
✅ **Strategy Pattern** - Flexible mapping for different LOBs and providers  
✅ **Refit Integration** - Type-safe HTTP clients for third-party APIs  
✅ **Resilience Patterns** - Retry and circuit breaker policies using Polly  
✅ **Comprehensive Logging** - Structured logging with Serilog  
✅ **Swagger Documentation** - Interactive API documentation  

## Supported Lines of Business

- **General Liability (GL)** - Commercial general liability insurance
- **Property** - Commercial property insurance
- **Flood** - Flood insurance coverage
- **Worker Compensation** - Workers' comp insurance

## Supported Providers

- **Dyad** - ACE-HUB integration
- **Herald** - Herald API integration
- **Zywave** - Zywave platform integration

## Architecture

```
┌─────────────────────────────────────────┐
│           API Layer                     │
│   Controllers, Middleware, Swagger     │
└───────────────┬─────────────────────────┘
                │
┌───────────────▼─────────────────────────┐
│       Application Layer                 │
│   Services, DTOs, Interfaces            │
└───────────────┬─────────────────────────┘
                │
┌───────────────▼─────────────────────────┐
│         Domain Layer                    │
│   Mappers, Factories, Enums             │
└───────────────┬─────────────────────────┘
                │
┌───────────────▼─────────────────────────┐
│      Infrastructure Layer               │
│   API Clients (Refit), Configurations   │
└─────────────────────────────────────────┘
```

## Getting Started

### Prerequisites

- .NET 8 SDK
- Visual Studio 2022 / VS Code / Rider

### Installation

1. Clone the repository
```bash
git clone https://github.com/yourcompany/insurance-wrapper-api.git
cd insurance-wrapper-api
```

2. Restore packages
```bash
dotnet restore
```

3. Update API keys in `appsettings.json`
```json
{
  "Providers": {
    "Dyad": {
      "BaseUrl": "https://api.dyad.com",
      "ApiKey": "your-actual-dyad-api-key"
    },
    "Herald": {
      "BaseUrl": "https://api.herald.com",
      "ApiToken": "your-actual-herald-token"
    },
    "Zywave": {
      "BaseUrl": "https://api.zywave.com",
      "ApiKey": "your-actual-zywave-key"
    }
  }
}
```

4. Run the application
```bash
cd InsuranceWrapperApi.Api
dotnet run
```

5. Access Swagger UI
```
https://localhost:5001
```

## API Usage Examples

### Example 1: General Liability Quote

**Request:**
```http
POST /api/quote
Content-Type: application/json

{
  "businessName": "ABC Construction",
  "contactName": "John Smith",
  "email": "john@abcconstruction.com",
  "phone": "555-1234",
  "businessAddress": {
    "street": "123 Main St",
    "city": "Atlanta",
    "state": "GA",
    "zipCode": "30301"
  },
  "effectiveDate": "2025-01-01",
  "glClassCode": "91343",
  "operationsDescription": "General Contracting",
  "annualRevenue": 1500000,
  "numberOfEmployees": 25,
  "yearsInBusiness": 10
}
```

**Response:**
```json
{
  "success": true,
  "quoteId": "Q-2025-001234",
  "lineOfBusiness": "GeneralLiability",
  "provider": "Zywave",
  "premium": 15000.00,
  "fees": 250.00,
  "totalCost": 15250.00,
  "expirationDate": "2025-02-01T00:00:00Z",
  "coverages": [
    {
      "coverageType": "General Aggregate",
      "limit": 2000000,
      "premium": 8000
    },
    {
      "coverageType": "Per Occurrence",
      "limit": 1000000,
      "premium": 7000
    }
  ],
  "message": "Quote successfully retrieved from Zywave"
}
```

### Example 2: Property Quote

**Request:**
```http
POST /api/quote
Content-Type: application/json

{
  "businessName": "Main Street Retail",
  "contactName": "Jane Doe",
  "email": "jane@mainstreetretail.com",
  "phone": "555-5678",
  "businessAddress": {
    "street": "456 Commerce Blvd",
    "city": "Chicago",
    "state": "IL",
    "zipCode": "60601"
  },
  "effectiveDate": "2025-02-01",
  "buildingValue": 850000,
  "contentsValue": 250000,
  "constructionType": "Masonry",
  "yearBuilt": 2010,
  "hasSprinklers": true,
  "hasAlarm": true
}
```

**Response:**
```json
{
  "success": true,
  "quoteId": "Q-2025-005678",
  "lineOfBusiness": "Property",
  "provider": "Herald",
  "premium": 4500.00,
  "fees": 150.00,
  "totalCost": 4650.00,
  "expirationDate": "2025-03-01T00:00:00Z",
  "coverages": [
    {
      "coverageType": "Building",
      "limit": 850000,
      "deductible": 2500,
      "premium": 3200
    },
    {
      "coverageType": "Business Personal Property",
      "limit": 250000,
      "deductible": 2500,
      "premium": 1300
    }
  ],
  "message": "Quote successfully retrieved from Herald"
}
```

### Example 3: Worker Compensation Quote

**Request:**
```http
POST /api/quote
Content-Type: application/json

{
  "businessName": "Tech Solutions Inc",
  "contactName": "Mike Johnson",
  "email": "mike@techsolutions.com",
  "phone": "555-9012",
  "businessAddress": {
    "street": "789 Tech Park",
    "city": "Austin",
    "state": "TX",
    "zipCode": "78701"
  },
  "effectiveDate": "2025-01-15",
  "stateOfOperation": "TX",
  "payrollByClass": [
    {
      "classCode": "8810",
      "classDescription": "Clerical Office Employees",
      "annualPayroll": 500000,
      "numberOfEmployees": 15
    },
    {
      "classCode": "8742",
      "classDescription": "Outside Sales",
      "annualPayroll": 300000,
      "numberOfEmployees": 5
    }
  ],
  "numberOfEmployees": 20,
  "yearsInBusiness": 7
}
```

**Response:**
```json
{
  "success": true,
  "quoteId": "Q-2025-009012",
  "lineOfBusiness": "WorkerCompensation",
  "provider": "Zywave",
  "premium": 6400.00,
  "fees": 100.00,
  "totalCost": 6500.00,
  "expirationDate": "2025-02-15T00:00:00Z",
  "coverages": [
    {
      "coverageType": "Workers Compensation - Statutory",
      "premium": 5000
    },
    {
      "coverageType": "Employers Liability",
      "limit": 1000000,
      "premium": 1400
    }
  ],
  "message": "Quote successfully retrieved from Zywave"
}
```

### Example 4: Bind Policy

**Request:**
```http
POST /api/bind
Content-Type: application/json

{
  "quoteId": "Q-2025-001234",
  "lineOfBusiness": "GeneralLiability",
  "provider": "Zywave",
  "payment": {
    "paymentMethod": "CreditCard",
    "amount": 15250.00,
    "cardNumber": "4111111111111111",
    "expiryDate": "12/26",
    "cvv": "123"
  }
}
```

**Response:**
```json
{
  "success": true,
  "policyNumber": "POL-2025-GL-001234",
  "quoteId": "Q-2025-001234",
  "lineOfBusiness": "GeneralLiability",
  "provider": "Zywave",
  "effectiveDate": "2025-01-01T00:00:00Z",
  "expirationDate": "2026-01-01T00:00:00Z",
  "boundPremium": 15250.00,
  "message": "Policy successfully bound with Zywave"
}
```

## Configuration

### Provider Selection Rules

You can configure default providers for each LOB in `appsettings.json`:

```json
{
  "Providers": {
    "Default": {
      "GeneralLiability": "Zywave",
      "Property": "Herald",
      "Flood": "Dyad",
      "WorkerCompensation": "Zywave"
    }
  }
}
```

### Business Rules

Customize routing logic in `ProviderSelectionService.cs`:

```csharp
// Example: Route high-revenue GL businesses to Dyad
if (request.AnnualRevenue > 5_000_000)
{
    return ProviderType.Dyad;
}
```

## Project Structure

```
InsuranceWrapperApi/
├── InsuranceWrapperApi.Api/              # Web API Layer
│   ├── Controllers/                      # API endpoints
│   ├── Program.cs                        # Application entry point
│   └── appsettings.json                  # Configuration
│
├── InsuranceWrapperApi.Application/      # Application Services
│   ├── DTOs/                            # Data Transfer Objects
│   │   ├── Common/                      # Generic request/response
│   │   ├── LOB/                         # LOB-specific DTOs
│   │   └── Providers/                   # Provider-specific DTOs
│   ├── Interfaces/                      # Service interfaces
│   └── Services/                        # Service implementations
│
├── InsuranceWrapperApi.Domain/           # Business Logic
│   ├── Enums/                           # Enumerations
│   ├── Factories/                       # Factory patterns
│   └── Mappers/                         # Mapping logic
│       ├── LOB/                         # LOB mappers
│       └── Providers/                   # Provider mappers
│
└── InsuranceWrapperApi.Infrastructure/   # External Services
    ├── ApiClients/                      # Refit clients
    └── Extensions/                      # DI configuration
```

## Error Handling

The API includes comprehensive error handling:

```json
{
  "success": false,
  "errors": [
    "Unable to determine Line of Business from provided information"
  ]
}
```

## Logging

Logs are written to:
- Console (structured JSON)
- File: `logs/insurance-wrapper-api-YYYYMMDD.txt`

## Testing

### Manual Testing with cURL

```bash
# Get a quote
curl -X POST https://localhost:5001/api/quote \
  -H "Content-Type: application/json" \
  -d @sample-gl-request.json

# Bind a policy
curl -X POST https://localhost:5001/api/bind \
  -H "Content-Type: application/json" \
  -d @sample-bind-request.json
```

## Deployment

### Docker (Optional)

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["InsuranceWrapperApi.Api/InsuranceWrapperApi.Api.csproj", "InsuranceWrapperApi.Api/"]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InsuranceWrapperApi.Api.dll"]
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

MIT License - see LICENSE file for details

## Support

For issues and questions:
- GitHub Issues: https://github.com/yourcompany/insurance-wrapper-api/issues
- Email: support@yourcompany.com
