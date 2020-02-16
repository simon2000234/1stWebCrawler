using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Assingment2AA
{
    public class WebCrawler
    {
        private static readonly Regex urlTagPattern = new Regex(@"<a.*?href=[""'](?<url>.*?)[""'].*?>(?<name>.*?)</a>", RegexOptions.IgnoreCase);
        private static readonly Regex hrefPattern = new Regex("href\\s*=\\s*(?:\"(?<1>[^\"]*)\"|(?<1>\\S+))", RegexOptions.IgnoreCase);
        private int count;
        private int maxSeaches;
        private List<string> allLinks;
        private Queue<UriDepth> frontier;
        private Dictionary<string, bool> myWebOfLinks;
        private string urlStr;
        private int maxDepth;

        public WebCrawler(int maxSeaches, string urlStr, int maxDepth)
        {
            this.maxSeaches = maxSeaches;
            this.urlStr = urlStr;
            this.maxDepth = maxDepth;
            count = 0;
            allLinks = new List<string>();
            frontier = new Queue<UriDepth>();
            myWebOfLinks = new Dictionary<string, bool>();
        }

        public void Crawl()
        {
            UriBuilder ub = new UriBuilder(urlStr);
            frontier.Enqueue(new UriDepth(ub.Uri, 0));
            while (frontier.Any())
            {
                UriDepth localUriDepth = frontier.Dequeue();
                WebClient wc = new WebClient();
                try
                {
                    string webPage = wc.DownloadString(localUriDepth.Uri.ToString());


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
                        if (localUriDepth.Depth >= maxDepth)
                        {
                            //nothing
                        }
                        else if (isNewLink)
                        {
                            try
                            {
                                Uri.TryCreate(localUriDepth.Uri, newUrl, out var uri);
                                myWebOfLinks.Add(newUrl, true);
                                frontier.Enqueue(new UriDepth(uri, localUriDepth.Depth + 1));
                            }
                            catch (Exception e)
                            {

                                myWebOfLinks.Add(newUrl, false);
                                Console.WriteLine("Fuck it shit: " + newUrl);
                            }
                        }
                    }
                    allLinks.Add(localUriDepth.Uri.ToString());
                    count++;
                    Console.WriteLine(localUriDepth.Uri.ToString() + " Depth is: " + localUriDepth.Depth);
                    if (count >= maxSeaches)
                    {
                        frontier.Clear();
                        Console.WriteLine("Fuck iz Done");
                    }

                }
                catch (Exception e)
                {
                    myWebOfLinks[localUriDepth.Uri.ToString()] = false;
                }

            }
        }
    }
}