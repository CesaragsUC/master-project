FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Api.Gateway/Api.Gateway.csproj", "Api.Gateway/"]
COPY ["Shared.Kernel/Shared.Kernel.csproj", "Shared.Kernel/"]
RUN dotnet restore "./Api.Gateway/Api.Gateway.csproj"
COPY . .
WORKDIR "/src/Api.Gateway"
RUN dotnet build "./Api.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Api.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.Gateway.dll"]