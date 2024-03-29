using System;
using System.Collections.Generic;
using UnityEngine;

namespace WrongWarp
{
    /// <summary>
    /// Contains a bunch of functions used to serialize tag data.
    /// </summary>
    public static class WWTagConverter
    {

        /// <summary>
        /// Converts a string into a byte array made up of the string's characters.
        /// </summary>
        public static byte[] GetStringAsCharBytes(string stringToConvert)
        {
            List<byte> tempData = new();
            foreach (char character in stringToConvert)
            {
                tempData.Add(BitConverter.GetBytes(character)[0]);
            }
            return tempData.ToArray();
        }

        /// <summary>
        /// Converts an int, ushort, float, bool or string to an array of bytes.
        /// <para>
        /// Strings must be '$' terminated.
        /// </para>
        /// </summary>
        public static byte[] ConvertSingleDataToBytes<T>(T data)
        {
            Type dataType = typeof(T);

            if (dataType == typeof(int))            // Int
            {
                return BitConverter.GetBytes((int)(data as int?));
            }
            else if (dataType == typeof(ushort))    // UShort
            {
                return BitConverter.GetBytes((ushort)(data as ushort?));
            }
            else if (dataType == typeof(float))     // Float
            {
                return BitConverter.GetBytes((float)(data as float?));
            }
            else if (dataType == typeof(bool))      // Bool
            {
                return BitConverter.GetBytes((bool)(data as bool?));
            }
            else if (dataType == typeof(string))    // String
            {
                return GetStringAsCharBytes(data as string);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Writes an int[], ushort[], float[], bool[] or string[] as data.
        /// <para>
        /// Strings must be '$' terminated.
        /// </para>
        /// </summary>
        /// <param name="data"> An int[], ushort[], float[], bool[] or string[] to write to this tag's data. </param>
        public static byte[] GenericArrayToByteArray<T>(T[] data)
        {
            Type dataType = typeof(T[]);

            if(dataType == typeof(byte[]))
            {
                Debug.Log("Tried to pass a byte array through the generic array to byte array converter.");
                return data as byte[];
            }

            List<byte> dataToUse = new();
            if (dataType == typeof(int[]))
            {
                for (uint i = 0; i < data.Length; i++)
                {
                    dataToUse.AddRange(BitConverter.GetBytes((int)(data[i] as int?)));
                }
            }
            else if (dataType == typeof(ushort[]))
            {
                for (uint i = 0; i < data.Length; i++)
                {
                    dataToUse.AddRange(BitConverter.GetBytes((ushort)(data[i] as ushort?)));
                }
            }
            else if (dataType == typeof(float[]))
            {
                for (uint i = 0; i < data.Length; i++)
                {
                    dataToUse.AddRange(BitConverter.GetBytes((float)(data[i] as float?)));
                }
            }
            else if (dataType == typeof(bool[]))
            {
                for (uint i = 0; i < data.Length; i++)
                {
                    dataToUse.AddRange(BitConverter.GetBytes((bool)(data[i] as bool?)));
                }
            }
            else if (dataType == typeof(string[]))
            {
                for (uint i = 0; i < data.Length; i++)
                {
                    dataToUse.AddRange(GetStringAsCharBytes(data[i] as string));
                }
            }
            else
            {
                Debug.Log("Attempted to add an unrecognized data type as a tag.");
                dataToUse = null;
            }
            return dataToUse == default ? null : dataToUse.ToArray();
        }

    }
}
