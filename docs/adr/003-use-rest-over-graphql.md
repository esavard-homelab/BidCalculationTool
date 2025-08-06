# ADR-003: Choosing REST over GraphQL

## Status
Accepted

## Context
The project consists of developing a price calculation tool for a vehicle sold at auction.
The backend will need to receive a set of parameters (vehicle type, base price, etc.) and return a total including
dynamic fees.

GraphQL was considered as an alternative to a classic REST API, particularly for:

- Offering more flexibility in response structure
- Enabling better evolution of the API contract on the frontend side
- Centralizing interactions in a single schema

## Decision
We have chosen **not to use GraphQL at this stage** and to maintain a **simple RESTful approach** for the following
reasons:

- The current need is minimal: a single operation (`calculate`) with a fixed structure
- REST remains **faster to implement** and **better mastered** in a technical test context
- Adhering to the KISS principle ("Keep It Simple, Stupid") is preferable here
- The architecture remains open to evolving towards GraphQL if the need arises

## Consequences
- The API will expose a `POST /calculate` endpoint that will receive the data necessary for the calculation
- The interface contract is documented via Swagger / OpenAPI
- A future transition to GraphQL would be straightforward to implement if the business logic becomes more complex or
involves multiple entities
