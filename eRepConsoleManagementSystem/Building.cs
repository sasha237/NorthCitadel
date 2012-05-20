using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace eRepConsoleManagementSystem
{
    class Building
    {
        public int m_iType;
        public int m_iArea;
        public int m_iQuality;
        public int m_iValue;
        public int m_iDurability;
        public Building()
        {
            m_iType = 0;
            m_iArea = 0;
            m_iQuality = 0;
            m_iValue = 0;
            m_iDurability = 0;
        }
        public Building(ArrayList arr)
        {
            m_iType = int.Parse(arr[0].ToString());
            m_iArea = int.Parse(arr[1].ToString());
            m_iQuality = int.Parse(arr[2].ToString());
            m_iValue = int.Parse(arr[3].ToString());
            m_iDurability = int.Parse(arr[4].ToString());
        }
    }
}
