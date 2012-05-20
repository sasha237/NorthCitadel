/*
AMF.NET - an open-source .NET alternative to Macromedia's .NET Flash Remoting
Copyright (C) 2006 Karl Seguin - http://www.openmymind.net/

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/
using System;
using System.Web;
using System.Web.SessionState;
using log4net;

namespace Fuel.AmfNet
{
   public class GatewayHandler : IHttpHandler, IRequiresSessionState
   {
      private static readonly ILog log = LogManager.GetLogger(typeof(GatewayHandler));
      
      public void ProcessRequest(HttpContext context)
      {
         context.Response.Buffer = true;

         Message message = null;         
         try
         {
            Deserializer deserializer = new Deserializer(context.Request.InputStream);
            message = deserializer.Deserialize();           
         }
         catch (Exception ex)
         {
            log.Error("An error occured will deserializing the AMF stream", ex);
            throw;
         }
         foreach (Body body in message.Bodies)
         {
            Response response = new Response();
            try
            {
               response.Data = body.Invoke();
               response.ReplyMethod = body.Target + "/onResult";
            }
            catch (Exception ex)
            {
               log.Error("An error occured while executing the package", ex);
               response.Data = new AmfError(ex);
               response.ReplyMethod = body.Target + "/onStatus";
            }
            message.Response.Add(response);
         }
         context.Response.ContentType = "application/x-amf";         
         try
         {
            Serializer serializer = new Serializer(context.Response.OutputStream);
            serializer.Serialize(message);            
         }
         catch (Exception ex)
         {
            log.Error("An error occured will serializing to the AMF stream", ex);
            throw;
         }
         context.Response.Flush();
         context.Response.End();
      }

      public bool IsReusable
      {
         get { return true; }
      }
   }
}