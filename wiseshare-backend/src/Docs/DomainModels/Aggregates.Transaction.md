# Domain Aggregates
## Transaction

```csharp
class Transaction
{
Transaction Create();
    void UpdateTransactionStatus(string status);
    // TODO: Add additional methods for transaction management
}
```

```json
{
     "id": "00000000-0000-0000-0000-000000000000",
    "userId": "11111111-1111-1111-1111-111111111111",
    "propertyId": "22222222-2222-2222-2222-222222222222",
    "transactionType": "Purchase",
    "amount": 500.00,
    "dateTime": "2024-01-05T14:00:00.0000000Z",
    "paymentMethod": "Credit Card",
    "transactionStatus": "Completed"
}

