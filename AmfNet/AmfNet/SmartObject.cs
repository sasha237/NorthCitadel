using System;
using System.Collections;

namespace Fuel.AmfNet
{
    public class SmartObject
   {
      #region Fields and Properties
        public const string ObjectTypeIdentifier = "__NetType";
      private bool? _canBeInt32;
      private bool? _canBeInt64;
      private bool? _isArray;
      private bool _isEmptyArray;
      private bool? _isClass;
      private Type _classType;
      private Type _arrayType;
      private object[] _typedArray;
      private object _value;
      private Type _type;

      public bool IsEmptyArray
      {
         get { return _isEmptyArray; }
      }
      public Type ArrayType
      {
         get
         {
            return _arrayType;
         }
      }
      public Type ClassType
      {
         get { return _classType; }
      }
      public object[] TypedArray
      {
         get
         {
            return _typedArray;
         }
      }
      public bool CanBeInt32
      {
         get
         {
            if (!_canBeInt32.HasValue)
            {
               _canBeInt32 = (_value is double && IsInt32((double)Value));
            }
            return _canBeInt32.Value;
         }
      }
      public bool CanBeInt64
      {
         get
         {
            if (!_canBeInt64.HasValue)
            {
               _canBeInt64 = (_value is double && IsInt64((double)Value));
            }
            return _canBeInt64.Value;
         }
      }
      public object Value
      {
         get
         {
            return _value;
         }
      }
      public Type Type
      {
         get
         {
            if (_type == null && _value !=  null)
            {
               _type = _value.GetType();
            }
            return _type;
         }
      }
      #endregion

      #region Constructor
      public SmartObject(object value)
      {
         _value = value;
      }
      #endregion

      /// <summary>
      /// All arrays are initially deserialized as ArrayLists,
      /// Let's try and see if it can be an actual typed array
      /// </summary>      
      /// <returns></returns>
      public bool CanBeArray()
      {
         if (_isArray.HasValue)
         {
            return _isArray.Value;
         }

         ArrayList arr = _value as ArrayList;
         //short circuit the logic, this can only be an typed array
         //if it was deserialized as an arraylist and it isn't empty
         //(it could be empty and be typed, but how would we know?)
         if (arr == null || arr.Count == 0)
         {
            _isEmptyArray = true;
            _isArray = false;
            return false;
         }
         _isEmptyArray = false;

         Type firstType = arr[0].GetType();
         for (int i = 1; i < arr.Count; ++i)
         {
            if (!firstType.Equals(arr[i].GetType()))
            {
               _isArray = false;
               return false;
            }
         }                  
         _typedArray = (object[])arr.ToArray(firstType);
         _arrayType = _typedArray.GetType();
         _isArray = true;
         return true;                  
      }
      /// <summary>
      /// Converts a hashtable to a class
      /// </summary>      
      public bool CanBeClass()
      {
         if (_isClass.HasValue)
         {
            return _isClass.Value;               
         }
         Hashtable value = _value as Hashtable;
         if (value == null)
         {
            _isClass = false;
            return false;
         }
         string typeName = value[ObjectTypeIdentifier] as string;
         if (typeName == null)
         {
            _isClass = false;
            return false;
         }
         _classType = Type.GetType(typeName);
         value.Remove(ObjectTypeIdentifier); //remove the identifier 
         _isClass = (_classType != null);
         return _isClass.Value;         
      }
      
      #region Private Methods
      private static bool IsInt32(double value)
      {
         if (value <= Int32.MaxValue && value >= Int32.MinValue)
         {
            int integer = (int)value;
            double result = (value - integer);
            return result == 0.0;
         }
         return false;
      }
      private static bool IsInt64(double value)
      {
         if (value <= Int64.MaxValue && value >= Int64.MinValue)
         {
            int integer = (int)value;
            double result = (value - integer);
            return result == 0.0;
         }
         return false;
      }
      #endregion
   }
}