using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder {

    public struct ValueWithIndex<T> {
        public int Index;
        public T Value;

        public ValueWithIndex(int index, T value) {
            Index = index;
            Value = value;
        }
    }

    public static class Utils {

        public static IEnumerable<ValueWithIndex<T>> WithIndex<T>(this IEnumerable<T> values) {
            int index = 0;
            foreach(T value in values)
            {
                yield return new ValueWithIndex<T>(index, value);
                index++;
            }
        }

    }
}
