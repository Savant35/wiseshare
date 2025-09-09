# Domain Aggregates
## Property

```csharp 
class property 
{
    Property Create();
    Void updatePropertyValue();
    Void UpdateSharePrice();
    Void UpdateAvailableShares();
}
``` 
```json 
{
    "id": "00000000-0000-0000-0000-000000000000",
    "propertyAddress": "123 Main St, Springfield, IL",
    "location": "Springfield, IL",
    "PropertyValue": "$2500000.00",
    "sharePrice": "propertyValue/#ofshares",
    "availableShares": "20000",
    "createdDateTime": "2024-01-01T00:00:00.0000000Z",
    "propertyDescriptions": "A cozy single-family home in a quiet  neighborhood with a large backyard."



}