# Use the official .NET 8.0 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY TrackerApi/*.csproj TrackerApi/
RUN dotnet restore TrackerApi/TrackerApi.csproj

# Copy everything else and build
COPY TrackerApi/ TrackerApi/
RUN dotnet publish TrackerApi/TrackerApi.csproj -c Release -o out

# Use the official .NET 8.0 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 80
ENTRYPOINT ["dotnet", "TrackerApi.dll"]
