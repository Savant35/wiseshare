# Domain Aggregates
## Wallet

```csharp 
class Wallet
{
   Wallet Create();
   void UpdateBalance(decimal amount);
}
``` 
```json 
{
    "walletId": "00000000-0000-0000-0000-000000000000",
    "userId": "11111111-1111-1111-1111-111111111111",
    "balance": 5000.00,
    "createdDateTime": "2024-01-01T00:00:00.0000000Z",
    "updatedDateTime": "2024-01-10T12:00:00.0000000Z"

}