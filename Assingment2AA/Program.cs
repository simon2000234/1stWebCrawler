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
            Console.WriteLine("Website to crawl:");
            string urlStr = Console.ReadLine();
            Console.WriteLine("Number of Searches:");
            int maxSeaches = int.Parse(Console.ReadLine());
            Console.WriteLine("Max Depth:");
            
            int maxDepth = int.Parse(Console.ReadLine());
            WebCrawler wc = new WebCrawler(maxSeaches, urlStr, maxDepth);
            wc.Crawl();
        }
    }
}
