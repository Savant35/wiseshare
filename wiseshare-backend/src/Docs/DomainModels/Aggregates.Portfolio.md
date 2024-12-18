# Domain Aggregates
## Portfolio

```csharp
class Portfolio 
{
   Portfolio Create();
    void UpdateInvestmentAmount(decimal amount);
    // TODO: Add additional methods for portfolio managemen
}
```

```json 
{
    "id": "00000000-0000-0000-0000-000000000000",
    "userId": "11111111-1111-1111-1111-111111111111",
    "totalInvestmentAmount": 10000.00,
    "totalReturns": 1200.00,
    "createdDateTime": "2024-01-01T00:00:00.0000000Z",
    "updatedDateTime": "2024-01-10T12:00:00.0000000Z"
}