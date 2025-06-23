using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Script.Serialization;

namespace LinkvertiseBypass
{
    internal static class Program
    {
        private static void Main()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Enter a valid Linkvertise URL:");

                string link = Console.ReadLine();

                // Known Linkvertise domains
                string[] validPatterns =
                {
                    "https://linkvertise.com/",
                    "https://up-to-down.net/",
                    "https://link-to.net/",
                    "https://direct-link.net/",
                    "https://file-link.net/"
                };

                if (!validPatterns.Any(pattern => link.StartsWith(pattern, StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine("❌ The input is not a valid Linkvertise link.\nPress any key to try again.");
                    Console.ReadKey();
                    continue;
                }

                try
                {
                    Console.Clear();
                    LinkvertiseResponse result = Bypass(link);

                    Console.WriteLine("✅ Bypass Successful!");
                    Console.WriteLine($"🔎 Query:       {result.Query}");
                    Console.WriteLine($"🎯 Destination: {result.Destination}");
                    Console.WriteLine($"⏱️ Time:         {result.Time} ms");
                    Console.WriteLine($"🕒 Cache TTL:    {result.CacheTtl}");
                    Console.WriteLine($"🔌 Plugin:       {result.Plugin}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Bypass failed: {ex.Message}");
                }

                Console.WriteLine("\nPress any key to bypass another link...");
                Console.ReadKey();
            }
        }

        /// <summary>
        /// Sends the given Linkvertise URL to the public bypass API and returns the parsed response.
        /// </summary>
        private static LinkvertiseResponse Bypass(string url)
        {
            string api = $"https://bypass.bot.nu/bypass2?url={url}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(api);
            request.Referer = "https://thebypasser.com";
            request.Headers.Add("DNT", "1");
            request.Accept = "*/*";

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string json = reader.ReadToEnd();
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                var result = serializer.Deserialize<LinkvertiseResponse>(json);

                if (!result.Success)
                    throw new Exception("API returned an unsuccessful response.");

                return result;
            }
        }
    }

    /// <summary>
    /// Deserializable class to represent the API response.
    /// </summary>
    public class LinkvertiseResponse
    {
        public bool Success { get; set; }
        public string Destination { get; set; }
        public string Query { get; set; }
        public int Time { get; set; }
        public long CacheTtl { get; set; }
        public string Plugin { get; set; }

        // Handles possible variations in API field naming
        public void FixLegacyFields(dynamic raw)
        {
            if (string.IsNullOrEmpty(Query) && raw.ContainsKey("query"))
                Query = raw["query"];
        }
    }
}
