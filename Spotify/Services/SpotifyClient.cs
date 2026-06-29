using System.Diagnostics;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using Misc;
using Spotify.Models;

namespace Spotify.Services;

internal sealed class SpotifyClient(Settings settings, LocalHttpServer localHttpServer)
{
    private readonly HttpClient _httpClient = new();
    private const string RedirectHtml =
        "<!DOCTYPE html><html><body><p>Authorization is finished. This tab will be, probably, automatically closed in 5 seconds or you can close it manually. Please return back to the app.</p><script>setTimeout(window.close, 5000)</script></body></html>";

    public async Task<T> GetAsync<T>(string url)
    {
        await EnsureAuthorizedAsync();

        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    private async Task EnsureAuthorizedAsync()
    {
        if (_httpClient.DefaultRequestHeaders.Authorization is not null)
        {
            return;
        }

        (string codeVerifier, string codeChallenge) = PreparePkceData();
        string userAuthCode = await GenerateUserAuthCodeAsync(codeChallenge);
        SpotifyTokenResponse accessToken = await GenerateAccessTokenAsync(userAuthCode, codeVerifier);
        _httpClient.DefaultRequestHeaders.Authorization = new (accessToken.TokenType, accessToken.AccessToken);
    }

    private (string codeVerifier, string codeChallenge) PreparePkceData()
    {
        string codeVerifier = GenerateRandomString(64);
        byte[] hashed = GetSha256(codeVerifier);
        string codeChallenge = GetBase64(hashed);

        return (codeVerifier, codeChallenge);
    }

    private async Task<string> GenerateUserAuthCodeAsync(string codeChallenge)
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

        string code = spotifyRedirect.QueryString["code"]!;

        await spotifyRedirectSession.ThenRespond(RedirectHtml, MediaTypeNames.Text.Html);

        return code;
    }

    private async Task<SpotifyTokenResponse> GenerateAccessTokenAsync(
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

        var response = await _httpClient.PostAsync(url, httpContent);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<SpotifyTokenResponse>())!;
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