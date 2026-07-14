# SpotifyLikesStats

A console application that imports a user's saved tracks from Spotify and generates statistics about their music library.

## Features

* Spotify authentication via PKCE
* Import of saved tracks
* Automatic pagination handling
* Progress reporting in console
* Music library statistics calculation
* Structured error handling using Result pattern

## Requirements

* .NET 10 SDK
* Spotify account
* Spotify Developer application

Verify your .NET installation:

```bash
dotnet --version
```

## Configuration

Create a Spotify application in the [Spotify Developer Dashboard](https://developer.spotify.com/dashboard).

Add the following redirect URI:

```text
http://127.0.0.1:5000/callback/
```

Create a local configuration file:

```text
appsettings.Local.json
```

using the provided template:

```text
appsettings.Local.example.json
```

Example:

```json
{
    "Spotify": {
        "ClientId": "replace-with-your-spotify-client-id"
    }
}
```

The local configuration file is ignored by Git and should never be committed.

## Build

```bash
dotnet restore
dotnet build
```

## Run

```bash
dotnet run --project SpotifyLikesStats
```

During startup the application will:

1. Open Spotify authorization page in the browser.
2. Wait for the OAuth callback.
3. Retrieve an access token.
4. Download saved tracks.
5. Calculate statistics.
6. Display results.

## Solution Structure

```text
SpotifyLikesStats
    Application entry point and DI configuration.

Application
    Use cases and orchestration layer.

Spotify
    Spotify API integration and authentication.

Stats
    Statistics calculation logic.

View
    Console UI and progress reporting.
```

High-level flow:

```text
Authenticate
    ↓
Import Tracks
    ↓
Calculate Statistics
    ↓
Display Results
```

## Design Decisions

### PKCE Authentication

Since this is a public client application, PKCE is used instead of storing a client secret.

### Railway Oriented Programming

Expected failures are represented using `Result<T>` from `CSharpFunctionalExtensions`, allowing business workflows to be expressed as a sequence of operations without exception-driven control flow.

### Modular Architecture

The Spotify integration, statistics calculation, and presentation layers are isolated from each other and coordinated through the application layer.

## Limitations

Current limitations:

* Single-user execution
* No persistent storage
* Fixed callback port
* Requires active Spotify API access

## Feedback Welcome

I would especially appreciate feedback on:

* module boundaries and dependencies
* application layer design
* error modeling
* Result pattern usage
* testability
* naming and code organization

## License

MIT
