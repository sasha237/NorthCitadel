using System;
using System.Collections.Generic;

namespace NerZul.Core.Utils
{
    public static class CommonUtils
    {
        public static List<E> MixList<E>(List<E> inputList)
        {
            List<E> randomList = new List<E>();

            Random r = new Random();
            int randomIndex = 0;
            while (inputList.Count > 0)
            {
                randomIndex = r.Next(0, inputList.Count); //Choose a random object in the list
                randomList.Add(inputList[randomIndex]); //add it to the new, random list
                inputList.RemoveAt(randomIndex); //remove to avoid duplicates
            }

            return randomList; //return the new random list
        }

        public static string GetStringBetween(string source, string start, string end)
        {
            return GetStringBetween(source, start, end, 0, 1);
        }

        public static string GetStringBetween(string source, string start, string end, int startIndex, int count)
        {
            int ipos = startIndex-1;
            for (int i = 0; i < count; i++)
            {
                ipos++;
                ipos = source.IndexOf(start, ipos);
            }

            string ss = source.Substring(ipos + start.Length, source.Length - (ipos + start.Length));
            return ss.Substring(0, ss.IndexOf(end));
        }

        public static string GetStringFrom(string source, string start)
        {
            int ipos = source.IndexOf(start, 0);
            return source.Substring(ipos + start.Length, source.Length - (ipos + start.Length));
        }

        public static string GetToken(string source)
        {
            return CommonUtils.GetStringBetween(
                source, 
                "[_csrf_token]\" value=\"", 
                "\" ");
        }

        public static string GetToken2(string source)
        {
            return CommonUtils.GetStringBetween(
                source,
                "'_csrf_token'); m.setAttribute('value', '", 
                "'");
        }
    }
}
