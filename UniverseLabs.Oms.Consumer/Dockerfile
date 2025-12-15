FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY UniverseLabs.Oms.Consumer/UniverseLabs.Oms.Consumer.csproj UniverseLabs.Oms.Consumer/
COPY UniverseLabs.Common/UniverseLabs.Common.csproj UniverseLabs.Common/
COPY UniverseLabs.Oms.Models/UniverseLabs.Oms.Models.csproj UniverseLabs.Oms.Models/
COPY UniverseLabs.Messages/UniverseLabs.Messages.csproj UniverseLabs.Messages/
RUN dotnet restore UniverseLabs.Oms.Consumer/UniverseLabs.Oms.Consumer.csproj

COPY UniverseLabs.Oms.Consumer/ UniverseLabs.Oms.Consumer/
COPY UniverseLabs.Common/ UniverseLabs.Common/
COPY UniverseLabs.Oms.Models/ UniverseLabs.Oms.Models/
COPY UniverseLabs.Messages/ UniverseLabs.Messages/

RUN dotnet publish UniverseLabs.Oms.Consumer/UniverseLabs.Oms.Consumer.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "UniverseLabs.Oms.Consumer.dll"]  