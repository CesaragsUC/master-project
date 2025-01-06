FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Api.Produto/Product.Api.csproj", "Api.Produto/"]
COPY ["Application.Dtos/Application.Dtos.csproj", "Application.Dtos/"]
COPY ["Domain/Product.Domain.csproj", "Domain/"]
COPY ["Messaging.Contracts/Messaging.Contracts.csproj", "Messaging.Contracts/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrasctructure/"]
COPY ["Product.Services/Product.Services.csproj", "src/Product.Services/"]
RUN dotnet restore "./Api.Produto/Product.Api.csproj"
COPY . .
WORKDIR "/src/Api.Produto"
RUN dotnet build "./Product.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Product.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Product.Api.dll"]