# The Bid Calculation Tool

A full-stack web application for calculating the total price of vehicles (Common or Luxury) at car auctions, including
various fees and charges.


## Project Overview

This application calculates the total cost of purchasing a vehicle at auction by applying different fees based on
vehicle type and price:

- **Basic buyer fee**: 10% of vehicle price (with min/max limits based on type)
- **Seller's special fee**: 2% (Common) or 4% (Luxury) of vehicle price  
- **Association fee**: Tiered based on price ranges ($5-$20)
- **Storage fee**: Fixed $100

### Example Calculation
For a **Common vehicle** priced at **$1,000**:
- Vehicle Price: $1,000
- Basic buyer fee: $50 (10%, max $50 for Common)
- Special fee: $20 (2% for Common)
- Association fee: $10 (price range $500-$1000)
- Storage fee: $100
- **Total**: $1,180


## Architecture

### Technology Stack
- **Backend**: .NET Core 8 (C#) with RESTful API
- **Frontend**: Vue.js 3 with Vite
- **Infrastructure**: Docker & Docker Compose
- **Development**: Hot reload for both frontend and backend

### Project Structure
```
BidCalculationTool/
├── backend/                # .NET Core 8 API
│   ├── Dockerfile          # Production image
│   ├── Dockerfile.dev      # Development with hot reload
│   ├── backend.csproj      # Project configuration
│   └── Program.cs          # Application entry point
├── frontend/               # Vue.js 3 application
│   ├── Dockerfile          # Production image (Nginx)
│   ├── Dockerfile.dev      # Development with hot reload
│   ├── package.json        # Dependencies and scripts
│   └── src/                # Vue.js source code
├── docs/                   # Documentation
│   ├── adr/                # Architectural Decision Records
│   ├── requirements/       # Original coding challenge description
│   └── c4/                 # C4 architecture diagrams
├── scripts/                # Automation scripts
│   └── init.mjs            # Project initialization
└── docker-compose files    # Multi-environment orchestration
```


## Getting Started

### Prerequisites
- **Docker** + **Docker Compose**
- **Node.js** ≥ 18.x (for build scripts)

### Quick Start
```bash
# Install dependencies and initialize environment
npm install
npm run init

# Your application will be available at:
# Frontend: http://localhost:5173 (Vite dev server)
# Backend API: http://localhost:5000/swagger (Swagger UI)
```

### Available Commands
```bash
npm run init    # Initialize and start development environment
npm run dev     # Start development containers with hot reload
npm run prod    # Build and start production containers
npm run down    # Stop all containers
npm run docs    # Generate C4 architecture diagrams
```


## Docker Configuration

The project uses a multi-file Docker Compose setup for different environments:

- **`docker-compose.parent.yml`**: Base configuration
- **`docker-compose.dev.yml`**: Development overrides (hot reload, volume mounts)
- **`docker-compose.prod.yml`**: Production overrides (optimized builds)

### Development Environment
- **Backend**: Uses .NET SDK with `dotnet watch` for hot reload
- **Frontend**: Uses Node.js with Vite dev server for hot reload
- **Volumes**: Source code mounted for real-time development

### Production Environment  
- **Backend**: Multi-stage build with optimized .NET runtime
- **Frontend**: Multi-stage build with Nginx for static file serving
- **Images**: Optimized for deployment (smaller size, security)


## Development Workflow

1. **Initialize**: `npm run init` - Sets up and starts development environment
2. **Develop**: Edit code - changes are automatically reflected via hot reload
3. **Test**: Access frontend at http://localhost:5173 and API at http://localhost:5000 (TODO: adjust URL)
   - Use Swagger UI to test API endpoints
   - Run unit tests in the backend with `dotnet test`
4. **Deploy**: `npm run prod` - Build and test production images

TODO: Describe SonarQube integration for code quality analysis, GitHub Actions for CI/CD, and automated testing.


## Documentation

### Architectural Decisions
Key architectural decisions are documented using ADR (Architectural Decision Records):
- [ADR-001: Project Structure and Technology Stack](./docs/adr/001-project-structure-and-technology-stack.md)
- [ADR-002: Docker and Docker Compose Usage](./docs/adr/002-docker-and-docker-compose-usage.md)
- [ADR-003: REST over GraphQL](./docs/adr/003-use-rest-over-graphql.md)

### Requirements
The original coding challenge requirements are documented in the `docs/requirements` directory:
- [Original Coding Challenge](./docs/requirements/coding-challenge.md)

### C4 Architecture Diagrams
The C4 model is used to visualize the architecture at different levels:
- [Domain Storytelling Diagram](./docs/c4/diagrams/domain-storytelling-diagram.svg)
- [Container Diagram](./docs/c4/diagrams/container-diagram.svg)

You can generate these diagrams using the `npm run docs` command, which uses PlantUML to create SVG files from the C4
model definitions. The resulting diagrams are stored in the `docs/c4/diagrams` directory.


## AI Usage Disclaimer
AI was used as technical support in a "rubber duck debugging" approach, primarily for:

- Validating architectural and technology choices (e.g., REST vs. GraphQL)
- Proposing documentation drafts (README, ADR, API documentation)
- Unblocking configuration issues and syntax problems

No code was copied without critical review, adaptation, and testing. AI served as a 24/7 available technical colleague, 
not as a substitute for critical thinking or system design.


## Test Cases
The application handles various test scenarios as specified in the requirements:

| Vehicle Price | Type    | Basic Fee | Special Fee | Association Fee | Storage Fee | **Total**  |
|---------------|---------|-----------|-------------|-----------------|-------------|------------|
| $398.00       | Common  | $39.80    | $7.96       | $5.00           | $100.00     | **$550.76** |
| $501.00       | Common  | $50.00    | $10.02      | $10.00          | $100.00     | **$671.02** |
| $57.00        | Common  | $10.00    | $1.14       | $5.00           | $100.00     | **$173.14** |
| $1,800.00     | Luxury  | $180.00   | $72.00      | $15.00          | $100.00     | **$2,167.00** |
| $1,100.00     | Common  | $50.00    | $22.00      | $15.00          | $100.00     | **$1,287.00** |
| $1,000,000.00 | Luxury  | $200.00   | $40,000.00  | $20.00          | $100.00     | **$1,040,320.00** |

---

## Final Disclaimer
*This project was developed as a technical assessment demonstrating full-stack development capabilities with modern
tools and best practices. It showcases enterprise-grade architecture patterns, containerization, and development 
workflows suitable for production environments. The implementation prioritizes code quality, maintainability, 
and scalability over feature completeness.*
