﻿# WhisperVote

WhisperVote is a voting application built with C# and ASP.NET Core. It uses MySQL as the database and Docker for containerization.
This project was created as part of the course "IT-Projekt" at the Ostfalia University of Applied Sciences in Suderburg.
All four projects are parts of the WhisperVote application.

## Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Docker](https://www.docker.com/get-started)

## Getting Started

### Clone the repository

```sh
git clone https://github.com/Drizein/WhisperVote-Backend-Auth.git
cd WhisperVote-Backend-Auth
```

### Set the environment variables

modify the `docker-compose.yml` file to set the environment variables for the MySQL container and the .NET application.

```sh
      - 'MYSQL_DATABASE=WhisperVoteAuth'
      - 'MYSQL_PASSWORD=SuperSicheresPasswort123!'
      - 'MYSQL_ROOT_PASSWORD=SuperSicheresPasswort123!'
      - 'MYSQL_USER=WhisperVote'
      - 'ConnectionStrings__AuthServer=http://auth:9912'
      - 'ASPNETCORE_HTTP_PORTS=9912'
```

### Run the Docker container

```sh
docker compose up -d
```

### Access the application

Open your browser and navigate to `http://localhost:9912`.