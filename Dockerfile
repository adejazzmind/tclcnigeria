# STAGE 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["tclcnigeria.csproj", "./"]
RUN dotnet restore "tclcnigeria.csproj"

COPY . .
RUN dotnet build "tclcnigeria.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "tclcnigeria.csproj" -c Release -o /app/publish /p:UseAppHost=false

# STAGE 2: Run the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "tclcnigeria.dll"]
