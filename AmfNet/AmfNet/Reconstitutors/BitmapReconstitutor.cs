using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

namespace Fuel.AmfNet
{
   public class BitmapReconstitutor : IReconstitutor
   {
      #region IReconstitutor Members
      public string RegisteredName
      {
         get { return "SerializedBitmap"; }
      }
      public bool ParametersAreValid(Hashtable parameters)
      {
         if (parameters == null)
         {
            return false;
         }
         if (!parameters.ContainsKey("__width") || !parameters.ContainsKey("__height") || !parameters.ContainsKey("__pixels"))
         {
            return false;
         }
         return true;
      }
      public object Reconstitute(Hashtable parameters)
      {
         int width, height;
         bool isTransparent;
         if (!Int32.TryParse(parameters["__width"].ToString(), out width))
         {
            return null;
         }
         if (!Int32.TryParse(parameters["__height"].ToString(), out height))
         {
            return null;
         }
         if (!bool.TryParse(parameters["__transparent"].ToString(), out isTransparent))
         {
            return null;
         }         
         string[] pixelData = parameters["__pixels"].ToString().Split(',');

         Bitmap bm = new Bitmap(width, height);       
         int x = -1;
         int y = 0;
         foreach(string data in pixelData)
         {            
            string[] split = data.Split('|');
            if (split.Length < 3)
            {
               continue;
            }
            
            int repeatCount = Convert.ToInt32(split[0]);          
            Color color = ColorTranslator.FromHtml("#" + Convert.ToInt32(split[1]).ToString("X6"));
            if (isTransparent)
            {
               color = Color.FromArgb(Convert.ToInt32(split[2]), color);
            }            
            for (int i = 0; i < repeatCount; ++i)
            {
               ++x;
               if (x == width)
               {
                  x = 0;
                  ++y;
               }
               bm.SetPixel(x, y, color);
            }
         }
         return bm;         
      }
      #endregion
   }
}