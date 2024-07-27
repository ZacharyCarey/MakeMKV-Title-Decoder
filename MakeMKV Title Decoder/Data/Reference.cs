using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeMKV_Title_Decoder {

    // A convenience structure to use instead of const management of indices
    public readonly struct Ref<T> : IEquatable<Ref<T>> {
        public static Ref<T> Null => new();

        public readonly List<T>? Storage;
        public readonly int Index;
        public T Value { get => Storage[Index]; set => Storage[Index] = value; }

        /// <summary>
        /// Returns true if a valid pointer. Valid means there is a valid storage assigned, and the index is within it's bounds.
        /// </summary>
        public bool Valid { get => this.Storage != null && this.Index >= 0 && this.Index < Storage.Count; }

        public Ref() {
            this.Storage = null;
            this.Index = -1;
        }

        public Ref(List<T>? array) {
            // C style constructor where pointing to the array points to the first object
            this.Storage = array;
            this.Index = 0;
        }

        public Ref(List<T>? array, int index) {
            this.Storage = array;
            this.Index = index;
        }

        public T this[int offset] {
            get => Storage[this.Index + offset];
            set => Storage[this.Index + offset] = value;
        }

        public bool Equals(Ref<T> other) {
            return this == other;
        }

        public static Ref<T> operator ++(Ref<T> a) => new Ref<T>(a.Storage, a.Index + 1);
        public static Ref<T> operator --(Ref<T> a) => new Ref<T>(a.Storage, a.Index - 1);
        public static Ref<T> operator +(Ref<T> left, int right) => new Ref<T>(left.Storage, left.Index + right);
        public static Ref<T> operator +(int left, Ref<T> right) => new Ref<T>(right.Storage, left + right.Index);
        public static Ref<T> operator -(Ref<T> left, int right) => new Ref<T>(left.Storage, left.Index - right);
        public static Ref<T> operator -(int left, Ref<T> right) => new Ref<T>(right.Storage, left - right.Index);
        public static bool operator ==(Ref<T> left, Ref<T> right) {
            bool leftValid = left.Valid;
            bool rightValid = right.Valid;

            if (leftValid && rightValid)
            {
                return (left.Storage == right.Storage) && (left.Index == right.Index);
            } else if (leftValid == false && rightValid == false)
            {
                return true;
            } else
            {
                return false;
            }
        }
        public static bool operator !=(Ref<T> left, Ref<T> right) => !(left == right);
        public static bool operator ==(Ref<T> left, int right) => (left.Index == right);
        public static bool operator ==(int left, Ref<T> right) => (left == right.Index);
        public static bool operator !=(Ref<T> left, int right) => (left.Index != right);
        public static bool operator !=(int left, Ref<T> right) => (left != right.Index);
        public static bool operator <(Ref<T> left, int right) => (left.Index < right);
        public static bool operator <(int left, Ref<T> right) => (left < right.Index);
        public static bool operator >(Ref<T> left, int right) => (left.Index > right);
        public static bool operator >(int left, Ref<T> right) => (left > right.Index);
        public static bool operator <=(Ref<T> left, int right) => (left.Index <= right);
        public static bool operator <=(int left, Ref<T> right) => (left <= right.Index);
        public static bool operator >=(Ref<T> left, int right) => (left.Index >= right);
        public static bool operator >=(int left, Ref<T> right) => (left >= right.Index);

        public override string ToString() {
            if (this.Valid)
            {
                return $"Ptr[{this.Index}]";
            } else
            {
                return $"Ptr[null]";
            }
        }

        public override bool Equals(object? obj) {
            if (obj == null)
            {
                // I assume this != null, so this can't be true
                return false;
            } else if (obj is int index)
            {
                return this == index;
            } else if (obj is Ref<T> other)
            {
                return this == other;
            } else
            {
                return false;
            }
        }

        public override int GetHashCode() {
            return this.Index.GetHashCode();
        }
    }

    public static class RefUtils {
        public static IEnumerable<Ref<T>> AsPointers<T>(this List<T> storage) {
            for(int i = 0; i < storage.Count; i++)
            {
                yield return new Ref<T>(storage, i);
            }
        }
    }
}
