using System;
using System.Collections;

namespace SimpleDOMParserCSharp {
   /// <summary>
   /// Summary description for SimpleElements.
   /// </summary>
   public class SimpleElements : CollectionBase {
      public SimpleElements() {
         //
         // TODO: Add constructor logic here
         //
      }
      public void Add(SimpleElement se) {
         this.List.Add(se);
      }
      public SimpleElement Item(int index) {
         return (SimpleElement) this.List[index];
      }
      public Object[] ToArray() {
         Object[] ar = new Object[this.List.Count];
         for (int i=0; i < this.List.Count; i++) {
            ar[i] = this.List[i];
         }
         return ar;
      }
   }
}
