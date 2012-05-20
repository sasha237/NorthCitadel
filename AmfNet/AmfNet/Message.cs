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
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace Fuel.AmfNet
{
    public class Message
   {
      #region Fields and Properties
      private UInt16 _version = 3;
      private Collection<Header> _headers;
      private Collection<Body> _bodies;
      private List<Response> _response;

      public List<Response> Response
      {
         get 
         { 
            if (_response == null)
            {
               _response = new List<Response>();
            }
            return _response;
         }
      }
      public UInt16 Version
      {
         get
         {
            return _version;
         }
         internal set
         {
            _version = value;
         }
      }
      public Collection<Header> Headers
      {
         get
         {
            if (_headers == null)
            {
               _headers = new Collection<Header>();
            }
            return _headers;
         }
      }
      public Collection<Body> Bodies
      {
         get
         {
            if (_bodies == null)
            {
               _bodies = new Collection<Body>();
            }
            return _bodies;
         }
      }
      #endregion
   }
}