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
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace Fuel.AmfNet
{
    public class Body
   {
      #region Fields and Properties
      private readonly static Type _int32Type = Type.GetType("System.Int32");
      private readonly static Type _int64Type = Type.GetType("System.Int64");
      public string _method;
      public string _target;
      public object _data;
      public string _type;
      
      public string Target
      {
         get { return _target; }
      }

      #endregion

      #region Constructors

      public Body(string method, string type, string target, object data)
      {
         _method = method;
         _target = target;
         _type = type;
         _data = data;
      }

      #endregion

      #region Public Methods

      public object Invoke()
      {
         if (_type == null)
         {
            throw new AmfNetException("Can't load null type");
         }
         ArrayList rawInputs = _data as ArrayList;
         if (rawInputs == null)
         {
            throw new AmfNetException("Expecting ArrayList of parameters");
         }
                  

         Type type = Type.GetType(_type);
         if (type == null)
         {
            throw new AmfNetException(string.Format("Couldn't load type: {0}", _type));
         }
         List<SmartObject> smartInputs = LoadSmartObjects(rawInputs);

         ArrayList newInputs;         
         MethodInfo foundMethod = FindMethod(type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly), smartInputs, out newInputs);

         if (foundMethod == null)
         {
            throw new AmfNetException(string.Format("Match not found in type \"{0}\" for {1}", _type, this.ToString()));
         }
         return foundMethod.Invoke(Activator.CreateInstance(type), newInputs.ToArray());
      }
      public override string ToString()
      {

         StringBuilder sb = new StringBuilder();
         sb.AppendFormat("{0}(", _method);
         ArrayList rawInputs = _data as ArrayList;
         if (rawInputs != null && rawInputs.Count > 0)
         {
            foreach (object value in rawInputs)
            {
               if (value == null)
               {
                   sb.AppendFormat("\"{0}\", ", value);
               }
               else
               if (value is string)
               {
                  sb.AppendFormat("\"{0}\", ", value.ToString());
               }
               else
               {
                  sb.AppendFormat("{0}, ", value.ToString());
               }               
            }            
            sb.Remove(sb.Length - 2, 2);
         }
         sb.Append(")");
         return sb.ToString();
      }

      #endregion
      
      #region Private Methods
      private List<SmartObject> LoadSmartObjects(ArrayList rawInputs)
      {
         List<SmartObject> _smartInputs = new List<SmartObject>(rawInputs.Count);
         foreach (object value in rawInputs)
         {
            _smartInputs.Add(new SmartObject(value));
         }
         return _smartInputs;
      }

      private MethodInfo FindMethod(MethodInfo[] methods, List<SmartObject> inputs, out ArrayList newInputs)
      {
         foreach (MethodInfo method in methods)
         {
            if (method.Name == _method && IsMethodAMatch(method.GetParameters(), inputs, out newInputs))
            {
               return method;
            }
         }
         newInputs = new ArrayList();
         return null;
      }

      private bool IsMethodAMatch(ParameterInfo[] parameters, List<SmartObject> inputs, out ArrayList newInputs)
      {
         newInputs = new ArrayList(inputs.Count);
         List<int> classIndices = new List<int>();
         if (parameters.Length != inputs.Count)
         {
            return false;
         }
         if (parameters.Length == 0 && inputs.Count == 0)
         {
            return true;
         }
         for (int i = 0; i < inputs.Count; ++i)
         {
            object value;
            if (!IsParameterMatch(parameters[i].ParameterType, inputs[i], out value))
            {
               return false;
            }
            if (value is SmartObject)
            {
               classIndices.Add(i);
            }
            newInputs.Add(value);
         }
          
       
         //we have a match, now create any objects that 
         foreach (int index in classIndices)
         {            
            SmartObject smarty = (SmartObject)newInputs[index];
            Type type = smarty.ClassType;
            Hashtable hash = (Hashtable)smarty.Value;
            object o = Activator.CreateInstance(type);
            foreach (string key in hash.Keys)
            {
               PropertyInfo property = type.GetProperty(key);
               SmartObject smartObject = new SmartObject(hash[key]);
               object value;
               if (property != null && IsParameterMatch(property.PropertyType, smartObject, out value))
               {
                  property.SetValue(o, value, null);
               }
            }
            newInputs[index] = o;
         }
         return true;
      }

      private bool IsParameterMatch(Type parameterType, SmartObject smarty, out object convertedValue)
      {
         object value = smarty.Value;
         if (value == null)
         {
            convertedValue = value;
            return parameterType.IsClass;            
         }
         if (parameterType.IsAssignableFrom(smarty.Type))
         {
            convertedValue = value;
            return true;
         }
         if (smarty.CanBeInt32 && parameterType.IsAssignableFrom(_int32Type))
         {
            convertedValue = (int)(double)value;
            return true;
         }
         if (smarty.CanBeInt64 && parameterType.IsAssignableFrom(_int64Type))
         {
            convertedValue = (long)(double)value;
            return true;
         }
         if (parameterType.IsArray && smarty.CanBeArray() && parameterType.IsAssignableFrom(smarty.ArrayType))
         {
            convertedValue = smarty.TypedArray;
            return true;
         }
         if (parameterType.IsArray && smarty.IsEmptyArray)
         {
            Type itemType = Type.GetType(parameterType.FullName.Substring(0, parameterType.FullName.Length - 2));
            convertedValue = ((ArrayList)smarty.Value).ToArray(itemType);
            return true;
         }
         if (parameterType.IsEnum && smarty.CanBeInt32 && Enum.GetUnderlyingType(parameterType).IsAssignableFrom(_int32Type))
         {
            convertedValue = Enum.Parse(parameterType, ((int)(double)value).ToString());
            return true;
         }
         if (smarty.CanBeClass() && parameterType.IsAssignableFrom(smarty.ClassType))
         {
            convertedValue = smarty;            
            return true;
         }
         convertedValue = null;
         return false;
      }
     #endregion
   }
}
