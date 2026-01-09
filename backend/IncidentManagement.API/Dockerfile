FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["IncidentManagement.API/IncidentManagement.API.csproj", "IncidentManagement.API/"]
RUN dotnet restore "IncidentManagement.API/IncidentManagement.API.csproj"
COPY . .
WORKDIR "/src/IncidentManagement.API"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "IncidentManagement.API.dll"]
