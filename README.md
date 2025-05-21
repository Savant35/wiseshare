
<a name="readme-top"></a>
<!-- PROJECT LOGO -->
<br />
<div align="center">

  <h3 align="center">WiseShare</h3>

  <p align="center">
    A full-stack property investment platform with secure payments and admin management.
    <br />
    <a href="#demo">View Demo</a>
    ·
    <a href="https://github.com/Savant35/wiseshare/issues">Report Bug</a>
    ·
    <a href="https://github.com/Savant35/wiseshare/pulls">Request Feature</a>
  </p>
</div>

---

## Table of Contents

<details>
  <summary>Jump to</summary>
  <ol>
    <li><a href="#about-the-project">About The Project</a></li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
        <li><a href="#configuration">Configuration</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#demo">Demo</a></li>
    <li><a href="#license">License</a></li>
  </ol>
</details>

---

## About The Project

WiseShare backend is a RESTful API for fractional property investment.  
It provides endpoints to:


 **Authenticate** users (register/login/verify email/reset password)  
 **Manage** user profiles, roles, and account status  
 **Create**, **search**, **update**, and **delete** properties  
 **Invest** in properties, **sell** shares, and **request**/approve sell orders  
 **Track** portfolios and investment summaries  
 **Handle** deposits, withdrawals, and refunds via Stripe, with webhooks  
 **Maintain** user wallets and balances  

Built with:
 ASP.NET Core Web API  
 EF Core (SQLite / SQL Server)  
 Stripe.NET for payments  
 JWT Bearer tokens for auth  
 Swagger UI for interactive docs  


WiseShare Admin Dashboard is a modern web interface for managing a fractional property investment platform.
It allows admins to:

 Built with:
 Next.js (App Router)
 Tailwind CSS for styling
 TypeScript for type safety
 Lucide React for icons
 Firebase or API integration for dynamic data
 Responsive, theme-aware, and component-based design

<p align="right">(<a href="#readme-top">back to top</a>)</p>

---

## Getting Started

### Prerequisites

 [.NET 7 SDK](https://dotnet.microsoft.com/download)  
 (Optional) Docker & Docker Compose  
 Node.js 18+
 pnpm or npm
 A Stripe account (for payments)  

### Installation

1. **Clone the repo**  
   ```bash
   git clone https://github.com/Savant35/wiseshare.git 
   cd WiseShare-Backend
Run:
backend: dotnet run
frontend:  npm run dev


