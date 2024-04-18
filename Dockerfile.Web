#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TVScanner/TVScanner.csproj", "TVScanner/"]
COPY ["TVScanner.Shared/TVScanner.Shared.csproj", "TVScanner.Shared/"]
RUN dotnet restore "./TVScanner/TVScanner.csproj"
COPY . .
WORKDIR "/src/TVScanner"
RUN dotnet build "./TVScanner.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TVScanner.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# We then get the base image for Nginx and set the 
# work directory 
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# We'll copy all the contents from wwwroot in the publish
# folder into nginx/html for nginx to serve. The destination
# should be the same as what you set in the nginx.conf.
COPY --from=publish /app/publish/wwwroot /usr/local/webapp/nginx/html
COPY ["TVScanner/nginx.conf", "/etc/nginx/nginx.conf"] 