# Project Architecture

```
/src
  ├── Tradof.Domain  
  │   ├── Tradof.Data  
  │   │   ├── Entities
  |   |   ├──IGeneralRepository&IUOW
  │   │   |    ├── IGeneralRepository.cs  
  │   │   |    └── IUnitOfWork.cs  
  ├── Tradof.Infrastructure  
  │   ├── Tradof.EntityFramework 
  |   |   └── TradofDbContext.cs
  │   |   Tradof.Repository  
  |   |   ├── GeneralRepository.cs  
  │   |   └── UnitOfWork.cs  
  ├── Tradof.Services  
  │   └── Tradof.Module.Admin  
  │       ├── Tradof.Admin.API  
  │       └── Tradof.Admin.Services  
  ├── Tradof.Setting  
  │   ├── Tradof.Common  
  │   └── Tradof.ResponseHandler  
  └── Tradof.Web  
      └── Tradof.Api
```

# Tradof

Tradof is a **Translation and Freelancers Management Platform** designed to streamline translation projects for **Arabic companies and agencies**.  
It helps businesses manage translators, clients, projects, and payments — all in one place.

---

## Features

- Manage translation projects and client orders  
- Assign and track freelance translators  
- Manage quotes, invoices, and payments  
- Track deadlines and project progress  
- Secure authentication and role-based access  
- Communication between clients and translators  
