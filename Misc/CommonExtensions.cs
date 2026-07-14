namespace Misc;

public static class CommonExtensions
{
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