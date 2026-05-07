# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# Copy everything and restore
COPY . ./
RUN dotnet restore AuthAPI/AuthAPI.csproj

# Build and publish
RUN dotnet publish AuthAPI/AuthAPI.csproj -c Release -o out

# Runtime Stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build-env /app/out .

# Expose port 80 (standard for Render/Koyeb)
ENV ASPNETCORE_URLS=http://+:80
# DATABASE_URL can be set in Render/Koyeb environment variables
EXPOSE 80

ENTRYPOINT ["dotnet", "AuthAPI.dll"]
