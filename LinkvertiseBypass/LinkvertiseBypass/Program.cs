using System;
using System.IO;
using System.Linq;
using System.Net;

namespace LinkvertiseBypass
{
    internal static class Program
    {
        private static void Main()
        {
            Console.Clear();
            Console.WriteLine("Enter a vaild link: ");
            string link = Console.ReadLine();

            string[] pastterns = { "https://linkvertise.com/", "https://up-to-down.net/", "https://link-to.net/", "https://direct-link.net/", "https://file-link.net" };
            if (!pastterns.Any(x => link.StartsWith(x)))
            {
                Console.WriteLine("The given input was not a vaild Linkvertise link,\nPress any key to restart.");
                Console.ReadKey();
                Main();
            }

            Console.Clear();
            Linkvertise object_ = Bypass(link);
            Console.WriteLine($"Query: {object_.Query}" +
                              $"\nDestination: {object_.Destination}" +
                              $"\nTime: {object_.Time} ms." +
                              $"\nCache TTL: {object_.Cache_ttl}" +
                              $"\nPlugin: {object_.Plugin}");

            Console.ReadLine();
        }

        private static Linkvertise Bypass(string URL)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"https://bypass.bot.nu/bypass2?url={URL}");
            request.Referer = "https://thebypasser.com";
            request.Headers.Add("DNT", "1");
            request.Accept = "*/*";
            try
            {
                Linkvertise result = new Linkvertise(new StreamReader(((HttpWebResponse)request.GetResponse()).GetResponseStream()).ReadToEnd());
                return result.Success ? result : throw new Exception("There was an error.");
            }
            catch (WebException ex) { throw ex; }
        }
    }

    public class Linkvertise
    {
        public string Destination;
        public string Query;
        public bool Success;
        public int Time;
        public long Cache_ttl;
        public string Plugin;

        public Linkvertise(string Response)
        {
            string[] parameters = Response.Replace("{", "").Replace("}", "").Split(',');
            foreach (string field in parameters)
            {
                int fieldLength = field.IndexOf('"', 2) - 2;
                string fieldValue = field.Substring(field.IndexOf(':') + 2).Replace("\"", "");

                switch (field.Substring(2, fieldLength))
                {
                    case "success":
                        this.Success = fieldValue.ToLower().Contains("fal") ? false : true;
                        break;

                    case "destination":
                        this.Destination = fieldValue;
                        break;

                    case "uery":
                        this.Query = fieldValue;
                        break;

                    case "time_ms":
                        this.Time = int.Parse(fieldValue);
                        break;

                    case "cache_ttl":
                        this.Cache_ttl = long.Parse(fieldValue);
                        break;

                    case "plugin":
                        this.Plugin = fieldValue;
                        break;
                }
            }
        }
    }
}