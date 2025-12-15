FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /

COPY UniverseLabs.Oms/UniverseLabs.Oms.csproj UniverseLabs.Oms/
COPY UniverseLabs.Common/UniverseLabs.Common.csproj UniverseLabs.Oms.Common/
COPY UniverseLabs.Oms.Models/UniverseLabs.Oms.Models.csproj UniverseLabs.Oms.Models/
COPY UniverseLabs.Oms.Migrations/UniverseLabs.Oms.Migrations.csproj UniverseLabs.Oms.Migrations/
COPY UniverseLabs.Messages/UniverseLabs.Messages.csproj Messages/
RUN dotnet restore "UniverseLabs.Oms/UniverseLabs.Oms.csproj"

COPY . .

RUN dotnet publish "UniverseLabs.Oms/UniverseLabs.Oms.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "UniverseLabs.Oms.dll"]