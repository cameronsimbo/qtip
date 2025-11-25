FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["QTip.sln", "./"]
COPY ["QTip.Domain/Domain.csproj", "QTip.Domain/"]
COPY ["QTip.Application/Application.csproj", "QTip.Application/"]
COPY ["QTip.Infrastructure/Infrastructure.csproj", "QTip.Infrastructure/"]
COPY ["QTip.Api/Api.csproj", "QTip.Api/"]

RUN dotnet restore "QTip.Api/Api.csproj"

COPY . .

WORKDIR "/src/QTip.Api"
RUN dotnet publish "Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS="http://0.0.0.0:8080"

ENTRYPOINT ["dotnet", "Api.dll"]


