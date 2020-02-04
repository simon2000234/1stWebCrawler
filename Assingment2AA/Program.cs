using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Assingment2AA
{
    class Program
    {
        static void Main(string[] args)
        {
            int count = 0;
            int maxSeaches;
            List<string> allLinks = new List<string>();
            Queue<Uri> frontier = new Queue<Uri>();
            Dictionary<string, bool> myWebOfLinks = new Dictionary<string, bool>();
            Console.WriteLine("Website to crawl:");
            string urlStr = Console.ReadLine();
            Console.WriteLine("Number of Searches:");
            maxSeaches = int.Parse(Console.ReadLine());
            UriBuilder ub = new UriBuilder(urlStr);
            frontier.Enqueue(ub.Uri);
            while (frontier.Any())
            {
                Uri localUri = frontier.Dequeue();
                WebClient wc = new WebClient();
                try
                {
                    string webPage = wc.DownloadString(localUri.ToString());

                    var urlTagPattern = new Regex(@"<a.*?href=[""'](?<url>.*?)[""'].*?>(?<name>.*?)</a>",
                        RegexOptions.IgnoreCase);
                    var hrefPattern = new Regex("href\\s*=\\s*(?:\"(?<1>[^\"]*)\"|(?<1>\\S+))", RegexOptions.IgnoreCase);

                    var urls = urlTagPattern.Matches(webPage);
                    foreach (Match url in urls)
                    {

                        string newUrl = hrefPattern.Match(url.Value).Groups[1].Value;
                        bool isNewLink = true;
                        foreach (var key in myWebOfLinks.Keys)
                        {
                            if (key.Equals(newUrl))
                            {
                                isNewLink = false;
                            }
                        }
                        if (isNewLink)
                        {
                            try
                            {
                                Uri.TryCreate(localUri, newUrl, out var uri);
                                myWebOfLinks.Add(newUrl, true);
                                frontier.Enqueue(uri);
                            }
                            catch (Exception e)
                            {

                                myWebOfLinks.Add(newUrl, false);
                                Console.WriteLine("Fuck it shit: " + newUrl);
                            }
                        }

                    }
                    allLinks.Add(localUri.ToString());
                    count++;
                    if (count >= maxSeaches)
                    {
                        frontier.Clear();
                        Console.WriteLine("Listing All Links:");
                    }

                }
                catch (Exception e)
                {
                    myWebOfLinks[localUri.ToString()] = false;
                }
                
            }
            foreach (var url in allLinks)
            {
                Console.WriteLine(url);
            }

        }
    }
}
