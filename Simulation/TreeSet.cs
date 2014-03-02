using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    class TreeSet<T> : System.Collections.Generic.SortedList<T, int>
    {
        private int Counter;
        public TreeSet()
        {
            Counter = 0;
        }

        public void Add(T item)
        {
            this.Add(item, Counter++);
        }

        public T Pop()
        {
            T item = this.Keys[0];
            this.RemoveAt(0);
            return item;
        }

        public override string ToString()
        {
            return "\n\t"+ String.Join(", \n\t", this.Keys.ToArray());

        }
    }
}
