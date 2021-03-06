﻿using System;
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
        private Dictionary<string, string[]> _permissions;

        public WebCrawler(int maxSeaches, string urlStr, int maxDepth)
        {
            this.maxSeaches = maxSeaches;
            this.urlStr = urlStr;
            this.maxDepth = maxDepth;
            count = 0;
            allLinks = new List<string>();
            frontier = new Queue<UriDepth>();
            myWebOfLinks = new Dictionary<string, bool>();
            _permissions = new Dictionary<string, string[]>();
        }
        
        public void Crawl()
        {
            UriBuilder ub = new UriBuilder(urlStr);
            frontier.Enqueue(new UriDepth(ub.Uri, 0));
            while (frontier.Any())
            {
                UriDepth localUriDepth = frontier.Dequeue();
                if (!CheckIfNewHostUrl(localUriDepth.Uri.Host))
                { 
                    ReadRobotsTxt(localUriDepth.Uri.Host);
                }
                try
                {
                    WebClient wc = new WebClient();
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
                                if (!CheckIfNewHostUrl(uri.Host))
                                {
                                    ReadRobotsTxt(uri.Host);
                                }

                                if (CheckPermission(uri))
                                {
                                    myWebOfLinks.Add(newUrl, true);
                                    frontier.Enqueue(new UriDepth(uri, localUriDepth.Depth + 1));
                                }
                                else
                                {
                                    myWebOfLinks.Add(newUrl, false);
                                    Console.WriteLine("Fuck it not allowed: " + newUrl);
                                }
                                
                                
                            }
                            catch (Exception e)
                            {

                                myWebOfLinks.Add(newUrl, false);
                                Console.WriteLine("Fuck it broken: " + newUrl);
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

        public void DoesDisallowWork()
        {
            //fuck it not work this 
            foreach (var url in myWebOfLinks.Keys)
            {
                if (myWebOfLinks[url])
                {
                    UriBuilder ub = new UriBuilder(url);
                    if (!CheckPermission(ub.Uri))
                    {
                        Console.WriteLine(url + " is not allowed but visited anyway");
                    }
                }
                
            }
        }

        private bool CheckIfNewHostUrl(string hostURL)
        {
            foreach (var url in _permissions.Keys)
            {
                if (url.Equals(hostURL))
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckPermission(Uri uri)
        {
            foreach (var noNo in _permissions[uri.Host])
            {
                int start = uri.AbsoluteUri.IndexOf("/".ToCharArray()[0]) + 2;
                string notAllowed = ( uri.Host + noNo.Substring(10));
                string current = uri.AbsoluteUri.Substring(start);
                if (current.Equals(notAllowed))
                {
                    return false;
                }
            }
            return true;
        }
        private void ReadRobotsTxt(string hostUrl)
        {
            try
            {
                WebClient wc = new WebClient();
                string wholeFile = wc.DownloadString("http://" + hostUrl + "/robots.txt");
                string[] array = wholeFile.Split("\n");
                List<string> arrayInProgress = new List<string>();
                for (int i = 0; i < array.Length; i++)
                {
                    if (array[i].Length >= 10)
                    {
                        if (array[i].Substring(0, 9).Equals("Disallow:"))
                        {
                            arrayInProgress.Add(array[i]);
                        }
                    }
                }
                _permissions[hostUrl] = arrayInProgress.ToArray();
            }
            catch (Exception e)
            {
                //The Website has no robots.txt
                _permissions[hostUrl] = new string[0];
            }
            

        }
    }
}