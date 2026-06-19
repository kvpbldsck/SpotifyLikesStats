using System.Net;

namespace Spotify.Services;

internal interface IHttpReceiver
{
    Task<HttpListenerRequest> ReceiveRequest(string receiveUrl);
}

internal sealed class HttpReceiver : IHttpReceiver
{
    public async Task<HttpListenerRequest> ReceiveRequest(string receiveUrl)
    {
        using var listener = new HttpListener();
        listener.Prefixes.Add(receiveUrl);
        listener.Start();

        var context = await listener.GetContextAsync();
        return context.Request;
    }
}