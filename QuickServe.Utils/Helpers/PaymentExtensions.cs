using System.Web;

namespace QuickServe.Utils.Helpers
{
    public static class PaymentExtensions
    {
        /// <summary>
        /// Parse query string to string json object
        /// </summary>
        /// <param name="str"></param>
        /// <returns>string json object</returns>
        public static string ParseQueryStringToJson(this string str)
        {
            var dict = HttpUtility.ParseQueryString(str);
            var json = System.Text.Json.JsonSerializer.Serialize(dict.AllKeys.ToDictionary(k => k, k => dict[k]));

            return json;
        }
    }
}
