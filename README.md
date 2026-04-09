# Tarah E-Commerce Platform

A microservices-based e-commerce backend built with **.NET Core**, focused on exploring real-world distributed systems challenges such as consistency, reliability, and service decoupling.

What started as a simple backend evolved into a practical exploration of microservices, where each design decision led to deeper considerations around communication patterns, failure handling, and data consistency.

The system is composed of independent services that communicate through **RabbitMQ using MassTransit**, enabling loosely coupled workflows and event-driven interactions.

---

## Architecture

The platform follows a microservices architecture with clear separation between:

- **Identity Service** (authentication & user data)
- **Business Service** (products, carts, orders, etc.)

Each service owns its data and communicates using a mix of **asynchronous events** and **synchronous requests** where appropriate.

### Key Design Decisions

- Event-driven communication for cross-service workflows  
- No shared database between services  
- Explicit service boundaries and responsibilities  
- Combination of sync (HTTP) and async (messaging) communication  

---

## Core Concepts & Challenges

### Event-Driven Communication

Cross-service workflows are handled using events via RabbitMQ.

Example:
- User registration → `UserRegistered` event → cart initialization in the business service

This allows services to react to changes without tight coupling.

---

### Reliable Messaging (Outbox Pattern)

To avoid message loss during failures, the system uses the **MassTransit Outbox pattern**.

This ensures that:
- Database changes and event publishing are coordinated  
- Messages are not lost if failures occur after persistence  

---

### Atomicity & Consistency

Several non-trivial consistency challenges were addressed:

- **Database + Event Publishing Atomicity**  
  Ensuring that events are only published if the corresponding database operation succeeds  

- **Database + File System Atomicity**  
  Handling product image uploads in a way that avoids orphaned files or inconsistent state during failures  

---

### Eventual Consistency

The system is designed around **eventual consistency** rather than tightly coupled synchronous operations.

This allows:
- Better service independence  
- More resilient workflows  
- Reduced cascading failures  

---

### Fault Tolerance & Fallbacks

To improve resilience and maintain loose coupling:

- Services can **fall back to lazy loading strategies** when the message broker is unavailable  
- Critical operations are not tightly dependent on real-time event delivery  

---

### Token Invalidation

Authentication is not based solely on token expiration:

- Token invalidation mechanisms are implemented to handle logout and security scenarios more reliably  

---

## Technologies

### Backend
- .NET Core  
- C#  

### Messaging
- RabbitMQ  
- MassTransit  

### Architecture
- Microservices  
- Event-driven architecture  
- Asynchronous messaging  

---

## Goals of the Project

This project was built to explore:

- Designing microservices-based systems  
- Handling real-world consistency challenges  
- Implementing reliable messaging patterns (Outbox)  
- Managing tradeoffs between synchronous and asynchronous communication  
- Building loosely coupled and resilient services  

---

## Project Status

The project is continuously evolving.

Future improvements include:
- Performance testing and optimization  
- Further resilience improvements  
- Refining existing workflows as new concepts are explored  
