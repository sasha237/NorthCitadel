using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;

namespace SimpleDOMParserCSharp {
   public class SimpleElement {
      private String tagName;
      private String text;
      private StringDictionary attributes;
      private SimpleElements childElements;


      public SimpleElement(String tagName) {
         this.tagName = tagName;
         attributes = new StringDictionary();
         childElements = new SimpleElements();
         this.text="";
      }

      public String TagName {
         get {
            return tagName;
         }
         set {
            this.tagName = value;
         }
      }
      public string Text {
         get {
            return text;
         }
         set {
            this.text = value;
         }
      }

      public SimpleElements ChildElements {
         get {
            return this.childElements;
         }
      }
      public StringDictionary Attributes {
         get {
            return this.attributes;
         }
      }
      public String Attribute(String name) {
         return (String) attributes[name];
      }

      public void setAttribute(String name, String value) {
         attributes.Add(name, value);
      }
   }
}
