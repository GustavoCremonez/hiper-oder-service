FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Hiper.API/Hiper.API.csproj", "Hiper.API/"]
COPY ["src/Hiper.Application/Hiper.Application.csproj", "Hiper.Application/"]
COPY ["src/Hiper.Domain/Hiper.Domain.csproj", "Hiper.Domain/"]
COPY ["src/Hiper.Infrastructure/Hiper.Infrastructure.csproj", "Hiper.Infrastructure/"]

RUN dotnet restore "Hiper.API/Hiper.API.csproj"

COPY src/ .

WORKDIR "/src/Hiper.API"
RUN dotnet build "Hiper.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Hiper.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hiper.API.dll"]
