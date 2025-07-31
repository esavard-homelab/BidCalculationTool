# ADR-002: Docker and Docker Compose Usage

## Status
Accepted

## Context
The project requires consistent development and deployment environments across team members and different stages 
(development, production). We need quick setup, hot reload for development, and optimized builds for production.

We considered different containerization approaches:
- **Single Docker Compose file**: Simple but lacks environment-specific optimizations
- **Multiple Docker Compose files**: Base configuration with environment-specific overrides
- **No containerization**: Platform-dependent setup with potential inconsistencies

## Decision
We have chosen a **multi-file Docker Compose strategy** with environment-specific configurations:

- **`docker-compose.parent.yml`**: Base configuration shared across environments
- **`docker-compose.dev.yml`**: Development overrides (hot reload, volume mounts)
- **`docker-compose.prod.yml`**: Production overrides (optimized builds, different ports)

Uses Docker Compose override pattern:
```bash
# Development: docker compose -f docker-compose.parent.yml -f docker-compose.dev.yml up
# Production: docker compose -f docker-compose.parent.yml -f docker-compose.prod.yml up
```

## Consequences
- Developer experience: One command setup with hot reload
- Environment parity: Same orchestration across dev/prod
- Team onboarding: New developers start with `npm run init`
- Initial complexity: Multiple files to understand
- Scripts automation abstracts complexity (`npm run dev`, `npm run prod`)
