using System;

namespace Fuel.AmfNet
{
   public class AmfError
   {
      #region Fields and Properties
      private string _level = "error";
      private string _code = "SERVER.PROCESSING";
      private string _details;
      private string _description;
      private string _type;

      public string Type
      {
         get
         {
            return _type;
         }
      }
      public string Code
      {
         get
         {
            return _code;
         }
      }
      public string Description
      {
         get
         {
            return _description;
         }
      }
      public string Details
      {
         get
         {
            return _details;
         }
      }

      public string Level
      {
         get
         {
            return _level;
         }
      }

      #endregion

      #region Constructors
      public AmfError(Exception ex)
      {
         Exception rootCause;
         for (rootCause = ex; rootCause.InnerException != null; rootCause = rootCause.InnerException)
            ;
         _details = rootCause.StackTrace;
         _description = rootCause.Message;
         _type = rootCause.GetType().FullName;
      }
      #endregion
   }
}
