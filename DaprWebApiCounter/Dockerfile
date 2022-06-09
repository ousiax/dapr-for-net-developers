FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DaprWebApiCounter.csproj", "src/DaprWebApiCounter.csproj"]
RUN dotnet restore "src/DaprWebApiCounter.csproj"
COPY . .
RUN dotnet publish "DaprWebApiCounter.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/publish .
USER 1000:1000
EXPOSE 5000
ENTRYPOINT ["dotnet", "DaprWebApiCounter.dll"]
