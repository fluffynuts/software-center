using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace software_center
{
    public interface IAutoConverter
    {
        bool TryConvert(string data, Type toType, out object result);
    }

    public class AutoConverter
        : IAutoConverter
    {
        public bool TryConvert(string data, Type toType, out object result)
        {
            try
            {
                result = Convert(data, toType);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        public object Convert(string value, Type toType)
        {
            try
            {
                return System.Convert.ChangeType(value, toType);
            }
            catch
            {
                var culturedParser = TryFindCulturedParseMethodFor(toType);
                if (culturedParser != null)
                {
                    return TryParseCultured(culturedParser, value, toType);
                }

                var nativeParser = TryFindParseMethodFor(toType);
                if (nativeParser != null)
                {
                    return nativeParser.Invoke(null, new object[] { value });
                }

                // TODO: add implicit conversion
                throw new Exception($"No known conversion found from '{value ?? "(null)"}' to {toType}");
            }
        }

        private static MethodInfo TryFindParseMethodFor(Type type)
        {
            if (NativeParsers.TryGetValue(type, out var handler))
            {
                return handler;
            }

            var method = FindMethodOnType(type, "Parse", typeof(string));
            NativeParsers.TryAdd(type, method);
            return method;
        }

        private static readonly ConcurrentDictionary<Type, MethodInfo> NativeParsers = new();


        private static object TryParseCultured(
            MethodInfo culturedParser,
            string value,
            Type type)
        {
            foreach (var culture in TryCultures)
            {
                try
                {
                    return culturedParser.Invoke(
                        null,
                        new object[] { value, culture }
                    );
                }
                catch
                {
                    /* suppress errors, try next culture */
                }
            }

            throw new ArgumentException($"Unable to convert '{value}' to type {type}");
        }

        private static readonly CultureInfo[] TryCultures =
        {
            CultureInfo.InvariantCulture,
            CultureInfo.CurrentCulture,
            CultureInfo.DefaultThreadCurrentCulture
        };


        private static MethodInfo TryFindCulturedParseMethodFor(Type type)
        {
            if (NativeCulturedParsers.TryGetValue(type, out var handler))
            {
                return handler;
            }

            var method = FindMethodOnType(type, "Parse", typeof(string), typeof(IFormatProvider));
            NativeCulturedParsers.TryAdd(type, method);
            return method;
        }

        private static readonly ConcurrentDictionary<Type, MethodInfo> NativeCulturedParsers = new();

        private static MethodInfo FindMethodOnType(
            Type type,
            string methodName,
            params Type[] requiredParameterTypes)
        {
            return type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .FirstOrDefault(mi =>
                {
                    if (mi.Name != methodName)
                    {
                        return false;
                    }

                    var parameterTypes = mi.GetParameters()
                        .Select(p => p.ParameterType)
                        .ToArray();
                    if (parameterTypes.Length != requiredParameterTypes.Length)
                    {
                        return false;
                    }

                    return requiredParameterTypes.Zip(
                            parameterTypes,
                            Tuple.Create
                        )
                        .Aggregate(
                            true,
                            (acc, cur) => acc && cur.Item1 == cur.Item2
                        );
                });
        }
    }
}