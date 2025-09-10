# Automation Backend (.NET 8)

RESTful API services for:
- User authentication (JWT)
- Social account integration (Facebook, Instagram, Twitter, YouTube)
- Post scheduling and retrieval of scheduled/published content
- Analytics aggregation (summary + time series)
- Automation rules management

Run:
- dotnet run (launches at http://localhost:3001)
- Docs at http://localhost:3001/docs

Environment variables (or appsettings.json):
- MANAGEMENT_DB_CONNECTION: SQL Server connection string for management_database
- JWT_SECRET: Secret key for JWT signing
- Jwt:Issuer, Jwt:Audience (optional overrides)

API Tags:
- Auth, SocialAccounts, Scheduling, Content, Analytics, AutomationRules

Notes:
- This implementation uses EF Core with SQL Server.
- For initial development, ensure the database is reachable and migrations are applied externally (or use EnsureCreated in a setup routine if desired).
