using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZW
{
    public class EnumerableEqualityComparer<V> : IEqualityComparer<List<V>>
    {
        public IEqualityComparer<V> ElementComparer { get; }

        public EnumerableEqualityComparer() : this(null)
        {

        }

        public EnumerableEqualityComparer(IEqualityComparer<V> elementComparer)
        {
            this.ElementComparer = elementComparer ?? EqualityComparer<V>.Default;
        }

        public bool Equals(List<V> x, List<V> y) => x.SequenceEqual(y);

        public int GetHashCode([DisallowNull] List<V> obj)
        {
            var hash = 17;

            foreach (var v in obj)
            {
                hash = hash * 31 + this.ElementComparer.GetHashCode(v);
            }

            return hash;
        }

    }

}
