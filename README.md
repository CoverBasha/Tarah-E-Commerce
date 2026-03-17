# Tarah E-Commerce Platform

A microservices-based e-commerce backend built with **.NET Core**, designed to explore distributed systems, event-driven architecture, and asynchronous service communication.

This project also serves as a consolidated application of everything I have learned in backend development so far, including API design, messaging systems, and service architecture.

The system is composed of independent services that communicate through **RabbitMQ using MassTransit**, enabling loosely coupled workflows for order processing, inventory updates, and other business events.

---

## Architecture

The platform follows a microservices architecture, where each service is responsible for a specific business capability.

Services communicate using asynchronous messaging rather than direct service-to-service HTTP calls. This approach improves scalability, resilience, and service independence.

### Key Principles

- Loose coupling between services  
- Event-driven communication  
- Clear service boundaries  
- Asynchronous workflows  

Messaging is handled through **RabbitMQ**, while **MassTransit** provides an abstraction layer for message publishing and consumption.

---

## Core Concepts

### Event-Driven Communication

Services publish and consume domain events through RabbitMQ to coordinate workflows such as:

- Order placement  
- Inventory updates  
- Order processing  

This allows services to react to events without direct dependencies.

---

### Message Contracts

Services communicate using shared message contracts, ensuring a consistent structure for events exchanged between services.

This enables services to evolve independently while maintaining compatibility.

---

### Asynchronous Workflows

Operations that span multiple services are handled asynchronously using events, allowing the system to remain responsive while processing complex workflows in the background.

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
- Implementing event-driven communication  
- Handling service-to-service messaging  
- Structuring loosely coupled distributed services  

---

## Project Status

The platform is currently in active development as new services and workflows are being implemented.
