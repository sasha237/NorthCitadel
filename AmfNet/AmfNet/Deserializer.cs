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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;


namespace Fuel.AmfNet
{
    public class Deserializer : BinaryReader
   {
      #region Fiels and Properties
        public static readonly DateTime BaseDate = new DateTime(1970, 1, 1);
      private static List<IReconstitutor> _reconstitutors;
      #endregion

      #region Constructors
      static Deserializer()
      {
         AmfNetConfiguration config = AmfNetConfiguration.GetConfig;
         if (config == null)
         {
            _reconstitutors = new List<IReconstitutor>(1);
         }
         else
         {
            _reconstitutors = new List<IReconstitutor>(config.Reconstitutors);
         }        
         _reconstitutors.Add(new BitmapReconstitutor());         
      }
      public Deserializer(Stream input) : base(input)
      {

      }
      #endregion

      #region Public Methods
      public Message Deserialize()
      {
         Message message = new Message();
         message.Version = ReadUInt16();
         int headerCount = ReadUInt16();
         for (int i = 0; i < headerCount; ++i)
         {
            string key = ReadShortString();
            bool required = ReadBoolean();
            int length = ReadInt32(); //doesn't seem necessary?
            object data = ReadData(ReadByte());
            message.Headers.Add(new Header(key, required, data));
         }
         int bodyCount = ReadUInt16();
         for (int i = 0; i < bodyCount; ++i)
         {
            string rawMethod = ReadShortString();
            int index = rawMethod.LastIndexOf('.');
            string method = rawMethod.Substring(index+1);
            if (index==-1)
            {
                index = 0;
            }
            string type = rawMethod.Substring(0, index);
            string target = ReadShortString();
            int length = ReadInt32();
            object data = ReadData(ReadByte());
            message.Bodies.Add(new Body(method, type, target, data));
         }
         return message;
      }

      #endregion

      #region Private Readers
      private object ReadData(byte type)
      {
         DataType t = (DataType) type;
         switch (t)
         {
            case DataType.Number:
               return ReadDouble();
            case DataType.Boolean:
               return ReadBoolean();
            case DataType.String:
               return ReadShortString();      
            case DataType.Array:
               return ReadArray();
            case DataType.Date:
               return ReadDate();
            case DataType.LongString:
               return ReadLongString();
            case DataType.TypedObject:
               return ReadTypedObject();            
            case DataType.MixedArray:
               return ReadDictionary();
            case DataType.Null:
            case DataType.Undefined:
            case DataType.End:
               return null;      
            case DataType.UntypedObject:
               return  ReadUntypedObject();
            case DataType.Xml:
               return ReadXmlDocument();
            case DataType.MovieClip:
            case DataType.ReferencedObject:            
            case DataType.TypeAsObject:               
            case DataType.Recordset:                         
               break;
         }
         return null;
      }
  
      private Hashtable ReadDictionary()
      {
         int size = ReadInt32();
         Hashtable hash = new Hashtable(size);
         string key = ReadShortString();
         for (byte type = ReadByte(); type != 9; type = ReadByte())
         {
            object value = ReadData(type);
            hash.Add(key, value);
            key = ReadShortString();
         }
         return hash;
      }
      private DateTime ReadDate()
      {
         double ms = ReadDouble();
         DateTime date = BaseDate.AddMilliseconds(ms);         
         ReadUInt16(); //get's the timezone
         return date;
      }
      private new double ReadDouble()
      {
         return BitConverter.ToDouble(ReadInternal(8), 0);
      }

      private new int ReadInt32()
      {
         return BitConverter.ToInt32(ReadInternal(4), 0);
      }

      private new ushort ReadUInt16()
      {
         return BitConverter.ToUInt16(ReadInternal(2), 0);
      }

      private byte[] ReadInternal(int size)
      {
         byte[] buffer = new byte[size];
         for (int i = size - 1; i >= 0; --i)
         {
            buffer[i] = ReadByte();
         }
         return buffer;
      }
      private string ReadLongString()
      {
         int length = ReadInt32();
         return ReadString(length);
      }
      private string ReadShortString()
      {
         int length = ReadUInt16();
         return ReadString(length);
      }
      private string ReadString(int length)
      {
         byte[] buffer = new byte[length];
         Read(buffer, 0, length);
         return Encoding.UTF8.GetString(buffer);
      }
      private XmlDocument ReadXmlDocument()
      {
         XmlDocument xml = new XmlDocument();
         xml.LoadXml(ReadLongString());
         return xml;
      }
      private object ReadTypedObject()
      {
         string classType = ReadShortString();
         Hashtable hash = ReadUntypedObject();
         object typedObject = TryToReconstitute(classType, hash);
         if (typedObject != null)
         {
            return typedObject;
         }
         //only use the flash object type if it wasn't actual typed
         if (!hash.ContainsKey(SmartObject.ObjectTypeIdentifier))
         {
            hash[SmartObject.ObjectTypeIdentifier] = classType;
         }
         return hash;

      }
      private Hashtable ReadUntypedObject()
      {
         Hashtable hash = new Hashtable();
         string key = ReadShortString();
         for (byte type = ReadByte(); type != 9; type = ReadByte())
         {
            hash.Add(key, ReadData(type));
            key = ReadShortString();
         }
         return hash;
      }

      private ArrayList ReadArray()
      {
         int size = ReadInt32();
         ArrayList arr = new ArrayList(size);
         for (int i = 0; i < size; ++i)
         {
            arr.Add(ReadData(ReadByte()));
         }
         return arr;
      }
      private object TryToReconstitute(string typeName, Hashtable parameters)
      {
         foreach (IReconstitutor reconstitutor in _reconstitutors)
         {
            if (string.Compare(typeName, reconstitutor.RegisteredName, true) == 0)
            {
               if (reconstitutor.ParametersAreValid(parameters))
               {
                  return reconstitutor.Reconstitute(parameters);
               }
            }
         }
         return null;
      }  
      #endregion
   }
}