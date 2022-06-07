FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["DaprCounter.csproj", "src/DaprCounter.csproj"]
RUN dotnet restore "src/DaprCounter.csproj"
COPY . .
RUN dotnet publish "DaprCounter.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/publish .
USER 1000:1000
EXPOSE 5000
ENTRYPOINT ["dotnet", "DaprCounter.dll"]
