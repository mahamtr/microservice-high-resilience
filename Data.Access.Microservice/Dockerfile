FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Data.Access.Microservice/Data.Access.Microservice.csproj", "Data.Access.Microservice/"]
RUN dotnet restore "Data.Access.Microservice/Data.Access.Microservice.csproj"
COPY . .
WORKDIR "/src/Data.Access.Microservice"
RUN dotnet build "Data.Access.Microservice.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Data.Access.Microservice.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Data.Access.Microservice.dll"]
