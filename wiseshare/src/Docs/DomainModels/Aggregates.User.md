# Domain Aggregates

## user

``` csharp
class User{
    User Create();
}
```
```json 
{
   "id": "00000000-0000-0000-0000-000000000000",
    "firstName": "Ali",
    "lastName": "Arthur",
    "email": "user@gmail.com",
    "phone": "814-999-9999",
    "password": "password123", // TODO: Hash this
    "createdDateTime": "2020-01-01T00:00:00.0000000Z",
    "updatedDateTime": "2020-01-01T00:00:00.0000000Z"
}