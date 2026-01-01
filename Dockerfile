# Build stage - Use SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy only the project file first (bypasses .slnx detection, enables layer caching)
COPY LogosAPI/LogosAPI.csproj ./LogosAPI/
RUN dotnet restore LogosAPI/LogosAPI.csproj

# Copy everything else
COPY . .
WORKDIR /source/LogosAPI

# Verify Data files exist in source
RUN echo "=== Checking source Data files ===" && \
    ls -la Data/ && \
    wc -l Data/*.json || echo "No Data files found"

# Publish targeting the .csproj directly (bypasses .slnx)
RUN dotnet publish LogosAPI.csproj -c Release -o /publish --no-restore

# Verify Data files are in publish output
RUN echo "=== Checking publish Data files ===" && \
    ls -la /publish/Data/ || echo "No Data folder in publish"

# Runtime stage - Use lean ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published output from build stage
COPY --from=build /publish .

# Final verification of Data files
RUN echo "=== Final Data check ===" && \
    ls -la /app/Data/ || echo "No Data folder found"

# Port configuration - Koyeb expects port 8000
ENV ASPNETCORE_URLS=http://+:8000
ENV ASPNETCORE_ENVIRONMENT=Production

# Memory optimization for 1GB RAM instance
# GC Heap Hard Limit = 953MB (0x3B9ACA00) to prevent OOM on 1GB container
# This leaves ~71MB headroom for non-heap memory (stack, native allocations, etc.)
ENV DOTNET_GCHeapHardLimit=0x3B9ACA00

EXPOSE 8000

ENTRYPOINT ["dotnet", "LogosAPI.dll"]