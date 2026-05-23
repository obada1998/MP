# Multi-Tenant E-Commerce Platform MVP

Production-oriented MVP for a SaaS-style multi-tenant commerce platform using ASP.NET Core Web API, EF Core, SQL Server, Clean Architecture, and Blazor.

## Solution Structure

```text
src/
  Platform.Domain          Domain entities, enums, tenant markers
  Platform.Application     DTOs, service contracts, page layout models, validators
  Platform.Infrastructure  EF Core, Identity, SQL Server, services, seed data, migrations
  Platform.Api             REST API, JWT auth, controllers, error handling
  Platform.Blazor          Bootstrap Blazor dashboards, page builder, storefront renderer
tests/
  Platform.Tests           Unit tests for tenant isolation, products, layouts, renderer catalog
```

## Main Capabilities

- Store-level multi-tenancy with `StoreId` on catalog, pages, orders, customers, settings, and layouts.
- ASP.NET Core Identity users with roles: `PlatformAdmin`, `StoreOwner`, `StoreStaff`, `Customer`.
- Tenant guard service checks store membership before management operations.
- Metadata-driven products using `ProductFieldDefinition` and `ProductCustomFieldValue`.
- JSON page layouts stored in `PageLayout.LayoutJson`.
- Dynamic Blazor renderer maps section types to components through a registry.
- Blazor owner dashboard for products, categories, pages, page builder, orders, and settings.
- Public storefront pages, product listing/details, cart, checkout placeholder, and order creation.
- Initial EF Core SQL Server migration and seed data.

## Prerequisites

- .NET SDK 10.0+
- SQL Server or SQL Server LocalDB
- Browser that trusts the local ASP.NET Core dev certificate

Trust the dev certificate if needed:

```powershell
dotnet dev-certs https --trust
```

## Database

Default connection string:

```json
"Server=(localdb)\\MSSQLLocalDB;Database=MultiTenantECommerce;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
```

Change it in `src/Platform.Api/appsettings.json` if you use a full SQL Server instance.
If LocalDB is not installed or cannot create an automatic instance, point `DefaultConnection` at a running SQL Server instance before starting the API.

Create or update the database:

```powershell
dotnet tool restore
dotnet tool run dotnet-ef database update --project src\Platform.Infrastructure --startup-project src\Platform.Api
```

The API also runs migrations and seed data automatically in Development when `SeedData:Enabled` is `true`.

## Seed Accounts

All seeded users use:

```text
Password123!
```

Accounts:

```text
admin@platform.local
owner@demo.local
customer@demo.local
```

Seed data includes a `demo-store` tenant, product metadata fields, one active product, categories, and a published homepage layout.

## Run

Start the API:

```powershell
dotnet run --project src\Platform.Api
```

API URLs:

```text
https://localhost:7044
http://localhost:5162
```

The Blazor app is configured to call `http://localhost:5162` by default for local development.

Start Blazor:

```powershell
dotnet run --project src\Platform.Blazor
```

Blazor URLs:

```text
https://localhost:7045
http://localhost:5192
```

Useful paths:

```text
/login
/admin
/owner
/store/demo-store
/store/demo-store/products
/store/demo-store/cart
```

## Test

```powershell
dotnet test MultiTenantECommerce.slnx
```

Current tests cover:

- Cross-store access rejection.
- Product creation with store-defined custom fields.
- Layout JSON validation and unsafe custom HTML rejection.
- Page component catalog resolution.

## API Overview

Authentication:

```text
POST /api/auth/register
POST /api/auth/login
```

Management:

```text
GET    /api/stores/mine
POST   /api/stores
GET    /api/stores/{storeId}
PUT    /api/stores/{storeId}
GET    /api/stores/{storeId}/settings
PUT    /api/stores/{storeId}/settings
GET    /api/stores/{storeId}/theme
PUT    /api/stores/{storeId}/theme
GET    /api/stores/{storeId}/products
POST   /api/stores/{storeId}/products
GET    /api/stores/{storeId}/products/field-definitions
GET    /api/stores/{storeId}/categories
GET    /api/stores/{storeId}/pages
POST   /api/stores/{storeId}/pages/{pageId}/layout
GET    /api/stores/{storeId}/orders
PATCH  /api/stores/{storeId}/orders/{orderId}/status
```

Public storefront:

```text
GET  /api/storefront/{storeKey}/home
GET  /api/storefront/{storeKey}/pages/{slug}
GET  /api/storefront/{storeKey}/catalog
GET  /api/storefront/{storeKey}/products/{slug}
POST /api/storefront/{storeKey}/checkout
```

## Page Layout JSON

Supported section types:

```text
hero
text
image
productGrid
categoryGrid
button
banner
featuredProducts
customHtml
```

Example:

```json
{
  "pageId": "home",
  "sections": [
    {
      "type": "hero",
      "order": 1,
      "props": {
        "title": "Welcome to our store",
        "subtitle": "Best products online",
        "imageUrl": "https://placehold.co/1600x700",
        "buttonText": "Shop Now",
        "buttonUrl": "/products"
      },
      "styles": {}
    },
    {
      "type": "productGrid",
      "order": 2,
      "props": {
        "columns": 4,
        "limit": 8
      },
      "styles": {}
    }
  ]
}
```

## Security Notes

- Management services require authenticated user id and store membership.
- Public storefront reads only active stores and published pages/products.
- Layout JSON is validated before persistence.
- Custom HTML blocks reject script tags, inline click/error handlers, and `javascript:` URLs.
- Payment is intentionally excluded; order creation is prepared for a future payment provider boundary.
