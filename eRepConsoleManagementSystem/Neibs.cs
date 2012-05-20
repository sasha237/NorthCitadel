using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace eRepConsoleManagementSystem
{
    public class Pair<T>
    {
        public Pair()
        {
        }

        public Pair(T first, T second)
        {
            this.First = first;
            this.Second = second;
        }

        public T First { get; set; }
        public T Second { get; set; }
    };

    public class Pair2<T, U>
    {
        public Pair2()
        {
        }

        public Pair2(T first, U second)
        {
            this.First = first;
            this.Second = second;
        }

        public T First { get; set; }
        public U Second { get; set; }
    };
    class Neibs
    {
        public Pair<int> m_iInd;
        public Hashtable m_neibs;
        public Neibs()
        {
            m_neibs = new Hashtable();
            m_iInd = new Pair<int>(0, 0);
        }
        public Neibs(Pair<int> pp)
        {
            m_iInd = pp;
            m_neibs = new Hashtable();
            m_neibs[0] = new Pair<int>(pp.First, pp.Second - 1);
            m_neibs[1] = new Pair<int>(pp.First + 1, pp.Second + (pp.First + 1) % 2 - 1);
            m_neibs[2] = new Pair<int>(pp.First + 1, pp.Second + (pp.First + 1) % 2 );
            m_neibs[3] = new Pair<int>(pp.First, pp.Second + 1);
            m_neibs[4] = new Pair<int>(pp.First - 1, pp.Second + (pp.First + 1) % 2);
            m_neibs[5] = new Pair<int>(pp.First - 1, pp.Second + (pp.First + 1) % 2 - 1);
        }
    }
}
