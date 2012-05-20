using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace eRepConsoleManagementSystem
{
    class UnitsCountsByType
    {
        public int m_iHelicopter;
        public int m_iTank;
        public int m_iArtillery;
        public int m_iRifle;
        public int m_iCountry;
        public UnitsCountsByType()
        {
            m_iHelicopter = 0;
            m_iTank = 0;
            m_iRifle = 0;
            m_iArtillery = 0;
            m_iCountry = 0;
        }
        public UnitsCountsByType(ArrayList users)
        {
            m_iHelicopter = 0;
            m_iTank = 0;
            m_iRifle = 0;
            m_iArtillery = 0;
            m_iCountry = 0;
            if (users==null)
                return;
            foreach (var us in users)
            {
                ArrayList uc = us as ArrayList;
                switch(int.Parse(uc[8].ToString()))
                {
                    case 0:
                    case 6:
                        m_iRifle++;
                        break;
                    case 7: 
                        m_iTank++;
                        break;
                    case 8:
                        m_iArtillery++;
                        break;
                    case 9:
                        m_iHelicopter++;
                        break;
                    default:
                        break;
                }
                m_iCountry = int.Parse(uc[4].ToString());
            }
        }
    }
}
