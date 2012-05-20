using System;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;


namespace SimpleDOMParserCSharp
{
   /*
    * @(#)SimpleDOMParser.java
    *Translated to C# from Java by A. Russell Jones
    * used by permission http://www.SunwestTek.com
    */
   /**
    * <code>SimpleDOMParser</code> is a highly-simplified XML DOM
    * parser.
    */
   public class SimpleDOMParser
   {
      private XmlTextReader Reader;
      private Stack elements;
      private SimpleElement currentElement;
      private SimpleElement rootElement;
      public SimpleDOMParser() 
      {
         elements = new Stack();
         currentElement = null;
      }

      public SimpleElement parse(XmlTextReader reader) 
      {
         SimpleElement se = null;
         this.Reader = reader;
         while (!Reader.EOF)
         {
            Reader.Read();            
            switch (Reader.NodeType)
            {
               case XmlNodeType.Element :
                  // create a new SimpleElement
                  se = new SimpleElement(Reader.LocalName);
                  currentElement = se;                  
                  if (elements.Count == 0) 
                  {
                     rootElement = se;
                     elements.Push(se);
                  }
                  else 
                  {                  
                     SimpleElement parent = (SimpleElement) elements.Peek();
                     parent.ChildElements.Add(se);

                     // don['t push empty elements onto the stack
                     if (Reader.IsEmptyElement) // ends with "/>"
                     {
                        break;
                     }
                     else {
                        elements.Push(se);
                     }
                  }
                  if (Reader.HasAttributes) 
                  {
                     while(Reader.MoveToNextAttribute()) 
                     {
                        currentElement.setAttribute(Reader.Name,Reader.Value);
                     }
                  }
                  break;
               case XmlNodeType.Attribute :
                  se.setAttribute(Reader.Name,Reader.Value);
                  break;
               case XmlNodeType.EndElement :
                  //pop the top element 
                  elements.Pop();
                  break;
               case XmlNodeType.Text :
                  currentElement.Text=Reader.Value;
                  break;
               case XmlNodeType.CDATA :
                  currentElement.Text=Reader.Value;
                  break;
               default :
                  // ignore
                  break;
            }
         }
         return rootElement;
      }
   }
}
