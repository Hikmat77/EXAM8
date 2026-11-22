# EventBooking Platform API

ASP.NET Core .NET 10 Web API for managing event venues, organizers, events, ticket types, attendees, orders, tickets, reviews, and sales reports.

## Projects

- `Domain` - entities and enums
- `Infrastructure` - `ApplicationDbContext`, EF Core configurations, migrations, and seed data
- `Application` - DTOs, mapping, interfaces, and business services
- `WebApi` - controllers, middleware, dependency injection, Swagger, and background order expiration

## Run

1. Set the PostgreSQL connection string as an environment variable. In PowerShell:
   `$env:ConnectionStrings__DefaultConnection = "Host=localhost;Database=EXAM8;Username=postgres;Password=YOUR_PASSWORD"`
2. Apply migrations in the same terminal:
   `dotnet ef database update --project Infrastructure --startup-project WebApi --context ApplicationDbContext`
3. Run in the same terminal:
   `dotnet run --project WebApi`
4. Open Swagger at `/swagger` in the Development environment.

## Migrations

- `InitialCreate`
- `AddReviewsAndProfiles`
- `AddIndexesAndConstraints`
