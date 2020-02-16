using System;

namespace Assingment2AA
{
    public class UriDepth
    {
        public Uri Uri { get; set; }
        public int Depth { get; set; }

        public UriDepth(Uri uri, int depth)
        {
            Uri = uri;
            Depth = depth;
        }
    }
}