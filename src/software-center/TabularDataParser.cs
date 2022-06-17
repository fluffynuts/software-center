using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace software_center
{
    public class TabularDataParser : IParser
    {
        private readonly IAutoConverter _autoConverter;
        private readonly IHeaderParser _headerParser;

        public TabularDataParser(
            IHeaderParser headerParser,
            IAutoConverter autoConverter
        )
        {
            _autoConverter = autoConverter;
            _headerParser = headerParser;
        }

        public IEnumerable<T> Parse<T>(string data)
            where T : class, new()
        {
            var lines = data.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length < 2)
            {
                yield break;
            }

            var headers = _headerParser.Parse(lines[0]);
            foreach (var line in lines.Skip(2))
            {
                var current = ParseLine<T>(line, headers);
                if (current is not null)
                {
                    yield return current;
                }
            }
        }

        private T ParseLine<T>(string line, HeaderCollection headers)
            where T : class, new()
        {
            if (!LineCanBeMappedFor(line, headers))
            {
                return null;
            }

            var result = new T();
            var propMap = GeneratePropertyMap(headers.Headers, typeof(T));
            foreach (var header in headers.Headers)
            {
                var propInfo = propMap[header.Name];
                if (propInfo is null)
                {
                    continue;
                }

                var data = line.Substring(
                    header.Offset,
                    (header.Next?.Offset ?? line.Length) - header.Offset
                ).Trim();

                if (_autoConverter.TryConvert(data, propInfo.PropertyType, out var propData))
                {
                    propInfo.SetValue(
                        result,
                        propData
                    );
                }
            }

            return result;
        }

        private bool LineCanBeMappedFor(string line, HeaderCollection headers)
        {
            if (string.IsNullOrEmpty(line))
            {
                return false;
            }

            if (line.Length <= headers.MaxOffset)
            {
                return false;
            }

            // tabular data is expected to have at least a space before the start
            // of a column
            return headers.Headers.Skip(1).Aggregate(
                true,
                (acc, cur) => acc && line[cur.Offset - 1] == ' '
            );
        }

        private static Dictionary<Type, Dictionary<string, PropertyInfo>> HeaderMapCache
            = new();

        private static Dictionary<string, PropertyInfo> GeneratePropertyMap(
            Header[] headers,
            Type target
        )
        {
            if (HeaderMapCache.TryGetValue(target, out var result))
            {
                return result;
            }

            var targetProps = target.GetProperties();
            result = headers.Select(h => new
            {
                Key = h.Name,
                Value = targetProps.FirstOrDefault(pi => pi.Name.ToLower() == h.Name.ToLower())
            }).ToDictionary(o => o.Key, o => o.Value, StringComparer.OrdinalIgnoreCase);
            HeaderMapCache[target] = result;

            return result;
        }
    }
}