using System.Web;

namespace Template.Core.Application.Utils
{
    public static class UrlUtils
    {
        public static string? GetNextURL(string url, int limit, int offset, int total)
        {
            var newOffSet = limit + offset;
            if (newOffSet >= total)
            {
                return null;
            }

            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["offset"] = $"{newOffSet}";
            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }

        public static string? GetPrevURL(string url, int limit, int offset, int total)
        {
            var oldOffSet = offset - limit;
            if (oldOffSet < 0)
            {
                return null;
            }

            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);

            if (oldOffSet == 0)
            {
                query.Remove("offset");
            }
            else
            {
                query["offset"] = $"{oldOffSet}";
            }

            uriBuilder.Query = query.ToString();
            return uriBuilder.ToString();
        }
    }
}