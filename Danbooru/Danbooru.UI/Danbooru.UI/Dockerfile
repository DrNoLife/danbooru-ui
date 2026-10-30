# Build context is the Danbooru folder (see docker-compose.yml and the CI workflow).

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
# Set by the release workflow from the git tag (v1.2.3 -> 1.2.3).
ARG VERSION=0.0.0-dev
WORKDIR /src

# Restore first, so the package layer is cached until a project file or package version changes.
COPY ["global.json", "Directory.Build.props", "Directory.Packages.props", "./"]
COPY ["Danbooru.ApiWrapper/Danbooru.ApiWrapper.csproj", "Danbooru.ApiWrapper/"]
COPY ["Danbooru.UI/Danbooru.UI/Danbooru.UI.csproj", "Danbooru.UI/Danbooru.UI/"]
RUN dotnet restore "Danbooru.UI/Danbooru.UI/Danbooru.UI.csproj"

COPY ["Danbooru.ApiWrapper/", "Danbooru.ApiWrapper/"]
COPY ["Danbooru.UI/Danbooru.UI/", "Danbooru.UI/Danbooru.UI/"]
RUN dotnet publish "Danbooru.UI/Danbooru.UI/Danbooru.UI.csproj" -c $BUILD_CONFIGURATION -o /app/publish --no-restore /p:UseAppHost=false /p:Version=$VERSION

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled AS final
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Danbooru.UI.dll"]
