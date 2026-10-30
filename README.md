# danbooru-ui

![CI](https://github.com/drnolife/danbooru-ui/actions/workflows/dotnet.yml/badge.svg)

## Overview
"danbooru-ui" is an alternative frontend for the popular image board [Danbooru](https://danbooru.donmai.us/). Designed with aesthetics in mind, it aims to provide a more visually appealing user interface for browsing images. The project focuses primarily on image viewing, sidelining community features like forums, at least in its current state.

## Features
- **Grid**: a justified, endlessly scrolling grid where pages become markers. Jump to any page, resize thumbnails, refine with related tags.
- **Search palette** (`Ctrl+K` or `/`): tag autocomplete, `Tab` to add and `Shift+Tab` to exclude, saved and recent searches.
- **Ratings**: pick any mix of General, Sensitive, Questionable and Explicit; remembered per browser.
- **Viewer**: the image fills the screen, with tags, save, original, similar and copy link beside it. Arrow keys step through the results.
- **Feed**: one post per screen; `J`/`K` or scroll.
- **Works on phones, tablets and desktops**, with a bottom navigation bar on small screens.

## Technology Stack
- **Language**: C#
- **Framework**: Blazor on .NET 10
- **Rendering**: Static server-side rendering, with interactive server components where needed
- **HTTP**: `IHttpClientFactory` with the standard resilience handler (retries, timeouts, circuit breaker)
- **Tests**: xUnit v3 on Microsoft.Testing.Platform
- **Deployment**: Docker + Docker Compose, images published to GitHub Container Registry on release

## Installation and Setup
1. Clone the repository: `git clone https://github.com/DrNoLife/danbooru-ui`
2. Navigate to the Danbooru directory: `cd Danbooru`
3. Run the Docker container: `docker compose up` (add `--build` to build the image from source)

The app listens on port `10100`, and `/healthz` reports whether it is up.

## Development
Requires the .NET 10 SDK (pinned in `Danbooru/global.json`).

```sh
cd Danbooru
dotnet run --project Danbooru.UI/Danbooru.UI
dotnet test
```

Package versions are managed centrally in `Danbooru/Directory.Packages.props`. CI treats compiler warnings as errors.

## Releasing
Releases are driven by git tags:

```sh
git tag -a v1.2.3 -m "Imeji 1.2.3"
git push origin v1.2.3
```

The `Release` workflow runs CI, publishes `ghcr.io/drnolife/danbooru-ui` as `1.2.3`, `1.2` and `latest`, and creates a GitHub release with generated notes.

To deploy, set `IMAGE_TAG=1.2.3` in the `.env` next to `docker-compose.yml` on the server and run `docker compose pull && docker compose up -d`. Roll back by setting the previous version and running the same command. `/version` shows which release is running.

No API key is required as the project only uses GET requests from the API.

## Usage
The "danbooru-ui" can be used through any web browser. It functions as a frontend replacement for Danbooru, focusing on image viewing. For instance, to view images of "Hatsune Miku", simply search and enjoy the results in a gallery view or via infinite scrolling.

## Contribution Guidelines
Contributions are welcome through pull requests. Contributors are expected to:
- Adhere to strict C# coding style guidelines.
- Employ inversion of control where it makes sense.

## License
This project is licensed under the GNU General Public License v3.0. This means that any derivative work must also be open source and distributed under the GPLv3. See the LICENSE file for more details.
