# Inventory System

## Overview
This is an **Inventory Management API** built with **.NET 8**, **PostgreSQL**, and **Docker**.  
It supports core inventory operations such as adding, updating, and tracking products, along with testing support.

## Tech Stack
- **Backend**: ASP.NET Core Web API
- **Database**: PostgreSQL
- **Containerization**: Docker, Docker Compose
- **Testing**: xUnit, Moq
- **Architecture**: Clean Architecture (API, Application, Domain, Infrastructure layers)

## Features
- Add, update, delete, and view products
- Stock tracking
- Unit tests for key functionalities
- Docker support for easy deployment

## Setup Instructions

### Prerequisites
- .NET 8 SDK
- Docker & Docker Compose
- PostgreSQL (optional if not using Docker)

### Run with Docker
```bash
docker-compose up --build
