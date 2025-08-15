# build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ../Inventory.Domain/ Inventory.Domain/
COPY ../Inventory.Application/ Inventory.Application/
COPY ../Inventory.Infrastructure/ Inventory.Infrastructure/
COPY . Inventory.Api/
RUN dotnet restore Inventory.Api/Inventory.Api.csproj
RUN dotnet publish Inventory.Api/Inventory.Api.csproj -c Release -o /out

# run
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Inventory.Api.dll"]
