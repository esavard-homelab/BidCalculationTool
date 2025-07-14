# ADR-001: Project Structure and Technology Stack

## Status
Accepted

## Context
Development of a Bid Calculation Tool demonstrating modern full-stack capabilities. The application requires a user 
interface for input and a backend API for price calculations.

We considered different architectural approaches:
- **Monolithic application**: Single deployable unit with embedded frontend
- **Modular monolith**: Single application with internal module boundaries  
- **Distributed architecture**: Separate frontend and backend services

## Decision
We have chosen a **distributed architecture** with clear separation between frontend and backend services:

- **Frontend**: Vue.js 3 with TypeScript (as required by Progi)
- **Backend**: .NET 8 (C#) REST API (as required by Progi)
- **Infrastructure**: Docker containers for each service
- **Repository**: Monorepo structure for development efficiency

This is a **2-tier distributed architecture** where frontend and backend are separate deployable services communicating
over HTTP/REST.

## Consequences
- Clear separation of concerns between UX and business logic
- Technology flexibility for optimal tool selection per layer
- Independent scaling and deployment capabilities
- Network overhead for service communication
- Docker Compose orchestration simplifies local development
