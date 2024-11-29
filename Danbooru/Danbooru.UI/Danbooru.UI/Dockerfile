# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0-noble-chiseled-extra AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0-noble AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Danbooru.UI/Danbooru.UI/Danbooru.UI.csproj", "Danbooru.UI/Danbooru.UI/"]
COPY ["Danbooru.ApiWrapper/Danbooru.ApiWrapper.csproj", "Danbooru.ApiWrapper/"]
COPY ["Danbooru.UI/Danbooru.UI.Client/Danbooru.UI.Client.csproj", "Danbooru.UI/Danbooru.UI.Client/"]
RUN dotnet restore "./Danbooru.UI/Danbooru.UI/Danbooru.UI.csproj"
COPY . .
WORKDIR "/src/Danbooru.UI/Danbooru.UI"
RUN dotnet build "./Danbooru.UI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Danbooru.UI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Danbooru.UI.dll"]