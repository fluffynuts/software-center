using System.Collections.Generic;

namespace software_center
{
    public interface IParser
    {
        IEnumerable<T> Parse<T>(string data) where T: class, new();
    }
}