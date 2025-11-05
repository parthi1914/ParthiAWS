# Sample API Requests

## 1. General Liability Quote

### Small Business
```json
{
  "businessName": "Smith Plumbing Services",
  "contactName": "Robert Smith",
  "email": "robert@smithplumbing.com",
  "phone": "555-1111",
  "businessAddress": {
    "street": "100 Main Street",
    "city": "Atlanta",
    "state": "GA",
    "zipCode": "30301"
  },
  "effectiveDate": "2025-01-01T00:00:00Z",
  "glClassCode": "91340",
  "operationsDescription": "Residential plumbing services",
  "annualRevenue": 500000,
  "numberOfEmployees": 8,
  "yearsInBusiness": 12
}
```

### Large Construction Company
```json
{
  "businessName": "Mega Construction Corp",
  "contactName": "Jennifer Lee",
  "email": "jennifer@megaconstruction.com",
  "phone": "555-2222",
  "businessAddress": {
    "street": "500 Industrial Blvd",
    "city": "Houston",
    "state": "TX",
    "zipCode": "77001"
  },
  "effectiveDate": "2025-02-01T00:00:00Z",
  "glClassCode": "91343",
  "operationsDescription": "Commercial construction and contracting",
  "annualRevenue": 12000000,
  "numberOfEmployees": 150,
  "yearsInBusiness": 25
}
```

## 2. Property Quote

### Retail Store
```json
{
  "businessName": "Downtown Boutique",
  "contactName": "Sarah Johnson",
  "email": "sarah@downtownboutique.com",
  "phone": "555-3333",
  "businessAddress": {
    "street": "250 Fashion Avenue",
    "city": "New York",
    "state": "NY",
    "zipCode": "10001"
  },
  "effectiveDate": "2025-01-15T00:00:00Z",
  "buildingValue": 1200000,
  "contentsValue": 400000,
  "constructionType": "Masonry",
  "yearBuilt": 2015,
  "hasSprinklers": true,
  "hasAlarm": true,
  "industryType": "Retail - Clothing"
}
```

### Office Building
```json
{
  "businessName": "Tech Hub Properties",
  "contactName": "Michael Chen",
  "email": "michael@techhub.com",
  "phone": "555-4444",
  "businessAddress": {
    "street": "1000 Silicon Drive",
    "city": "San Francisco",
    "state": "CA",
    "zipCode": "94105"
  },
  "effectiveDate": "2025-03-01T00:00:00Z",
  "buildingValue": 5000000,
  "contentsValue": 1500000,
  "constructionType": "Steel Frame",
  "yearBuilt": 2020,
  "hasSprinklers": true,
  "hasAlarm": true,
  "industryType": "Office Building"
}
```

## 3. Flood Quote

### Coastal Property
```json
{
  "businessName": "Beachside Restaurant",
  "contactName": "David Martinez",
  "email": "david@beachsiderest.com",
  "phone": "555-5555",
  "businessAddress": {
    "street": "50 Ocean View Drive",
    "city": "Miami",
    "state": "FL",
    "zipCode": "33139"
  },
  "effectiveDate": "2025-01-01T00:00:00Z",
  "buildingValue": 800000,
  "contentsValue": 200000,
  "floodZone": "AE",
  "hasElevationCertificate": true,
  "baseFloodElevation": 12.5,
  "buildingOccupancyType": "Restaurant",
  "yearBuilt": 2010
}
```

### Low-Risk Property
```json
{
  "businessName": "Mountain View Lodge",
  "contactName": "Emily White",
  "email": "emily@mountainviewlodge.com",
  "phone": "555-6666",
  "businessAddress": {
    "street": "100 Summit Road",
    "city": "Denver",
    "state": "CO",
    "zipCode": "80202"
  },
  "effectiveDate": "2025-02-15T00:00:00Z",
  "buildingValue": 1500000,
  "contentsValue": 500000,
  "floodZone": "X",
  "hasElevationCertificate": false,
  "buildingOccupancyType": "Hotel",
  "yearBuilt": 2018
}
```

## 4. Worker Compensation Quote

### Small Office
```json
{
  "businessName": "Bright Marketing Agency",
  "contactName": "Lisa Brown",
  "email": "lisa@brightmarketing.com",
  "phone": "555-7777",
  "businessAddress": {
    "street": "300 Creative Lane",
    "city": "Portland",
    "state": "OR",
    "zipCode": "97201"
  },
  "effectiveDate": "2025-01-01T00:00:00Z",
  "stateOfOperation": "OR",
  "numberOfEmployees": 12,
  "yearsInBusiness": 5,
  "payrollByClass": [
    {
      "classCode": "8810",
      "classDescription": "Clerical Office Employees",
      "annualPayroll": 600000,
      "numberOfEmployees": 12
    }
  ]
}
```

### Manufacturing Company
```json
{
  "businessName": "Precision Manufacturing Inc",
  "contactName": "Thomas Anderson",
  "email": "thomas@precisionmfg.com",
  "phone": "555-8888",
  "businessAddress": {
    "street": "1500 Factory Road",
    "city": "Detroit",
    "state": "MI",
    "zipCode": "48201"
  },
  "effectiveDate": "2025-02-01T00:00:00Z",
  "stateOfOperation": "MI",
  "numberOfEmployees": 75,
  "yearsInBusiness": 20,
  "payrollByClass": [
    {
      "classCode": "3632",
      "classDescription": "Machine Shop",
      "annualPayroll": 2500000,
      "numberOfEmployees": 50
    },
    {
      "classCode": "8810",
      "classDescription": "Clerical Office Employees",
      "annualPayroll": 800000,
      "numberOfEmployees": 20
    },
    {
      "classCode": "8742",
      "classDescription": "Outside Sales",
      "annualPayroll": 300000,
      "numberOfEmployees": 5
    }
  ]
}
```

## 5. Bind Request Examples

### Bind GL Quote
```json
{
  "quoteId": "Q-2025-001234",
  "lineOfBusiness": "GeneralLiability",
  "provider": "Zywave",
  "payment": {
    "paymentMethod": "CreditCard",
    "amount": 5250.00,
    "cardNumber": "4111111111111111",
    "expiryDate": "12/26",
    "cvv": "123"
  }
}
```

### Bind Property Quote
```json
{
  "quoteId": "Q-2025-005678",
  "lineOfBusiness": "Property",
  "provider": "Herald",
  "payment": {
    "paymentMethod": "ACH",
    "amount": 4650.00,
    "accountNumber": "123456789",
    "routingNumber": "021000021"
  }
}
```

## Testing Tips

### Using cURL

**Quote Request:**
```bash
curl -X POST https://localhost:5001/api/quote \
  -H "Content-Type: application/json" \
  -d '{
    "businessName": "Test Company",
    "contactName": "John Doe",
    "email": "john@test.com",
    "phone": "555-0100",
    "businessAddress": {
      "street": "123 Test St",
      "city": "Atlanta",
      "state": "GA",
      "zipCode": "30301"
    },
    "effectiveDate": "2025-01-01",
    "glClassCode": "91343",
    "operationsDescription": "General Contracting",
    "annualRevenue": 1000000,
    "numberOfEmployees": 15
  }'
```

**Bind Request:**
```bash
curl -X POST https://localhost:5001/api/bind \
  -H "Content-Type: application/json" \
  -d '{
    "quoteId": "Q-2025-001234",
    "lineOfBusiness": "GeneralLiability",
    "provider": "Zywave",
    "payment": {
      "paymentMethod": "CreditCard",
      "amount": 5250.00
    }
  }'
```

### Using PowerShell

```powershell
$body = @{
    businessName = "Test Company"
    contactName = "John Doe"
    email = "john@test.com"
    phone = "555-0100"
    businessAddress = @{
        street = "123 Test St"
        city = "Atlanta"
        state = "GA"
        zipCode = "30301"
    }
    effectiveDate = "2025-01-01"
    glClassCode = "91343"
    operationsDescription = "General Contracting"
    annualRevenue = 1000000
    numberOfEmployees = 15
} | ConvertTo-Json

Invoke-RestMethod -Uri "https://localhost:5001/api/quote" -Method Post -Body $body -ContentType "application/json"
```
