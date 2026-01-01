# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy csproj and restore dependencies
COPY LogosAPI/*.csproj ./LogosAPI/
RUN dotnet restore LogosAPI/LogosAPI.csproj

# Copy everything else and build
COPY . .
WORKDIR /source/LogosAPI

# Verify Data files exist in source
RUN echo "=== Checking source Data files ===" && \
    ls -la Data/ && \
    wc -l Data/*.json || echo "No Data files found"

# Publish
RUN dotnet publish -c Release -o /publish

# Verify Data files are in publish output
RUN echo "=== Checking publish Data files ===" && \
    ls -la /publish/Data/ || echo "No Data folder in publish"

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published output
COPY --from=build /publish .

# Final verification
RUN echo "=== Final Data check ===" && \
    ls -la /app/Data/ || echo "No Data folder found"

# Koyeb expects port 8000 by default
ENV ASPNETCORE_URLS=http://+:8000
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8000

ENTRYPOINT ["dotnet", "LogosAPI.dll"]
