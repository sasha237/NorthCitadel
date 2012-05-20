using System;
using System.IO;  
using System.Text;
using System.Xml;
namespace SimpleDOMParserCSharp
{
	/// <summary>
	/// Summary description for main.
	/// </summary>
	public class main
	{
      static void Main()
      {
         int starttick;
         int endtick;
         int i;
         XmlTextReader rdr;
         StringBuilder sb;
         SimpleElement se = new SimpleElement("junk");
         SimpleDOMParser sdp;
         sb = new StringBuilder();
         try 
         {
            starttick = System.Environment.TickCount;
            for (i=0; i < 10; i++) 
            {
               rdr = new XmlTextReader(@"c:\rjones\temp\employees.xml");
               sdp = new SimpleDOMParser();
               se = sdp.parse(rdr);
               rdr.Close();
            }
            endtick = System.Environment.TickCount;
            sb = new StringBuilder();
            printTree(se, sb, 0);
            System.Diagnostics.Debug.WriteLine(sb.ToString());
            System.Diagnostics.Debug.WriteLine(endtick - starttick);
         }
         catch (Exception ex)
         {
            System.Diagnostics.Debug.WriteLine(ex.Message);
         }
      }
      private static void printTree(SimpleElement se, StringBuilder sb, int depth) 
      {
         sb.Append(new string('\t',depth) +  "<" + se.TagName);
         foreach (string attName in se.Attributes.Keys) {
            sb.Append(" " + attName + "=" + "\"" + se.Attribute(attName) + "\"");
         }
         sb.Append(">" + se.Text.Trim());
         if (se.ChildElements.Count > 0) 
         {
            sb.Append(System.Environment.NewLine);
            depth +=1;
            foreach(SimpleElement ch in se.ChildElements) 
            {
               //sb.Append(System.Environment.NewLine);
               printTree(ch, sb, depth);            
            }
         
            depth -= 1;
            sb.Append(new string('\t',depth) + "</" + se.TagName + ">" + System.Environment.NewLine);
         }    
         else 
         {
            sb.Append("</" + se.TagName + ">" + System.Environment.NewLine);
         }
      }
	}
}
