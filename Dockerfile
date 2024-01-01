FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY testapp-api.csproj testapp-api.csproj
RUN dotnet restore
COPY . .
RUN dotnet publish -o out
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out ./
EXPOSE 8080
ENTRYPOINT [ "dotnet", "testapp-api.dll" ]
