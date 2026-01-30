

# STAGE 1: Build Stage DbMigrator Project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /app

# Copy everything from the build context
COPY . .


# Copy minimal projects needed for migration (EF Core + Domain + Migrator)
RUN dotnet restore "Acme.ProductSelling.sln"

COPY . .

WORKDIR "src/Acme.ProductSelling.DbMigrator"
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# STAGE 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Acme.ProductSelling.DbMigrator.dll"]