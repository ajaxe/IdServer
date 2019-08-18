using System;
using System.Collections.Generic;

namespace ApogeeDev.IdServer.Helpers.Utilities
{
    public class GenericEqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, object> _fieldSelector;

        public GenericEqualityComparer(Func<T, object> fieldSelector)
        {
            _fieldSelector = fieldSelector;
        }
        public bool Equals(T x, T y)
        {
            var first = _fieldSelector.Invoke(x);
            var sec = _fieldSelector.Invoke(y);
            if (first != null && first.Equals(sec))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(T obj)
        {
            return _fieldSelector.Invoke(obj).GetHashCode();
        }
    }
}