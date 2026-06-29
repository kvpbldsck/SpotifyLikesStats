using System.Net;
using System.Text;

namespace Spotify.Services;

internal sealed class LocalHttpServer
{
    public Session StartListen(string receiveUrl)
    {
        var listener = new HttpListener();
        listener.Prefixes.Add(receiveUrl);
        listener.Start();

        return new Session(listener);
    }

    public sealed class Session(HttpListener listener) : IDisposable
    {
        private HttpListenerContext? _context;

        public async Task<HttpListenerRequest> WaitForRequest()
        {
            _context ??= await listener.GetContextAsync();

            return _context.Request;
        }

        public async Task ThenRespond(string response, string mimeType)
        {
            if (_context is null)
            {
                return;
            }

            _context.Response.ContentType = mimeType;
            _context.Response.ContentEncoding = Encoding.UTF8;
            await _context.Response.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(response));
            _context.Response.Close();
        }

        public void Dispose()
        {
            listener.Close();
            ((IDisposable)listener).Dispose();
        }
    }
}
