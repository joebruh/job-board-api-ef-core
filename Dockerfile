# # Stage 1: Build
# FROM mcr.microsoft.com/dotnet/sdk:9.0.303-noble AS build

# WORKDIR /src

# # restore 
# COPY ["src/EntityApi.API/EntityApi.API.csproj", "EntityApi.API"]
# COPY ["src/EntityApi.Core/EntityApi.Core.csproj", "EntityApi.Core"]
# COPY ["src/EntityApi.Data/EntityApi.Data.csproj", "EntityApi.Data"]

# RUN dotnet restore 'EntityApi.API/EntityApi.API.csproj'
# RUN dotnet restore 'EntityApi.Core/EntityApi.Core.csproj'
# # RUN dotnet restore 'EntityApi.Data/EntityApi.Data.csproj'

# # build
# COPY ["src/EntityApi.API", "EntityApi.API"]
# COPY ["src/EntityApi.Core", "EntityApi.Core"]
# COPY ["src/EntityApi.Data", "EntityApi.Data"]

# WORKDIR /src/EntityApi.API
# RUN dotnet build "EntityApi.API.csproj" -c Release -o /app/build

# # Stage 2: Publish

# FROM build AS publish
# RUN dotnet publish "EntityApi.API.csproj" -c Release -o /app/publish


# # Stage 3: Run
# FROM mcr.microsoft.com/dotnet/aspnet:9.0-noble
# ENV ASPNETCORE_HTTP_PORTS=5001
# EXPOSE 5001
# WORKDIR /app
# COPY --from=publish /app/publish .
# ENTRYPOINT [ "dotnet", "EntityApi.dll" ]

# Stage 1: Build
# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
FROM mcr.microsoft.com/dotnet/sdk:9.0.303-noble AS build

WORKDIR /app

# Copy csproj files first (layer caching)
COPY src/EntityApi.API/EntityApi.API.csproj ./src/EntityApi.API/
COPY src/EntityApi.Core/EntityApi.Core.csproj ./src/EntityApi.Core/
COPY src/EntityApi.Data/EntityApi.Data.csproj ./src/EntityApi.Data/

# Restore dependencies
RUN dotnet restore "src/EntityApi.API/EntityApi.API.csproj"
RUN dotnet restore "src/EntityApi.Data/EntityApi.Data.csproj"

# Copy the rest of the source
COPY ./src ./src

# Build and publish
RUN dotnet publish "src/EntityApi.API/EntityApi.API.csproj" -c Release -o /app/publish

# Stage 2: Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0-noble
# ENV ASPNETCORE_HTTP_PORTS=5001
# EXPOSE 5001
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "EntityApi.API.dll"]
