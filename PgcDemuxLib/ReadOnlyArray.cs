using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PgcDemuxLib
{
    public struct ReadOnlyArray<T> 
        : IEnumerable<T>
        , IReadOnlyCollection<T>
        , IReadOnlyList<T>
        , IEnumerable
    {
        [JsonInclude]
        public int Length => Values.Length;

        [JsonInclude]
        T[] Values;

        [JsonIgnore]
        public int Count => Values.Length;


        public T this[int index] { 
            get => Values[index]; 
            internal set => Values[index] = value; 
        }

        public ReadOnlyArray()
        {
            Values = Array.Empty<T>();
        }

        public ReadOnlyArray(int size)
        {
            Values = new T[size];
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)Values).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Values.GetEnumerator();
        }
    }
}
