using System;
using UnityEngine;

namespace WrongWarp
{
    /// <summary>
    /// Functions are big endian unless otherwise specified
    /// </summary>
    public static class WWBinaryFunctions
    {
        public static byte CompressFloatDegreesToByte(float value)
        {
            return CompressFloatToByte(value, 0, 360);
        }
        public static ushort CompressFloatDegreesToUShort(float value)
        {
            return CompressFloatToUShort(value, 0, 360);
        }
        public static byte CompressFloatToByte(float value, float minValue, float maxValue)
        {
            return (byte)(((value - minValue) / (maxValue - minValue)) * byte.MaxValue);
        }
        public static ushort CompressFloatToUShort(float value, float minValue, float maxValue)
        {
            return (ushort)(((value - minValue) / (maxValue - minValue)) * ushort.MaxValue);
        }
        public static float DecompressByteDegrees(byte value)
        {
            return DecompressByteToFloat(value, 0, 360);
        }
        public static float DecompressUShortDegrees(ushort value)
        {
            return DecompressUShortToFloat(value, 0, 360);
        }
        public static float DecompressByteToFloat(byte value, float minValue, float maxValue)
        {
            Debug.Log(value / byte.MaxValue);
            Debug.Log(maxValue - minValue);
            return (((float)value / (float)byte.MaxValue) * (maxValue - minValue)) + minValue;
        }
        public static float DecompressUShortToFloat(ushort value, float minValue, float maxValue)
        {
            return (((float)value / (float)ushort.MaxValue) * (maxValue - minValue)) + minValue;
        }

        public static int BoolToInt(bool a)
        {
            return a ? 1 : 0;
        }

        public static bool IntToBool(int a)
        {
            return a != 0;
        }
        /// <summary>
        /// Treats byte indices as 01234567
        /// </summary>
        public static bool ReadBit(byte b, int index)
        {
            return (b & (128 >> index)) != 0;
        }

        /// <summary>
        /// Treats byte indices as 01234567
        /// </summary>
        public static byte WriteBit(byte b, int index, bool value)
        {
            if (value)
            {
                return (byte)(b | (128 >> index));
            }
            else
            {
                return (byte)(b & ~(128 >> index));
            }
        }

        /// <summary>
        /// Treats byte indices as 01234567
        /// </summary>
        public static bool[] TrimByte(byte b, int valuesToKeep)
        {
            if (valuesToKeep < 0 || valuesToKeep > 8)
            {
                throw new System.Exception($"Cannot trim a byte to {valuesToKeep} values.");
            }
            bool[] returnData = new bool[valuesToKeep];
            for (int i = 0; i < valuesToKeep ; i++)
            {
                returnData[i] = ReadBit(b, 7-i);
            }
            return returnData;
        }

        /// <summary>
        /// Treats byte indices as 01234567
        /// </summary>
        public static byte BoolsToByte(bool[] bools)
        {
            byte value = 0;
            for (int i = 0; i < bools.Length && i < 8; i++)
            {
                value *= 2;
                value += (byte)(bools[bools.Length - 1 - i] == true ? 1 : 0);
            }
            return value;
        }

        public static T ConvertType<T>(string value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }

        public static byte RemoveByteSign(sbyte _sbyte)
        {
            if(_sbyte < 0)
            {
                return (byte)(sbyte.MaxValue - _sbyte);
            }
            return (byte)_sbyte;
        }
        public static ushort RemoveShortSign(short _short)
        {
            if (_short < 0)
            {
                return (ushort)(short.MaxValue - _short);
            }
            return (ushort)_short;
        }
        public static uint RemoveIntSign(int _int)
        {
            if (_int < 0)
            {
                return (uint)(int.MaxValue - _int);
            }
            return (uint)_int;
        }
        public static ulong RemoveLongSign(long _long)
        {
            if (_long < 0)
            {
                return (ulong)(long.MaxValue - _long);
            }
            return (ulong)_long;
        }
        public static sbyte AddSByteSign(byte _byte)
        {
            if(_byte > sbyte.MaxValue)
            {
                return (sbyte)(-_byte + sbyte.MinValue);
            }
            return (sbyte)_byte;
        }
        public static short AddShortSign(ushort _ushort)
        {
            if (_ushort > short.MaxValue)
            {
                return (short)(-_ushort + short.MinValue);
            }
            return (short)_ushort;
        }
        public static int AddIntSign(uint _uint)
        {
            if (_uint > int.MaxValue)
            {
                return (int)(-_uint + int.MinValue);
            }
            return (int)_uint;
        }
        public static long AddLongSign(ulong _ulong)
        {
            if (_ulong > long.MaxValue)
            {
                return (long)(0 - _ulong) + long.MinValue;
            }
            return (long)_ulong;
        }
    }
}
