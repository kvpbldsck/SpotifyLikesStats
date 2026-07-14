using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using Application.Models;
using CSharpFunctionalExtensions;
using Misc;
using Spotify.Models;

namespace Spotify.Services;

internal sealed class SpotifyClient(Settings settings, LocalHttpServer localHttpServer)
{
    private readonly HttpClient _httpClient = new();
    private const string RedirectHtml =
        "<!DOCTYPE html><html><body><p>Authorization is finished. This tab will be, probably, automatically closed in 5 seconds or you can close it manually. Please return back to the app.</p><script>setTimeout(window.close, 5000)</script></body></html>";

    public async Task<Result<T, Error>> GetAsync<T>(string url)
    {
        return await AuthorizeIfNeededAsync()
            .Bind(async () => await SendRequestAsync<T>(new (HttpMethod.Get, url)));
    }

    private async Task<UnitResult<Error>> AuthorizeIfNeededAsync()
    {
        if (_httpClient.DefaultRequestHeaders.Authorization is not null)
        {
            return UnitResult.Success<Error>();
        }

        (string codeVerifier, string codeChallenge) = PreparePkceData();

        return await GenerateUserAuthCodeAsync(codeChallenge)
            .Bind(userAuthCode => GenerateAccessTokenAsync(userAuthCode, codeVerifier))
            .Tap(accessToken => _httpClient.DefaultRequestHeaders.Authorization = new (accessToken.TokenType, accessToken.AccessToken));
    }

    private (string codeVerifier, string codeChallenge) PreparePkceData()
    {
        string codeVerifier = GenerateRandomString(64);
        byte[] hashed = GetSha256(codeVerifier);
        string codeChallenge = GetBase64(hashed);

        return (codeVerifier, codeChallenge);
    }

    private async Task<Result<string, Error>> GenerateUserAuthCodeAsync(string codeChallenge)
    {
        string authUrl = new UrlBuilder(settings.AuthUrl)
            .WithQueryParam("response_type", "code")
            .WithQueryParam("client_id", settings.ClientId)
            .WithQueryParam("scope", settings.Scope)
            .WithQueryParam("code_challenge_method", "S256")
            .WithQueryParam("code_challenge", codeChallenge)
            .WithQueryParam("redirect_uri", settings.RedirectUrl)
            .ToString();

        using var spotifyRedirectSession = localHttpServer.StartListen(settings.RedirectUrl);

        Process.Start(new ProcessStartInfo { FileName = authUrl, UseShellExecute = true });

        var spotifyRedirect = await spotifyRedirectSession.WaitForRequest();

        string? code = spotifyRedirect.QueryString["code"];

        await spotifyRedirectSession.ThenRespond(RedirectHtml, MediaTypeNames.Text.Html);

        return string.IsNullOrWhiteSpace(code)
            ? Result.Failure<string, Error>(Error.Authorization("Failed to obtain authorization code."))
            : code;
    }

    private Task<Result<SpotifyTokenResponse, Error>> GenerateAccessTokenAsync(
        string userAuthCode,
        string codeVerifier)
    {
        string url = settings.AccessTokenUrl;
        var httpContent = new FormUrlEncodedContent(new Dictionary<string, string>()
        {
            { "client_id", settings.ClientId },
            { "grant_type", "authorization_code" },
            { "code", userAuthCode },
            { "redirect_uri", settings.RedirectUrl },
            { "code_verifier", codeVerifier }
        });
        var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = httpContent };

        return SendRequestAsync<SpotifyTokenResponse>(request)
            .MapError(_ => Error.Authorization("Spotify authorization failed"));
    }

    private async Task<Result<T, Error>> SendRequestAsync<T>(HttpRequestMessage request)
    {
        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => Result.Failure<T, Error>(Error.Authorization("Spotify request failed")),
                HttpStatusCode.TooManyRequests => Result.Failure<T, Error>(Error.Request("Too many request to Spotify")),
                _ => Result.Failure<T, Error>(Error.Authorization("Spotify request failed"))
            };
            // Spotify api can send error description in response body, if app will grow it is needed to be parsed
        }

        var body = await response.Content.ReadFromJsonAsync<T>();
        return body is null
            ? Result.Failure<T, Error>(Error.Request("Response is either empty or in unexpected format"))
            : body;

    }

    private static string GenerateRandomString(int length)
    {
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return RandomNumberGenerator.GetString(alphabet, length);
    }

    private static byte[] GetSha256(string text)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(text);
        return SHA256.HashData(bytes);
    }

    private static string GetBase64(byte[] input)
    {
        return Convert.ToBase64String(input)
            .Replace("=", "")
            .Replace("+", "-")
            .Replace("/", "_");
    }
}