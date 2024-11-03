using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PgcDemuxLib
{
    public class IndexedProperty<TIndex, TValue>
    {
        readonly Action<TIndex, TValue> SetAction;
        readonly Func<TIndex, TValue> GetFunc;

        public IndexedProperty(Func<TIndex, TValue> getFunc, Action<TIndex, TValue> setAction)
        {
            this.GetFunc = getFunc;
            this.SetAction = setAction;
        }

        public TValue this[TIndex i]
        {
            get
            {
                return GetFunc(i);
            }
            set
            {
                SetAction(i, value);
            }
        }
    }

    public class ReadOnlyIndexedProperty<TIndex, TValue>
    {
        readonly Func<TIndex, TValue> GetFunc;

        public ReadOnlyIndexedProperty(Func<TIndex, TValue> getFunc)
        {
            this.GetFunc = getFunc;
        }

        public TValue this[TIndex i]
        {
            get
            {
                return GetFunc(i);
            }
        }
    }

    public class WriteOnlyIndexedProperty<TIndex, TValue>
    {
        readonly Action<TIndex, TValue> SetAction;

        public WriteOnlyIndexedProperty(Action<TIndex, TValue> setAction)
        {
            this.SetAction = setAction;
        }

        public TValue this[TIndex i]
        {
            set
            {
                SetAction(i, value);
            }
        }
    }
}
