# Incident Management MVP

This repository contains a minimal, production-minded prototype of an Incident Management System.

Folders:
- backend/: ASP.NET Core (.NET 6) API
- frontend/: React (Vite) SPA
- architecture/: architecture diagrams and notes
- azure/: Azure Function placeholder for notifications

Quickstart (local using Docker Compose):
1. Copy .env.example to .env and set secrets (SQL SA_PASSWORD, AZURE_STORAGE_CONNECTION_STRING etc.)
2. docker-compose up --build
3. Backend: https://localhost:5001 (or http://localhost:5000)
4. Frontend: http://localhost:3000

See architecture/ and backend/README.md and frontend/README.md for more details.
