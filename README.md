# TuneBugged

TuneBugged is a full-stack music album rating application built as a hands-on learning project covering full-stack development, containerization, authentication, and database design at scale. Album, artist, and format data is sourced from real-world [Discogs](https://www.discogs.com/) XML data dumps.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core Web API (.NET) |
| Frontend | React + TanStack Query |
| Database | PostgreSQL |
| Data Source | Discogs XML data dumps (parsed via streaming `XmlReader` + `GZipStream`) |
| Infrastructure | Docker Compose (frontend, API, and database all containerized) |
| Auth | Custom JWT authentication (built from scratch, not ASP.NET Core Identity) |

---

## Project Goals

This project exists primarily as a learning vehicle. Areas of active focus include:

- Full stack application development end to end
- Docker
- Low-level and system design
- PostgreSQL performance and optimization
- .NET / ASP.NET Core internals
- Networking fundamentals
- Authentication & authorization implemented from first principles
- Software architecture and design principles

Where a higher-level abstraction exists (e.g. ASP.NET Core Identity, third-party auth providers), this project frequently opts to build the equivalent functionality manually for the learning value, with tradeoffs documented as they're made.

---

## Features

### Implemented

- **Custom JWT authentication**
  - Registration with `PasswordHasher<T>` for secure password hashing
  - Login issuing a signed JWT delivered via `httpOnly` cookie
  - `/auth/me` endpoint for frontend session rehydration on page load
  - Server-side logout (cookie deletion)
  - Refresh token rotation (rotate-by-deletion — no revocation flag needed)
- **Album ratings**
  - Upsert-based rating endpoint (`PUT`) — a user has exactly one rating per album
  - Composite primary key on `(UserId, MediaId)`
- **Search**
  - Keyset-paginated artist search with infinite scroll
  - GIN index on artist name for fast partial-text search
  - Materialized view + click-based rank score to improve result relevance
- **Global exception handling**
  - Centralized middleware mapping custom exception types to appropriate HTTP status codes
- **Discogs data pipeline**
  - Streaming XML parser for multi-gigabyte compressed data dumps (no full in-memory load)
  - Format/release-type backfill via CSV export + batched PostgreSQL import

### Planned

See [Roadmap](#roadmap) below.

---

## Getting Started
 
### Prerequisites
 
- [Docker](https://www.docker.com/) and Docker Compose
- .NET SDK (for running EF Core CLI commands)
### Setup
 
1. Clone the repository
```bash
   git clone <repo-url>
   cd MediaBoard
```
 
2. Generate a JWT signing key. In PowerShell:
```powershell
   [Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Maximum 256 }))
```
   Copy the output — you'll use it as `JWT_KEY` below.
 
3. Create a `.env` file in the project root with the required secrets:
```env
   POSTGRES_USER=mediadb_dev
   POSTGRES_PASSWORD=your-password-here
   POSTGRES_DB=mediadb
   JWT_KEY=your-generated-signing-key
 
   CONNECTION_STRING=Host=db;Port=5432;Database=mediadb;Username=mediadb_dev;Password=your-password-here
 
   VITE_API_URL=http://localhost:8080
   ALLOWED_ORIGIN=http://localhost:5173
```
   `CONNECTION_STRING` must use `Host=db` (the Compose service name), not `localhost` — the API container reaches Postgres over the Docker network, not through the host.
 
4. Build and start all services:
```bash
   docker compose up --build
```
 
5. Restore packages inside the API container, then apply database migrations:
```bash
   docker compose exec api dotnet restore
   docker compose exec api dotnet ef database update
```
 
6. Download the Discogs data dumps and place them for parsing:
   - Go to [data.discogs.com](https://data.discogs.com/) and download the latest **Artists**, **Masters**, and **Releases** `.xml.gz` dumps
   - Rename them to `artists.xml.gz`, `masters.xml.gz`, and `releases.xml.gz` — no need to decompress, all three parsers stream directly from `.gz`
   - Move all three files into `XmlParsing/XmlFiles/` (create the folder if it doesn't exist):
```
     XmlParsing/
       XmlFiles/
         artists.xml.gz
         masters.xml.gz
         releases.xml.gz
```
 
7. Run the XML parser to convert the dumps into CSVs:
```bash
   cd XmlParsing
   dotnet run
```
   This runs the full parse — releases, artists, and masters — and writes CSVs to `XmlParsing/Exports/`.
 
8. Create `DataImportToPostgres/appsettings.json` with a connection string pointed at your **host** machine (this project runs outside Docker, connecting to Postgres via its published port, not the Docker network — use `localhost`, not `db`):
```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Port=5432;Database=mediadb;Username=mediadb_dev;Password=your-password-here;CommandTimeout=180"
     }
   }
```
 
9. Run the import tool from `DataImportToPostgres` — this requires two separate runs, in order:
```bash
   cd ../DataImportToPostgres
   dotnet run full
   dotnet run
```
   `dotnet run full` performs the full entity/relation import (artists, albums, genres, styles, and their join tables) and must run first, since the format backfill below depends on albums already existing. `dotnet run` (no argument) then backfills the `format` column on the `album` table from the parsed release data.
 
The frontend, API, and PostgreSQL database will all be running as separate containers, networked together via Docker Compose.
 
---

## Architecture Notes

- **Auth:** Access tokens are short-lived JWTs delivered via `httpOnly`, `Secure` cookies (disabled in development). Refresh tokens are long-lived, stored server-side, and rotated on each use — the old token is deleted rather than flagged, so token validity is a simple existence check.
- **Data import:** Discogs XML dumps are parsed via streaming (`XmlReader` over `GZipStream`) rather than loaded fully into memory, since uncompressed dumps can reach tens of gigabytes. Derived data (e.g. release format) is exported to CSV, then bulk-imported into PostgreSQL via batched, idempotent update queries.
- **Read optimization:** Search relies on a GIN index for text search and a materialized view combined with a click-based ranking signal to improve result ordering without recomputing on every request.

---

## Roadmap

1. ~Parse Discogs releases dump → backfill `format` column on masters table~
2. ~Album ratings frontend~
3. Track import from releases dump; establish album → track relationship
4. Song ratings with auto-aggregate to album score + manual override
5. Full album rating flow (per-song ratings → aggregated score)
6. Expand search to albums and songs
7. Update GIN index strategy for albums and songs
8. Update Home Page + Data Population
9. Search UI distinction (labels or tabs for artists/albums/songs)
10. Album lists (private/public)
11. Artist groups (private/public)
12. Backend unit testing
13. Backend integration testing
14. Frontend unit testing (React)
15. Azure deployment (AKS or App Service + Azure Database for PostgreSQL)
16. CI/CD pipeline (GitHub Actions or Azure DevOps)

**Ongoing:** Database optimization pass — indexes, denormalization, and materialized views — deferred until the application reaches a stable feature set. The album → track join is a known candidate for denormalization once track data is imported.

---

## License

This is a personal learning project. License TBD.
