
using System;

namespace NerZul.Core.Utils
{
	public class StringSelector
	{
		private string[] m_List = null;
		System.Random m_Random=new System.Random();
		public StringSelector (string[] list)
		{
			if(list==null) throw new ArgumentNullException();
			m_List=list;
		}
		public StringSelector(string Path)
		{
            try
            {
                m_List = System.IO.File.ReadAllLines(Path);
            }
            catch (Exception e)
            {
                ConsoleLog.WriteLine("Error loading string list from " + Path + "\n" + e.Message);
            }
		}
		public string GetRandomString()
		{
            if (m_List.Length == 0)
            {
                throw new Exception("No proxies found");
            }
			return m_List[m_Random.Next(0, m_List.Length-1)];
		}
        public string GetString(int index)
        {
            if (index >= m_List.Length)
            {
                throw new Exception("Index out of range");
            }
            return m_List[index];
        }
        public int Count
        {
            get
            {
                return m_List.Length;
            }
        }
    }

	public class StringHolder
	{
		public readonly System.Collections.Generic.List<string> m_List;
		public StringHolder(System.Collections.Generic.IEnumerable<string> list)
		{
			m_List=new System.Collections.Generic.List<string>(list);
		}
		public StringHolder(string Path)
		{
			m_List=new System.Collections.Generic.List<string>(
				System.IO.File.ReadAllLines(Path));
		}
		public string GetNextString()
		{
			string rv=null;
			lock (m_List)
			{
				if(m_List.Count>0)
				{
					rv=m_List[0];
					m_List.RemoveAt(0);
				}
			}
			return rv;
		}
		public void Add(System.Collections.Generic.IEnumerable<string> list)
		{
			lock(m_List)
			{
				m_List.AddRange(list);
			}
		}
		public void Add(string item)
		{
			lock (m_List)
			{
				m_List.Add(item);
			}
		}
	}
	
}

