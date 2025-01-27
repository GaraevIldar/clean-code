# Шаг 1: Базовый образ для ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

# Шаг 2: SDK для сборки приложения
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ./WebApplication1.csproj ./
RUN dotnet restore "./WebApplication1.csproj"
COPY . ./
RUN dotnet build "./WebApplication1.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Шаг 3: Публикация
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebApplication1.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Шаг 4: Финальный образ
FROM base AS final

# Копируем все нужные файлы
COPY --from=publish /app/publish .

# Запуск приложения
ENTRYPOINT ["dotnet", "WebApplication1.dll"]
