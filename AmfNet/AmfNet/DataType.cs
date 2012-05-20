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

namespace Fuel.AmfNet
{
   public enum DataType
   {
      Number = 0,
      Boolean = 1,
      String = 2,
      UntypedObject = 3,
      MovieClip = 4,
      Null = 5,
      Undefined = 6,
      ReferencedObject = 7,
      MixedArray = 8,
      End = 9,
      Array = 10,
      Date = 11,
      LongString = 12,
      TypeAsObject = 13,
      Recordset = 14,
      Xml = 15,
      TypedObject = 16
   }
}