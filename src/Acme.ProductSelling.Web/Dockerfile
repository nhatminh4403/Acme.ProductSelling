# STAGE 1: Build & Publish .Web project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# Install Node.js
RUN apt-get update && \
    apt-get install -y curl gnupg && \
    curl -fsSL https://deb.nodesource.com/setup_22.x | bash - && \
    apt-get install -y nodejs

# Install ABP CLI
RUN dotnet tool install -g Volo.Abp.Cli

# Add dotnet tools to the PATH
ENV PATH="${PATH}:/root/.dotnet/tools"

WORKDIR /app

# Copy everything
COPY . .

# Restore Dependencies
RUN dotnet restore "Acme.ProductSelling.sln"

# Navigate to the Web project folder
WORKDIR "/app/src/Acme.ProductSelling.Web"

#  Explicitly install the missing libraries
# This downloads jQuery, Bootstrap, etc., into wwwroot/libs
RUN abp install-libs

# Build and Publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# STAGE 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Acme.ProductSelling.Web.dll"]
