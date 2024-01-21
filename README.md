# danbooru-ui

## Overview
"danbooru-ui" is an alternative frontend for the popular image board [Danbooru](https://danbooru.donmai.us/). Designed with aesthetics in mind, it aims to provide a more visually appealing user interface for browsing images. The project focuses primarily on image viewing, sidelining community features like forums, at least in its current state.

## Features
- **Gallery View**: Displays the latest added images in a visually appealing gallery format.
- **Search Functionality**: Allows users to search for images based on specific queries.
- **Image Details View**: Offers a detailed view of individual images.
- **Infinite Scrolling**: Enhances the browsing experience with a seamless scrolling feature.
- **Optimized for Vertical Display**: Best experienced on mobile devices or vertically oriented browser windows.

## Technology Stack
- **Language**: C#
- **Framework**: Blazor with .NET 8 features
- **Rendering**: Server-side rendering and server interactivity
- **Deployment**: Docker + Docker Compose

## Installation and Setup
1. Clone the repository: `git clone https://github.com/DrNoLife/danbooru-ui`
2. Navigate to the Danbooru directory: `cd Danbooru`
3. Run the Docker container: `docker compose up`

No API key is required as the project only uses GET requests from the API.

## Usage
The "danbooru-ui" can be used through any web browser. It functions as a frontend replacement for Danbooru, focusing on image viewing. For instance, to view images of "Hatsune Miku", simply search and enjoy the results in a gallery view or via infinite scrolling.

## Contribution Guidelines
Contributions are welcome through pull requests. Contributors are expected to:
- Adhere to strict C# coding style guidelines.
- Employ inversion of control where it makes sense.

## License
This project is licensed under the GNU General Public License v3.0. This means that any derivative work must also be open source and distributed under the GPLv3. See the LICENSE file for more details.
