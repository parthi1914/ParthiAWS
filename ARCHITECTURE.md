# Insurance Wrapper API - Architecture & Design

## Overview
A .NET 8 wrapper API that accepts insurance quote/bind requests, automatically determines the Line of Business (LOB), maps data appropriately, and routes to third-party providers (Dyad, Herald, Zywave).

## Architecture Pattern
**Clean Architecture with Strategy & Factory Patterns**

```
┌─────────────────────────────────────────────────────────────┐
│                    API Layer (Controllers)                   │
│                  - QuoteController                           │
│                  - BindController                            │
└───────────────────────┬─────────────────────────────────────┘
                        │
┌───────────────────────▼─────────────────────────────────────┐
│                  Application Layer                           │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  LOB Detector Service                                 │  │
│  │  - Analyzes input to determine LOB type              │  │
│  └──────────────────────────────────────────────────────┘  │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Quote Service (Orchestrator)                        │  │
│  │  - Coordinates the quote workflow                    │  │
│  └──────────────────────────────────────────────────────┘  │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Bind Service (Orchestrator)                         │  │
│  │  - Coordinates the bind workflow                     │  │
│  └──────────────────────────────────────────────────────┘  │
└───────────────────────┬─────────────────────────────────────┘
                        │
┌───────────────────────▼─────────────────────────────────────┐
│                   Domain Layer                               │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  LOB Strategy Pattern                                 │  │
│  │  - ILobMapper (interface)                            │  │
│  │  - GLMapper                                          │  │
│  │  - PropertyMapper                                    │  │
│  │  - FloodMapper                                       │  │
│  │  - WorkerCompMapper                                  │  │
│  └──────────────────────────────────────────────────────┘  │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Provider Strategy Pattern                            │  │
│  │  - IProviderMapper (interface)                       │  │
│  │  - DyadMapper                                        │  │
│  │  - HeraldMapper                                      │  │
│  │  - ZywaveMapper                                      │  │
│  └──────────────────────────────────────────────────────┘  │
└───────────────────────┬─────────────────────────────────────┘
                        │
┌───────────────────────▼─────────────────────────────────────┐
│              Infrastructure Layer                            │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Refit API Clients                                    │  │
│  │  - IDyadApiClient                                    │  │
│  │  - IHeraldApiClient                                  │  │
│  │  - IZywaveApiClient                                  │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

## Request Flow

### Quote Flow
```
User Request (Basic Details)
    ↓
API Controller receives GenericQuoteRequest
    ↓
LOB Detector analyzes input
    ↓
Returns LOB Type (GL/Property/Flood/WorkerComp)
    ↓
LOB-Specific Mapper (Strategy Pattern)
    ↓
LOB DTO Created
    ↓
Provider Selection (via config/business rules)
    ↓
Provider Mapper (Strategy Pattern)
    ↓
Provider-Specific DTO Created
    ↓
Refit Client makes API call
    ↓
Response mapped back
    ↓
Return to user
```

## Key Design Patterns

### 1. Strategy Pattern (LOB Mapping)
Different mapping strategies for each line of business:
- `GLMapper` - General Liability specific mapping
- `PropertyMapper` - Property insurance mapping
- `FloodMapper` - Flood insurance mapping
- `WorkerCompMapper` - Worker Compensation mapping

### 2. Strategy Pattern (Provider Mapping)
Different mapping strategies for each provider:
- `DyadMapper` - Maps to Dyad API structure
- `HeraldMapper` - Maps to Herald API structure
- `ZywaveMapper` - Maps to Zywave API structure

### 3. Factory Pattern
- `LobMapperFactory` - Creates appropriate LOB mapper
- `ProviderMapperFactory` - Creates appropriate provider mapper

### 4. Dependency Injection
All services registered in DI container for testability and maintainability

## Project Structure
```
InsuranceWrapperApi/
├── InsuranceWrapperApi.Api/              # Web API Layer
│   ├── Controllers/
│   ├── Middleware/
│   └── Program.cs
├── InsuranceWrapperApi.Application/      # Application Services
│   ├── Services/
│   ├── Interfaces/
│   └── DTOs/
│       ├── Common/
│       ├── LOB/
│       │   ├── GL/
│       │   ├── Property/
│       │   ├── Flood/
│       │   └── WorkerComp/
│       └── Providers/
│           ├── Dyad/
│           ├── Herald/
│           └── Zywave/
├── InsuranceWrapperApi.Domain/           # Business Logic
│   ├── Enums/
│   ├── Mappers/
│   │   ├── LOB/
│   │   └── Providers/
│   └── Factories/
└── InsuranceWrapperApi.Infrastructure/   # External Services
    ├── ApiClients/
    ├── Configurations/
    └── Extensions/
```

## Technology Stack
- **.NET 8** - Framework
- **Refit** - Type-safe HTTP client
- **FluentValidation** - Request validation
- **AutoMapper** - Object mapping (supplementary)
- **Serilog** - Logging
- **Polly** - Resilience (retry, circuit breaker)

## LOB Detection Logic
The system analyzes input fields to determine LOB:

```csharp
- Has ClassCode + PayrollInfo → Worker Comp
- Has BuildingValue/Contents → Property
- Has FloodZone/ElevationCertificate → Flood  
- Has GLClassCode/OperationsDescription → GL
```

## Provider Selection
Configurable via:
1. Configuration file (appsettings.json)
2. Business rules (premium amount, LOB type, state)
3. Manual selection via request parameter

## Security Considerations
- API key authentication for third-party providers
- Rate limiting
- Request/response logging
- Data encryption in transit
- PII handling compliance

## Error Handling Strategy
- Global exception middleware
- Typed result objects (Success/Failure)
- Detailed error logging
- User-friendly error messages
- Third-party API failure fallback
