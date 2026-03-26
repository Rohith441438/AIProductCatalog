# AI Product Catalog - .NET Backend

This repository contains a starter ASP.NET Core Web API backend for a product catalog where users can:

- Register/login.
- Add products (name, weight, size/dimensions, price).
- List only their own products.
- Add products via a chat-style command endpoint.

## API Endpoints

### Auth
- `POST /api/auth/register`
- `POST /api/auth/login`

### Products (JWT required)
- `POST /api/products`
- `GET /api/products`

### Chat command (JWT required)
- `POST /api/chat/commands`

Supported command format:

```text
add product name=Milk weight=1.2 size=10x10x20cm price=4.99
```

## Run

```bash
dotnet restore ProductService.Api/ProductService.Api.csproj
dotnet run --project ProductService.Api/ProductService.Api.csproj
```

## Notes

- SQLite is used (`product-service.db`).
- Database schema is auto-created at startup via `EnsureCreated()`.
- Change `Jwt:Key` in `appsettings.json` before production use.
