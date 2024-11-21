using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class LinqUtils
    {
        public static IEnumerable<(int Index, T Value)> WithIndex<T>(this IEnumerable<T> values)
        {
            int index = 0;
            foreach (T value in values)
            {
                yield return (index, value);
                index++;
            }
        }
    }
}
