# Domain Aggregates
## Investment

```csharp
class Investment 
{
    Investment Create();
    void UpdateInvestmentStatus(string status);
    // TODO: Add additional methods for investment managemen
}
```

```json
{
    "investmentId": "00000000-0000-0000-0000-000000000000",
    "userId": "11111111-1111-1111-1111-111111111111",
    "propertyId": "22222222-2222-2222-2222-222222222222",
    "numberOfSharesPurchased": 10,
    "investmentAmount": 1000.00,
    "investmentDateTime": "2024-01-05T08:30:00.0000000Z",
    "investmentStatus": "Active",
    "dividendEarned": 50.00
}