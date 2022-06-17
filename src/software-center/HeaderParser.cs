using System.Linq;
using System.Text.RegularExpressions;

namespace software_center
{
    public class HeaderCollection
    {
        public Header[] Headers { get; }
        public int MaxOffset { get; }

        public HeaderCollection(Header[] headers)
        {
            Headers = headers;
            MaxOffset = headers.Length > 0
                ? headers.Max(h => h.Offset)
                : 0;
        }
    }

    public interface IHeaderParser
    {
        HeaderCollection Parse(string line);
    }

    public class HeaderParser
        : IHeaderParser
    {
        private static readonly Regex WordSplitter = new("\\S+\\s*");

        public HeaderCollection Parse(string line)
        {
            var headers = WordSplitter.Matches(line)
                .Cast<Group>()
                .Select(m => new Header(m.Value.Trim(), m.Index))
                .ToArray();
            for (var i = 0; i < headers.Length - 1; i++)
            {
                headers[i].Next = headers[i + 1];
            }

            return new HeaderCollection(headers);
        }
    }
}