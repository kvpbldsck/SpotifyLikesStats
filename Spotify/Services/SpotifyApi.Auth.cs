using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using Spotify.Misc;
using Spotify.Models;

namespace Spotify.Services;

internal sealed partial class SpotifyApi
{
    private async Task<AuthenticationHeaderValue> GenerateAccessToken(HttpClient httpClient)
    {
        (string codeVerifier, string codeChallenge) = PreparePkceData();
        string userAuthCode = await GenerateUserAuthCode(codeChallenge);
        SpotifyTokenResponse accessToken = await GenerateAccessToken(httpClient, userAuthCode, codeVerifier);
        return new AuthenticationHeaderValue(accessToken.TokenType, accessToken.AccessToken);
    }

    private (string codeVerifier, string codeChallenge) PreparePkceData()
    {
        string codeVerifier = GenerateRandomString(64);
        byte[] hashed = GetSha256(codeVerifier);
        string codeChallenge = GetBase64(hashed);

        return (codeVerifier, codeChallenge);
    }

    private async Task<string> GenerateUserAuthCode(string codeChallenge)
    {
        string authUrl = new UriBuilder(settings.AuthUrl)
            .SetQuery(new()
            {
                { "response_type", "code" },
                { "client_id", settings.ClientId },
                { "scope", settings.Scope },
                { "code_challenge_method", "S256" },
                { "code_challenge", codeChallenge },
                { "redirect_uri", settings.RedirectUrl },
            })
            .Uri
            .ToString();

        var spotifyRedirectTask = httpReceiver.ReceiveRequest(settings.RedirectUrl);

        Process.Start(new ProcessStartInfo { FileName = authUrl, UseShellExecute = true });

        var spotifyRedirect = await spotifyRedirectTask;
        return spotifyRedirect.QueryString["code"]!;
    }

    private async Task<SpotifyTokenResponse> GenerateAccessToken(
        HttpClient httpClient,
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

        var response = await httpClient.PostAsync(url, httpContent);
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