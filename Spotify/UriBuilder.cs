using System.Globalization;
using System.Text;

namespace Spotify;

public struct UrlBuilder(string hostAndPath)
{
    private readonly StringBuilder _url = new StringBuilder(hostAndPath);
    private bool _isQueryCharAdded = false;

    public UrlBuilder WithQueryParam<T>(string key, T value) where T : IFormattable
        => WithQueryParam(key, value.ToString(null, CultureInfo.InvariantCulture));

    public UrlBuilder WithQueryParam(string key, string value)
    {
        _url
            .Append(_isQueryCharAdded ? '&' : '?')
            .Append(Uri.EscapeDataString(key))
            .Append('=')
            .Append(Uri.EscapeDataString(value));

        _isQueryCharAdded = true;

        return this;
    }

    public override string ToString() => _url.ToString();
}