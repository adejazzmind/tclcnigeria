# STAGE 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["tclcnigeria.csproj", "./"]
RUN dotnet restore "tclcnigeria.csproj"

# Copy the rest of the code and build it
COPY . .
RUN dotnet build "tclcnigeria.csproj" -c Release -o /app/build

# Publish the app to a folder
FROM build AS publish
RUN dotnet publish "tclcnigeria.csproj" -c Release -o /app/publish /p:UseAppHost=false

# STAGE 2: Run the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Tell Docker how to start the app
ENTRYPOINT ["dotnet", "tclcnigeria.dll"]