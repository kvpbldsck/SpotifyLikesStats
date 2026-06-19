namespace Misc;

public static class CommonExtensions
{
    extension(UriBuilder builder)
    {
        public UriBuilder SetQuery(Dictionary<string, object> query)
        {
            builder.Query = query
                .Select(q => $"{q.Key}={q.Value}")
                .Join(separator: "&", prefix: "?");

            return builder;
        }
    }

    extension(IEnumerable<string> items)
    {
        public string Join(string separator, string? prefix = null)
        {
            string result = string.Join(separator, items);
            return !string.IsNullOrWhiteSpace(prefix) && !string.IsNullOrWhiteSpace(result)
                ? prefix + result
                : result;
        }
    }
}