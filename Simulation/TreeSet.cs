using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation
{
    class TreeSet<T> : System.Collections.Generic.SortedList<int, T>
    {
        private int Counter;
        public TreeSet()
        {
            Counter = 0;
        }
        public void Insert(T item)
        {
            this.Add(Counter++, item);
        }

        public T Pop()
        {
            int key = this.Keys[0];
            T item = this.Values[0];
            this.Remove(key);
            
            return item;
        }

        public override string ToString()
        {
            return "\n\t"+ String.Join(", \n\t", this.ToArray());

        }
    }
}
