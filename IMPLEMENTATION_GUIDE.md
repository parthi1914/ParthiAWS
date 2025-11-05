# Implementation Guide

## Overview

This guide explains how to customize and extend the Insurance Wrapper API for your specific needs.

## Table of Contents

1. [Adding a New Provider](#adding-a-new-provider)
2. [Adding a New Line of Business](#adding-a-new-line-of-business)
3. [Customizing Business Rules](#customizing-business-rules)
4. [Adding Validation](#adding-validation)
5. [Error Handling](#error-handling)
6. [Performance Optimization](#performance-optimization)
7. [Security Considerations](#security-considerations)

---

## Adding a New Provider

Let's say you want to add a new provider called "InsureTech".

### Step 1: Create Provider DTOs

Create `InsuranceWrapperApi.Application/DTOs/Providers/InsureTech/InsureTechDtos.cs`:

```csharp
namespace InsuranceWrapperApi.Application.DTOs.Providers.InsureTech;

public class InsureTechQuoteRequest
{
    public string ClientName { get; set; } = string.Empty;
    public DateTime PolicyStart { get; set; }
    public string ProductType { get; set; } = string.Empty;
    // Add other required fields
}

public class InsureTechQuoteResponse
{
    public bool Success { get; set; }
    public string QuoteReference { get; set; } = string.Empty;
    public decimal TotalPremium { get; set; }
    // Add other response fields
}
```

### Step 2: Create Refit Client Interface

In `InsuranceWrapperApi.Infrastructure/ApiClients/IProviderApiClients.cs`, add:

```csharp
public interface IInsureTechApiClient
{
    [Post("/v1/quotes")]
    Task<InsureTechQuoteResponse> GetQuoteAsync(
        [Body] InsureTechQuoteRequest request, 
        CancellationToken cancellationToken = default);

    [Post("/v1/policies")]
    Task<InsureTechBindResponse> BindPolicyAsync(
        [Body] InsureTechBindRequest request, 
        CancellationToken cancellationToken = default);
}
```

### Step 3: Create Provider Mapper

Create `InsuranceWrapperApi.Domain/Mappers/Providers/InsureTechMapper.cs`:

```csharp
public class InsureTechQuoteMapper : IInsureTechQuoteMapper
{
    public LineOfBusiness SupportedLob { get; set; }

    public InsureTechQuoteRequest MapFromLobDto(object lobDto, LineOfBusiness lob)
    {
        // Implement mapping logic
        switch (lob)
        {
            case LineOfBusiness.GeneralLiability:
                var glDto = (GLQuoteRequest)lobDto;
                return new InsureTechQuoteRequest
                {
                    ClientName = glDto.BusinessName,
                    PolicyStart = glDto.EffectiveDate,
                    ProductType = "GL"
                };
            // Add other LOB cases
        }
    }

    public GenericQuoteResponse MapToGenericResponse(
        InsureTechQuoteResponse response, 
        LineOfBusiness lob)
    {
        return new GenericQuoteResponse
        {
            Success = response.Success,
            QuoteId = response.QuoteReference,
            TotalCost = response.TotalPremium,
            Provider = ProviderType.InsureTech,
            LineOfBusiness = lob
        };
    }
}
```

### Step 4: Register in DI Container

In `InsuranceWrapperApi.Infrastructure/Extensions/DependencyInjection.cs`:

```csharp
// Register mapper
services.AddScoped<IInsureTechQuoteMapper, InsureTechQuoteMapper>();

// Register Refit client
services.AddRefitClient<IInsureTechApiClient>()
    .ConfigureHttpClient(c =>
    {
        c.BaseAddress = new Uri(configuration["Providers:InsureTech:BaseUrl"]);
        c.DefaultRequestHeaders.Add("X-API-Key", 
            configuration["Providers:InsureTech:ApiKey"]);
    });
```

### Step 5: Update Provider Enum

In `InsuranceWrapperApi.Domain/Enums/ProviderType.cs`:

```csharp
public enum ProviderType
{
    Unknown = 0,
    Dyad = 1,
    Herald = 2,
    Zywave = 3,
    InsureTech = 4  // Add new provider
}
```

### Step 6: Update Configuration

In `appsettings.json`:

```json
{
  "Providers": {
    "InsureTech": {
      "BaseUrl": "https://api.insuretech.com",
      "ApiKey": "your-api-key-here",
      "Timeout": 30
    }
  }
}
```

---

## Adding a New Line of Business

Let's add "Cyber Liability" as a new LOB.

### Step 1: Update LOB Enum

```csharp
public enum LineOfBusiness
{
    Unknown = 0,
    GeneralLiability = 1,
    Property = 2,
    Flood = 3,
    WorkerCompensation = 4,
    CyberLiability = 5  // New LOB
}
```

### Step 2: Create LOB-Specific DTOs

Create `InsuranceWrapperApi.Application/DTOs/LOB/Cyber/CyberDtos.cs`:

```csharp
public class CyberQuoteRequest
{
    public string BusinessName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    // Standard fields
    
    // Cyber-specific fields
    public decimal AnnualRevenue { get; set; }
    public int NumberOfRecords { get; set; }
    public bool HasEncryption { get; set; }
    public bool HasMultiFactorAuth { get; set; }
    public string IndustryType { get; set; } = string.Empty;
    public decimal RequestedLimit { get; set; }
}
```

### Step 3: Create LOB Mapper

```csharp
public class CyberMapper : ICyberMapper
{
    public CyberQuoteRequest MapFromGeneric(GenericQuoteRequest request)
    {
        return new CyberQuoteRequest
        {
            BusinessName = request.BusinessName ?? string.Empty,
            ContactName = request.ContactName ?? string.Empty,
            AnnualRevenue = request.AnnualRevenue ?? 0,
            // Map other fields from AdditionalData
            NumberOfRecords = GetFromAdditionalData<int>(
                request.AdditionalData, "NumberOfRecords"),
            HasEncryption = GetFromAdditionalData<bool>(
                request.AdditionalData, "HasEncryption")
        };
    }
}
```

### Step 4: Update LOB Detection

In `LobDetectorService.cs`:

```csharp
private bool HasCyberIndicators(GenericQuoteRequest request)
{
    return request.AdditionalData?.ContainsKey("NumberOfRecords") == true ||
           request.AdditionalData?.ContainsKey("HasEncryption") == true;
}

public LineOfBusiness DetectLineOfBusiness(GenericQuoteRequest request)
{
    if (HasCyberIndicators(request))
        return LineOfBusiness.CyberLiability;
    
    // Existing logic
}
```

---

## Customizing Business Rules

Modify `ProviderSelectionService.cs` to implement custom routing logic:

### Example: State-Based Routing

```csharp
private ProviderType SelectGLProvider(GenericQuoteRequest request)
{
    // Route California businesses to specific provider
    if (request.BusinessAddress?.State == "CA")
    {
        return ProviderType.Herald;
    }
    
    // Route high-risk industries to specialized provider
    var highRiskIndustries = new[] { "Construction", "Roofing", "Demolition" };
    if (highRiskIndustries.Any(i => 
        request.IndustryType?.Contains(i, StringComparison.OrdinalIgnoreCase) == true))
    {
        return ProviderType.Dyad;
    }
    
    // Default provider
    return ProviderType.Zywave;
}
```

### Example: Time-Based Routing

```csharp
private ProviderType SelectProvider(LineOfBusiness lob, GenericQuoteRequest request)
{
    // Route to different provider based on time of day for load balancing
    var hour = DateTime.Now.Hour;
    
    if (hour >= 9 && hour < 17) // Business hours
    {
        return ProviderType.Dyad; // Primary provider
    }
    else
    {
        return ProviderType.Herald; // After-hours provider
    }
}
```

---

## Adding Validation

Use FluentValidation for request validation.

### Step 1: Install Package

```bash
dotnet add package FluentValidation.AspNetCore
```

### Step 2: Create Validator

```csharp
public class GenericQuoteRequestValidator : AbstractValidator<GenericQuoteRequest>
{
    public GenericQuoteRequestValidator()
    {
        RuleFor(x => x.BusinessName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.EffectiveDate)
            .GreaterThanOrEqualTo(DateTime.Today)
            .WithMessage("Effective date must be today or in the future");

        RuleFor(x => x.AnnualRevenue)
            .GreaterThan(0)
            .When(x => x.AnnualRevenue.HasValue);
    }
}
```

### Step 3: Register in Program.cs

```csharp
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<GenericQuoteRequestValidator>();
```

---

## Error Handling

### Global Exception Handler

Create `Middleware/GlobalExceptionHandler.cs`:

```csharp
public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        RequestDelegate next, 
        ILogger<GlobalExceptionHandler> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Provider API request failed");
            await HandleExceptionAsync(context, ex, 
                StatusCodes.Status502BadGateway);
        }
        catch (TimeoutException ex)
        {
            _logger.LogError(ex, "Provider API timeout");
            await HandleExceptionAsync(context, ex, 
                StatusCodes.Status504GatewayTimeout);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex, 
                StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context, 
        Exception exception, 
        int statusCode)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            success = false,
            errors = new[] { exception.Message }
        };

        await context.Response.WriteAsJsonAsync(response);
    }
}
```

Register in `Program.cs`:

```csharp
app.UseMiddleware<GlobalExceptionHandler>();
```

---

## Performance Optimization

### 1. Response Caching

```csharp
// In Program.cs
builder.Services.AddResponseCaching();
app.UseResponseCaching();

// In Controller
[HttpGet("{quoteId}")]
[ResponseCache(Duration = 300)] // 5 minutes
public async Task<ActionResult<GenericQuoteResponse>> GetQuote(string quoteId)
{
    // Implementation
}
```

### 2. Memory Caching

```csharp
// Register
builder.Services.AddMemoryCache();

// Use in service
private readonly IMemoryCache _cache;

public async Task<GenericQuoteResponse> GetCachedQuote(string quoteId)
{
    return await _cache.GetOrCreateAsync(quoteId, async entry =>
    {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
        return await FetchQuoteFromProvider(quoteId);
    });
}
```

### 3. Async All The Way

Ensure all I/O operations are async:

```csharp
// Good
public async Task<Result> ProcessAsync()
{
    var data = await _repository.GetDataAsync();
    var result = await _apiClient.SendAsync(data);
    return result;
}

// Bad - blocking
public Result Process()
{
    var data = _repository.GetDataAsync().Result; // Don't do this
    return _apiClient.SendAsync(data).Result; // Don't do this
}
```

---

## Security Considerations

### 1. API Key Management

Use Azure Key Vault or AWS Secrets Manager:

```csharp
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential());
```

### 2. Rate Limiting

```bash
dotnet add package AspNetCoreRateLimit
```

```csharp
// Configure
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(
    builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// Use
app.UseIpRateLimiting();
```

### 3. Input Sanitization

```csharp
public class InputSanitizer
{
    public static string Sanitize(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        // Remove potentially dangerous characters
        return Regex.Replace(input, @"[<>'"";]", string.Empty);
    }
}
```

### 4. HTTPS Only

```csharp
app.UseHttpsRedirection();
app.UseHsts(); // In production
```

---

## Testing

### Unit Test Example

```csharp
public class LobDetectorServiceTests
{
    [Fact]
    public void DetectLineOfBusiness_WithGLIndicators_ReturnsGL()
    {
        // Arrange
        var service = new LobDetectorService();
        var request = new GenericQuoteRequest
        {
            GLClassCode = "91343",
            OperationsDescription = "General Contracting"
        };

        // Act
        var result = service.DetectLineOfBusiness(request);

        // Assert
        Assert.Equal(LineOfBusiness.GeneralLiability, result);
    }
}
```

### Integration Test Example

```csharp
public class QuoteControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public QuoteControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostQuote_WithValidRequest_ReturnsSuccess()
    {
        // Arrange
        var request = new GenericQuoteRequest
        {
            BusinessName = "Test Company",
            // Other required fields
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/quote", request);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<GenericQuoteResponse>();
        Assert.True(result.Success);
    }
}
```

---

## Monitoring and Observability

### Application Insights

```csharp
builder.Services.AddApplicationInsightsTelemetry();
```

### Custom Metrics

```csharp
private readonly TelemetryClient _telemetry;

public async Task<GenericQuoteResponse> ProcessQuoteAsync(GenericQuoteRequest request)
{
    var sw = Stopwatch.StartNew();
    try
    {
        var result = await _quoteService.ProcessQuoteAsync(request);
        
        _telemetry.TrackMetric("QuoteProcessingTime", sw.ElapsedMilliseconds);
        _telemetry.TrackEvent("QuoteSuccess", new Dictionary<string, string>
        {
            { "LOB", result.LineOfBusiness.ToString() },
            { "Provider", result.Provider.ToString() }
        });
        
        return result;
    }
    catch (Exception ex)
    {
        _telemetry.TrackException(ex);
        throw;
    }
}
```

---

## Deployment

### Docker Compose Example

```yaml
version: '3.8'
services:
  api:
    build: .
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - Providers__Dyad__ApiKey=${DYAD_API_KEY}
      - Providers__Herald__ApiToken=${HERALD_API_TOKEN}
      - Providers__Zywave__ApiKey=${ZYWAVE_API_KEY}
```

### Kubernetes Deployment

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: insurance-wrapper-api
spec:
  replicas: 3
  template:
    spec:
      containers:
      - name: api
        image: insurance-wrapper-api:latest
        ports:
        - containerPort: 80
        env:
        - name: ASPNETCORE_ENVIRONMENT
          value: "Production"
```

---

## Best Practices

1. **Always use async/await** for I/O operations
2. **Implement proper logging** at all layers
3. **Use strongly-typed configuration**
4. **Implement health checks** for dependencies
5. **Use cancellation tokens** for long-running operations
6. **Implement circuit breakers** for external API calls
7. **Cache frequently accessed data**
8. **Monitor performance metrics**
9. **Keep secrets out of source control**
10. **Write comprehensive tests**

---

## Support

For additional help or questions, please refer to:
- [README.md](README.md) - General documentation
- [QUICKSTART.md](QUICKSTART.md) - Quick setup guide
- [SAMPLE_REQUESTS.md](SAMPLE_REQUESTS.md) - Example API requests
