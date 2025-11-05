# Quick Start Guide

## 5-Minute Setup

### 1. Prerequisites Check
```bash
dotnet --version  # Should be 8.0 or higher
```

### 2. Clone and Build
```bash
cd InsuranceWrapperApi
dotnet restore
dotnet build
```

### 3. Configure API Keys

Edit `InsuranceWrapperApi.Api/appsettings.json`:

```json
{
  "Providers": {
    "Dyad": {
      "BaseUrl": "https://api.dyad.com",
      "ApiKey": "YOUR_DYAD_KEY_HERE"
    },
    "Herald": {
      "BaseUrl": "https://api.herald.com",
      "ApiToken": "YOUR_HERALD_TOKEN_HERE"
    },
    "Zywave": {
      "BaseUrl": "https://api.zywave.com",
      "ApiKey": "YOUR_ZYWAVE_KEY_HERE"
    }
  }
}
```

### 4. Run the API
```bash
cd InsuranceWrapperApi.Api
dotnet run
```

### 5. Test with Swagger

Open browser: `https://localhost:5001`

## Sample Test Request

Copy this into Swagger UI or Postman:

### General Liability Quote Request

```json
{
  "businessName": "Test Company LLC",
  "contactName": "John Doe",
  "email": "john@testcompany.com",
  "phone": "555-0100",
  "businessAddress": {
    "street": "123 Test Street",
    "city": "Atlanta",
    "state": "GA",
    "zipCode": "30301"
  },
  "effectiveDate": "2025-01-01T00:00:00Z",
  "glClassCode": "91343",
  "operationsDescription": "General Contracting",
  "annualRevenue": 1000000,
  "numberOfEmployees": 15,
  "yearsInBusiness": 5
}
```

**Expected Response:**
- Success: true
- LineOfBusiness: GeneralLiability
- Provider: (determined by business rules)
- Premium: (calculated by provider)
- QuoteId: (unique identifier)

## How It Works

1. **User sends generic request** → System receives basic business info
2. **LOB Detection** → Analyzes fields to determine insurance type
3. **LOB Mapping** → Converts to GL/Property/Flood/WC specific DTO
4. **Provider Selection** → Chooses Dyad/Herald/Zywave based on rules
5. **Provider Mapping** → Converts to provider-specific format
6. **API Call** → Refit client calls third-party API
7. **Response Mapping** → Converts back to generic response
8. **Return to User** → Standardized response format

## Common Issues

### Issue: "Unable to determine Line of Business"
**Solution:** Ensure you include LOB-specific fields:
- GL: Include `glClassCode` or `operationsDescription`
- Property: Include `buildingValue` or `contentsValue`
- Flood: Include `floodZone`
- Worker Comp: Include `payrollByClass` array

### Issue: Provider API timeout
**Solution:** Check your API keys and provider base URLs in appsettings.json

### Issue: Build errors
**Solution:** 
```bash
dotnet clean
dotnet restore
dotnet build
```

## Next Steps

1. Review the [Architecture Documentation](ARCHITECTURE.md)
2. Customize business rules in `ProviderSelectionService.cs`
3. Add validation rules using FluentValidation (optional)
4. Implement unit tests
5. Set up CI/CD pipeline

## Development Tips

### Hot Reload
```bash
dotnet watch run
```

### Check Logs
```bash
tail -f logs/insurance-wrapper-api-*.txt
```

### Debug Mode
Set breakpoints in Visual Studio or VS Code

## Production Checklist

- [ ] Update all API keys
- [ ] Configure SSL certificates
- [ ] Set up monitoring (Application Insights, etc.)
- [ ] Enable rate limiting
- [ ] Configure CORS for specific origins
- [ ] Set up health checks
- [ ] Configure proper logging levels
- [ ] Add authentication/authorization
- [ ] Set up database for quote storage (optional)
- [ ] Configure backup strategy

## Support

Need help? Check the [README.md](README.md) for detailed documentation or contact support.
