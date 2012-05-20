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
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Fuel.AmfNet {
	public class Serializer : BinaryWriter {
		#region Constructors

		public Serializer(Stream output)
			: base(output) {
		}

		#endregion

		#region Public Methods

		public void Serialize(Message message) {
			WriteUInt16(message.Version);
			WriteUInt16((UInt16)message.Headers.Count);
			foreach (Header header in message.Headers) {
				Write(header.Key);
				Write(header.Required);
				Write(-1);
				WriteData(header.Data);
			}
			WriteUInt16((UInt16)message.Response.Count);
			foreach (Response response in message.Response) {
				WriteString(response.ReplyMethod);
				WriteString("null");
				Write(-1);
				WriteData(response.Data);
			}
		}

		public static byte[] SerializeBytes(string method, string target, object data) {
			//data
			var msData = new MemoryStream();
			var ser2 = new Serializer(msData);
			ser2.WriteData(data);
			byte[] b = msData.ToArray();

			var msMain = new MemoryStream();
			var ser = new Serializer(msMain);
			ser.WriteUInt16(3); //version
			ser.WriteUInt16(0); //0 headers
			ser.WriteUInt16(1); //1 body
			//body
			ser.WriteString(method);
			ser.WriteString(target);
			ser.WriteUInt32((uint)b.Length);
			msMain.Write(b, 0, b.Length);
			return msMain.ToArray();
		}

		public void SerializeRequest(Message message) {
			WriteUInt16(message.Version);
			WriteUInt16((UInt16)message.Headers.Count);
			foreach (Header header in message.Headers) {
				Write(header.Key);
				Write(header.Required);
				Write(-1);
				WriteData(header.Data);
			}
			WriteUInt16((UInt16)message.Bodies.Count);
			foreach (Body b in message.Bodies) {
				WriteString(b._type+"."+b._method);
				WriteString(b._target);
				Write(-1);
				WriteData(b._data);
			}
		}

		#endregion

		#region Private Methods
		private void WriteData(object data) {
			if (data == null || data == DBNull.Value) {
				Write((byte)DataType.Null);
			} else if (data is string) {
				string str = (string)data;
				if (str.Length > UInt16.MaxValue) {
					Write((byte)DataType.String);
					WriteString(str);
				} else {
					Write((byte)DataType.LongString);
					WriteLongString(str);
				}
			} else if (data is char) {
				Write((byte)DataType.String);
				WriteString(((char)data).ToString());
			} else if (data is bool) {
				Write((byte)DataType.Boolean);
				Write((bool)data);
			} else if (data is DateTime) {
				Write((byte)DataType.Date);
				WriteDate((DateTime)data);

			} else if (IsNumber(data)) {
				Write((byte)DataType.Number);
				double d = Convert.ToDouble(data.ToString());
				WriteDouble(d);
			} else if (data is XmlDocument) {
				Write((byte)DataType.Xml);
				WriteLongString(((XmlDocument)data).OuterXml);
			} else if (data is IDictionary) {
				Write((byte)DataType.MixedArray);
				WriteDictionary((IDictionary)data);
			} else if (data is ICollection) {
				Write((byte)DataType.Array);
				WriteCollection((ICollection)data);
			} else if (data.GetType().IsArray) {
				ArrayList arr = new ArrayList((object[])data);
				Write((byte)DataType.Array);
				WriteCollection(arr);
			} else if (data is AmfError) {
				Write((byte)DataType.TypedObject);
				WriteError((AmfError)data);
			} else {
				Write((byte)DataType.TypedObject);
				WriteCustomClass(data);
			}
		}

		private void WriteError(AmfError error) {
			WriteString(error.GetType().FullName);
			WriteString("level");
			Write((byte)DataType.String);
			WriteString(error.Level);

			WriteString("code");
			Write((byte)DataType.String);
			WriteString(error.Code);

			WriteString("details");
			Write((byte)DataType.String);
			WriteString(error.Details);

			WriteString("description");
			Write((byte)DataType.String);
			WriteString(error.Description);

			WriteString("type");
			Write((byte)DataType.String);
			WriteString(error.Type);

			WriteEnd();
		}

		private void WriteCustomClass(object value) {
			Type type = value.GetType();
			WriteString(type.FullName);
			PropertyInfo[] properties = type.GetProperties();
			foreach (PropertyInfo property in properties) {
				WriteString(property.Name);
				WriteData(property.GetValue(value, null));
			}
			WriteEnd();
		}
		private void WriteEnd() {
			WriteUInt16(0);
			Write((byte)DataType.End);
		}
		private void WriteDictionary(IDictionary value) {
			WriteUInt32((UInt32)value.Count);
			IDictionaryEnumerator enumerator = value.GetEnumerator();
			while (enumerator.MoveNext()) {
				//todo throw exception if key isn't string
				WriteString(enumerator.Key.ToString());
				WriteData(enumerator.Value);
			}
		}
		private void WriteCollection(ICollection value) {
			WriteUInt32((UInt32)value.Count);
			foreach (object o in value) {
				WriteData(o);
			}
		}
		private void WriteDate(DateTime date) {
			TimeSpan elapsed = date.Subtract(Deserializer.BaseDate);
			long l = (long)elapsed.TotalMilliseconds;
			WriteInternal(BitConverter.GetBytes(BitConverter.DoubleToInt64Bits((double)l)));
			WriteUInt16((UInt16)65236);
		}

		private void WriteDouble(double value) {
			WriteInternal(BitConverter.GetBytes(value));
		}
		private void WriteUInt32(UInt32 value) {
			WriteInternal(BitConverter.GetBytes(value));
		}
		private void WriteUInt16(UInt16 value) {
			WriteInternal(BitConverter.GetBytes(value));
		}
		private void WriteString(string value) {
			int length = value.Length;
			WriteUInt16((UInt16)length);
			byte[] buffer = Encoding.UTF8.GetBytes(value);
			Write(buffer, 0, length);
		}
		private void WriteLongString(string value) {
			int length = value.Length;
			WriteUInt32((UInt32)length);
			byte[] buffer = Encoding.UTF8.GetBytes(value);
			Write(buffer, 0, length);
		}

		private void WriteInternal(byte[] value) {
			for (int i = value.Length - 1; i >= 0; --i) {
				Write(value[i]);
			}
		}
		private bool IsNumber(object data) {
			return (data is int || data is long || data is short || data is float || data is byte || data is sbyte || data is uint || data is ulong || data is ushort || data is double);
		}
		#endregion
	}
}