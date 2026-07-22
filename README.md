# MediaBoard

MediaBoard is a full-stack music album rating application built as a hands-on learning project covering full-stack development, containerization, authentication, and database design at scale. Album, artist, and format data is sourced from real-world [Discogs](https://www.discogs.com/) XML data dumps.

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
- Docker & Kubernetes
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

2. Create a `.env` file in the project root with the required secrets:
   ```env
   POSTGRES_USER=mediadb_dev
   POSTGRES_PASSWORD=your-password-here
   POSTGRES_DB=mediadb
   JWT_KEY=your-generated-signing-key
   ```

3. Build and start all services:
   ```bash
   docker compose up --build
   ```

4. Apply database migrations (from inside the API container):
   ```bash
   docker compose exec api dotnet ef database update
   ```

The frontend, API, and PostgreSQL database will all be running as separate containers, networked together via Docker Compose.

---

## Architecture Notes

- **Auth:** Access tokens are short-lived JWTs delivered via `httpOnly`, `Secure` cookies (disabled in development). Refresh tokens are long-lived, stored server-side, and rotated on each use — the old token is deleted rather than flagged, so token validity is a simple existence check.
- **Data import:** Discogs XML dumps are parsed via streaming (`XmlReader` over `GZipStream`) rather than loaded fully into memory, since uncompressed dumps can reach tens of gigabytes. Derived data (e.g. release format) is exported to CSV, then bulk-imported into PostgreSQL via batched, idempotent update queries.
- **Read optimization:** Search relies on a GIN index for text search and a materialized view combined with a click-based ranking signal to improve result ordering without recomputing on every request.

---

## Roadmap

1. ~~Parse Discogs releases dump → backfill `format` column on masters table~~
2. Album ratings frontend
3. Track import from releases dump; establish album → track relationship
4. Song ratings with auto-aggregate to album score + manual override
5. Full album rating flow (per-song ratings → aggregated score)
6. Expand search to albums and songs
7. Update GIN index strategy for albums and songs
8. Search UI distinction (labels or tabs for artists/albums/songs)
9. Album lists (private/public)
10. Artist groups (private/public)
11. Backend unit testing
12. Backend integration testing
13. Frontend unit testing (React)
14. Azure deployment (AKS or App Service + Azure Database for PostgreSQL)
15. CI/CD pipeline (GitHub Actions or Azure DevOps)

**Ongoing:** Database optimization pass — indexes, denormalization, and materialized views — deferred until the application reaches a stable feature set. The album → track join is a known candidate for denormalization once track data is imported.

---

## License

This is a personal learning project. License TBD.
