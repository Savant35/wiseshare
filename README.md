<a name="readme-top"></a>
<!-- PROJECT LOGO -->
<br />
<div align="center">

  <h2 align="center">WiseShare</h2>
  <p align="center">
    WiseShare is a RESTful API for fractional property investment, handling user authentication, property management, investments, portfolios, payments via Stripe, and wallets, built with ASP.NET Core, EF Core, JWT auth, and Swagger UI.
    <br />
    <a href="#preview">Preview</a>
    ·
    <a href="https://github.com/Savant35/wiseshare/issues">Report Bug</a>
    ·
    <a href="https://github.com/Savant35/wiseshare/pulls">Request Feature</a>
  </p>
  
</div>
<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
        <li><a href="#appsettings.json">.env File</a></li>
      </ul>
    </li>
    <li><a href="#preview">Preview</a></li>
    <li><a href="#license">License</a></li>
  </ol>
</details>

<!-- ABOUT THE PROJECT -->
## About The Project
WiseShare backend is a RESTful API for fractional property investment, providing endpoints to authenticate users (register, login, verify email, reset password), manage user profiles, roles, and account status, create, search, update, and delete properties, invest in properties, sell shares, and request/approve sell orders, track portfolios and investment summaries, handle deposits, withdrawals, and refunds via Stripe (including webhooks), and maintain user wallets and balances. It is built with ASP.NET Core Web API, EF Core (SQLite/SQL Server), Stripe.NET for payments, JWT Bearer tokens for authentication, and Swagger UI for interactive documentation.

<p align="right">(<a href="#readme-top">back to top</a>)</p>


<!-- GETTING STARTED -->
## Getting Started

### Prerequisites
* .Net 9.0
* stripe account
* c#

### Installation
```sh
git clone https://github.com/Savant35/wiseshare.git
cd wiseshare/wiseshare/
dotnet restore
dotnet build
dotnet run --project ./src/WiseShare.Api
```

### appsettings.json
<p>Before running, open src/WiseShare.Api/appsettings.json and update the JWT secret, database connection, and any environment-specific settings. </p>

```sh
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "JwtSettings": {
    "Secret": "",
    "ExpiryMinutes": ,
    "Issuer": "",
    "Audience": ""
  },
  "ConnectionStrings": {
    "WiseshareDatabase": "Data Source=Wiseshare.db"
  }
}
```

### appsettings.Development.json
<p>Before running, open src/WiseShare.Api/appsettings.Development.json and update the JWT secret, database connection, and any environment-specific settings. </p>

```sh
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "JwtSettings": {
    "Secret": "",
    "ExpiryMinutes": 60,
    "Issuer": "",
    "Audience": ""
  },
  "Stripe": {
    "SecretKey": "",
    "PublishableKey": "",
    "WebhookSecret": ""
  },
  "Smtp": {
    "From": "",
    "Host": "",
    "Port": "",
    "User": "",
    "Pass": ""
  },
  "Api": {
    "BaseUrl": ""
  }
}
```

<p align="right">(<a href="#readme-top">back to top</a>)</p>

## preview 
<div style="display: flex; flex-wrap: wrap; justify-content: center; gap: 10px; width:100%">
  <img src="https://github.com/user-attachments/assets/441dfc30-f7d3-4022-af7a-e1101d1e6cdc" alt="Image" style="width: 48%; height: 300px; object-fit: cover;">
  <img src="https://github.com/user-attachments/assets/9c3692e3-9802-443f-9852-fb1227235531" alt="Image" style="width: 48%; height: 300px; object-fit: cover;">
  <img src="https://github.com/user-attachments/assets/2d59c409-a849-4366-aa3a-ad48af3e64ab" alt="Image" style="width: 48%; height: 300px; object-fit: cover;">
  <img src="https://github.com/user-attachments/assets/3c95cb16-a0f8-4986-a09e-d512958992aa" alt="Image" style="width: 48%; height: 300px; object-fit: cover;">
  <img src="https://github.com/user-attachments/assets/e3e1ca20-c864-43eb-aa99-6c12674d328c" alt="Image" style="width: 48%; height: 300px; object-fit: cover;">
  <img src="https://github.com/user-attachments/assets/c0e7af82-90a4-46f3-aa08-061563f16dcc" alt="Image" style="width: 48%; height: 300px; object-fit: cover;">
</div>


<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- LICENSE -->
## License

Distributed under the MIT License.

<p align="right">(<a href="#readme-top">back to top</a>)</p>


