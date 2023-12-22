#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Danbooru.UI/Danbooru.UI/Danbooru.UI.csproj", "Danbooru.UI/Danbooru.UI/"]
COPY ["Danbooru.ApiWrapper/Danbooru.ApiWrapper.csproj", "Danbooru.ApiWrapper/"]
COPY ["Danbooru.UI/Danbooru.UI.Client/Danbooru.UI.Client.csproj", "Danbooru.UI/Danbooru.UI.Client/"]
RUN dotnet restore "./Danbooru.UI/Danbooru.UI/./Danbooru.UI.csproj"
COPY . .
WORKDIR "/src/Danbooru.UI/Danbooru.UI"
RUN dotnet build "./Danbooru.UI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Danbooru.UI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Danbooru.UI.dll"]