using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WrongWarp
{

    public class ArbitraryDataSlice
    {
        /// <summary>
        /// Don't modify this directly unless you know what you're doing!
        /// </summary>
        public byte[] data;

        /// <summary>
        /// Returns how long a piece of data is for this slice.
        /// </summary>
        public int DataTagLength => GetTagLength(dataTag);
        /// <summary>
        /// Returns the data tag- which represents the type of marker, or length in bytes, of this data.
        /// </summary>
        public ADDataTag DataTag => dataTag;
        private ADDataTag dataTag;  // The data type of the slice

        /// <summary>
        /// Returns the ID assigned to this data slice, if one exists.
        /// </summary>
        public int? ID => id;
        private int? id;                         // Null unless using IDs

        /// <summary>
        /// Returns the length of the ID assigned to this slice.
        /// </summary>
        public ADIDLength IDLength => idLength;
        private ADIDLength idLength;
        public ushort Length
        {
            get
            {
                return DataTagLength == 0 ? ushort.MinValue : (ushort)Math.Floor((float)(data.Length / DataTagLength));
            }
        }

        public ushort LengthAssumingType(ADDataType type)
        {
            return GetADDataTypeLength(type) == 0 ? ushort.MinValue : (ushort)Math.Floor((float)(data.Length / GetADDataTypeLength(type)));
        }

        /// <summary>
        /// This constructor is slightly less efficient than using a data tag to assign a datatype.
        /// </summary>
        public ArbitraryDataSlice(ADDataType dataType, int? _id = null)
        {
            dataTag = ADDataTypeToDataTag(dataType);
            Setup(_id);
        }
        public ArbitraryDataSlice(ADDataTag _dataTag, int? _id = null)
        {
            dataTag = _dataTag;
            Setup(_id);
        }
        private void Setup(int? _id)
        {
            data = new byte[0];
            id = _id;
            if (id == null)
            {
                idLength = ADIDLength.Nonexistent;
            }
            else
            {
                int castedID = (int)id;
                if (castedID < 0)
                {
                    idLength = ADIDLength.Int;
                }
                else
                {
                    if (castedID <= byte.MaxValue)
                    {
                        idLength = ADIDLength.Byte;
                    }
                    else if (castedID <= ushort.MaxValue)
                    {
                        idLength = ADIDLength.UShort;
                    }
                    else
                    {
                        idLength = ADIDLength.Int;
                    }
                }
            }
        }

        public void AddData<T>(T _data, ADDataType dataType, int? index = 0)
        {
            byte[] tempData = null;

            switch (dataType)
            {
                case ADDataType.SByte:
                case ADDataType.Byte:
                    tempData = new byte[1];
                    tempData[0] = (byte)Convert.ChangeType(_data, typeof(byte));
                    break;
                case ADDataType.Short:
                    tempData = BitConverter.GetBytes((short)Convert.ChangeType(_data, typeof(short)));
                    break;
                case ADDataType.UShort:
                    tempData = BitConverter.GetBytes((ushort)Convert.ChangeType(_data, typeof(ushort)));
                    break;
                case ADDataType.Int:
                    tempData = BitConverter.GetBytes((int)Convert.ChangeType(_data, typeof(int)));
                    break;
                case ADDataType.UInt:
                case ADDataType.NetID:
                    tempData = BitConverter.GetBytes((uint)Convert.ChangeType(_data, typeof(uint)));
                    break;
                case ADDataType.Float:
                    tempData = BitConverter.GetBytes((float)Convert.ChangeType(_data, typeof(float)));
                    break;
                case ADDataType.Long:
                    tempData = BitConverter.GetBytes((long)Convert.ChangeType(_data, typeof(long)));
                    break;
                case ADDataType.ULong:
                    tempData = BitConverter.GetBytes((ulong)Convert.ChangeType(_data, typeof(ulong)));
                    break;
                case ADDataType.Double:
                    tempData = BitConverter.GetBytes((double)Convert.ChangeType(_data, typeof(double)));
                    break;
                case ADDataType.Char:
                    tempData = BitConverter.GetBytes((char)Convert.ChangeType(_data, typeof(char)));
                    break;
                case ADDataType.Bool:
                    tempData = new byte[1] { (byte)((bool)Convert.ChangeType(_data, typeof(bool)) == true ? 1 : 0) };
                    break;
                case ADDataType.Vector2:
                    Vector2 vector2 = (Vector2)Convert.ChangeType(_data, typeof(Vector2));
                    tempData = BitConverter.GetBytes(vector2.x);
                    tempData = tempData.Concat(BitConverter.GetBytes(vector2.y)).ToArray();
                    break;
                case ADDataType.Vector3:
                    Vector3 vector3 = (Vector3)Convert.ChangeType(_data, typeof(Vector3));
                    tempData = BitConverter.GetBytes(vector3.x);
                    tempData = tempData.Concat(BitConverter.GetBytes(vector3.y)).ToArray();
                    tempData = tempData.Concat(BitConverter.GetBytes(vector3.z)).ToArray();
                    break;
                case ADDataType.Quaternion:
                    Quaternion quat = (Quaternion)Convert.ChangeType(_data, typeof(Quaternion));
                    tempData = BitConverter.GetBytes(quat.x);
                    tempData = tempData.Concat(BitConverter.GetBytes(quat.y)).ToArray();
                    tempData = tempData.Concat(BitConverter.GetBytes(quat.z)).ToArray();
                    tempData = tempData.Concat(BitConverter.GetBytes(quat.w)).ToArray();
                    break;
                case ADDataType.Vector2SByte:
                case ADDataType.Vector3SByte:
                case ADDataType.Vector2Short:
                case ADDataType.Vector3Short:
                    Debug.LogError("This data type does not need to be converted to a byte array as it should already be a byte array.");
                    break;
                default:
                    Debug.LogError("Could not process data type, returning null.");
                    break;
            }

            if (index == null || index == 0)
            {
                data = data.Concat(tempData).ToArray();
            }
            else if (index < 0)
            {
                Debug.LogError("Cannot insert data at negative index.");
                return;
            }
            else
            {
                data.ToList().InsertRange((int)index * DataTagLength, tempData);
            }
        }

        #region Element Getters

        public sbyte GetElementAsSByte(int index = 0, bool beSafe = true)
        {
            if (beSafe)
            {
                CheckIsInRange(index, false, ADDataType.SByte);
            }
            return (sbyte)data[index];
        }
        public byte GetElementAsByte(int index = 0, bool beSafe = true)
        {
            if (beSafe)
            {
                CheckIsInRange(index, false, ADDataType.Byte);
            }
            return data[index];
        }
        public short GetElementAsShort(int index = 0, bool beSafe = true)
        {
            if (beSafe)
            {
                CheckIsInRange(index, false, ADDataType.Short);
            }
            return BitConverter.ToInt16(data, index * GetADDataTypeLength(ADDataType.Short));
        }
        public ushort GetElementAsUShort(int index = 0, bool beSafe = true)
        {
            if (beSafe)
            {
                CheckIsInRange(index, false, ADDataType.UShort);
            }
            return WWBinaryFunctions.RemoveShortSign(BitConverter.ToInt16(data, index * GetADDataTypeLength(ADDataType.UShort)));
        }
        public int GetElementAsInt(int index = 0, bool beSafe = true)
        {
            if (beSafe)
            {
                CheckIsInRange(index, false, ADDataType.Int);
            }
            return BitConverter.ToInt32(data, index * GetADDataTypeLength(ADDataType.Int));
        }
        public uint GetElementAsUInt(int index = 0, bool beSafe = true)
        {
            if (beSafe)
            {
                CheckIsInRange(index, false, ADDataType.UInt);
            }
            return WWBinaryFunctions.RemoveIntSign(BitConverter.ToInt32(data, index * GetADDataTypeLength(ADDataType.UInt)));
        }
        public float GetElementAsFloat(int index = 0, bool beSafe = true)
        {
            if (beSafe)
            {
                CheckIsInRange(index, false, ADDataType.Float);
            }
            return BitConverter.ToInt32(data, index * GetADDataTypeLength(ADDataType.Float));
        }
        public long GetElementAsLong(int index = 0, bool beSafe = true)
        {
            if (beSafe)
            {
                CheckIsInRange(index, false, ADDataType.Long);
            }
            return BitConverter.ToInt64(data, index * GetADDataTypeLength(ADDataType.Long));
        }
        public ulong GetElementAsULong(int index = 0, bool beSafe = true)
        {
            if (beSafe)
            {
                CheckIsInRange(index, false, ADDataType.ULong);
            }
            return WWBinaryFunctions.RemoveLongSign(BitConverter.ToInt64(data, index * GetADDataTypeLength(ADDataType.Long)));
        }
        public double GetElementAsDouble(int index = 0, bool beSafe = true)
        {
            if (beSafe)
            {
                CheckIsInRange(index, false, ADDataType.Double);
            }
            return BitConverter.ToDouble(data, index * GetADDataTypeLength(ADDataType.Double));
        }
        public char GetElementAsChar(int index = 0, bool beSafe = true)
        {
            if (beSafe)
            {
                CheckIsInRange(index, false, ADDataType.Char);

            }
            return BitConverter.ToChar(data, index * GetADDataTypeLength(ADDataType.Char));
        }
        public bool GetElementAsBool(int index = 0, bool beSafe = true)
        {
            if (beSafe)
            {
                CheckIsInRange(index, false, ADDataType.Bool);
            }
            return BitConverter.ToBoolean(data, index * GetADDataTypeLength(ADDataType.Bool));
        }
        public Vector2 GetElementAsVector2(int index = 0, bool beSafe = true)
        {
            if (beSafe)
            {
                CheckIsInRange(index, false, ADDataType.Vector2);
            }
            Vector3 ret = Vector3.zero;
            int startIndex = index * 8;
            ret.x = BitConverter.ToSingle(data, startIndex);
            ret.y = BitConverter.ToSingle(data, 4 + startIndex);
            return ret;
        }
        public Vector3 GetElementAsVector3(int index = 0, bool beSafe = true)
        {
            if (beSafe)
            {
                CheckIsInRange(index, false, ADDataType.Vector3);
            }
            Vector3 ret = Vector3.zero;
            int startIndex = index * 12;
            ret.x = BitConverter.ToSingle(data, startIndex);
            ret.y = BitConverter.ToSingle(data, 4 + startIndex);
            ret.z = BitConverter.ToSingle(data, 8 + startIndex);
            return ret;
        }
        public Quaternion GetElementAsQuat(int index = 0, bool beSafe = true)
        {
            if (beSafe)
            {
                CheckIsInRange(index, false, ADDataType.Quaternion);
            }
            Quaternion ret = Quaternion.identity;
            int startIndex = index * 16;
            ret.x = BitConverter.ToSingle(data, startIndex);
            ret.y = BitConverter.ToSingle(data, 4 + startIndex);
            ret.z = BitConverter.ToSingle(data, 8 + startIndex);
            ret.w = BitConverter.ToSingle(data, 12 + startIndex);
            return ret;
        }

        #endregion Element Getters

        #region Array Getters

        public byte[] GetDataAsByteArray()
        {
            return data;
        }
        public sbyte[] GetDataAsSByteArray()
        {
            sbyte[] ret = new sbyte[LengthAssumingType(ADDataType.SByte)];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = GetElementAsSByte(i, false);
            }
            return ret;
        }
        public short[] GetDataAsShortArray()
        {
            short[] ret = new short[LengthAssumingType(ADDataType.Short)];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = GetElementAsShort(i, false);
            }
            return ret;
        }
        public ushort[] GetUShortArray()
        {
            ushort[] ret = new ushort[LengthAssumingType(ADDataType.UShort)];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = GetElementAsUShort(i, false);
            }
            return ret;
        }
        public int[] GetDataAsIntArray()
        {
            int[] ret = new int[LengthAssumingType(ADDataType.Int)];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = GetElementAsInt(i, false);
            }
            return ret;
        }
        public uint[] GetDataAsUIntArray()
        {
            uint[] ret = new uint[LengthAssumingType(ADDataType.UInt)];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = GetElementAsUInt(i, false);
            }
            return ret;
        }
        public float[] GetDataAsFloatArray()
        {
            float[] ret = new float[LengthAssumingType(ADDataType.Float)];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = GetElementAsFloat(i, false);
            }
            return ret;
        }
        public long[] GetDataAsLongArray()
        {
            long[] ret = new long[LengthAssumingType(ADDataType.Long)];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = GetElementAsLong(i, false);
            }
            return ret;
        }
        public ulong[] GetDataAsULongArray()
        {
            ulong[] ret = new ulong[LengthAssumingType(ADDataType.ULong)];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = GetElementAsULong(i, false);
            }
            return ret;
        }
        public double[] GetDataAsDoubleArray()
        {
            double[] ret = new double[LengthAssumingType(ADDataType.Double)];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = GetElementAsDouble(i, false);
            }
            return ret;
        }
        public char[] GetDataAsCharArray()
        {
            char[] ret = new char[LengthAssumingType(ADDataType.Char)];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = GetElementAsChar(i, false);
            }
            return ret;
        }
        public bool[] GetDataAsBoolArray()
        {
            bool[] ret = new bool[LengthAssumingType(ADDataType.Bool)];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = GetElementAsBool(i, false);
            }
            return ret;
        }
        public Vector2[] GetDataAsVector2Array()
        {
            Vector2[] ret = new Vector2[LengthAssumingType(ADDataType.Vector2)];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = GetElementAsVector2(i, false);
            }
            return ret;
        }
        public Vector3[] GetDataAsVector3Array()
        {
            Vector3[] ret = new Vector3[LengthAssumingType(ADDataType.Vector3)];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = GetElementAsVector3(i, false);
            }
            return ret;
        }
        public Quaternion[] GetDataAsQuatArray()
        {
            Quaternion[] ret = new Quaternion[LengthAssumingType(ADDataType.Quaternion)];
            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = GetElementAsQuat(i, false);
            }
            return ret;
        }

        #endregion Array Getters

        #region Element Setters

        public void SetElementBool(bool set, int index = 0)
        {
            if (CheckIsInRange(index, false, ADDataType.Bool))
            {
                data[index] = (byte)WWBinaryFunctions.BoolToInt(set);
            }
        }

        #endregion Element Setters

        #region Enum Converters

        public static int GetTagLength(ADDataTag dataTag)
        {
            switch (dataTag)
            {
                // Data Types
                case ADDataTag.Zero:
                case ADDataTag.FrameMarker:
                    return 0;
                case ADDataTag.One:
                case ADDataTag.ChildMarker:
                    return 1;
                case ADDataTag.Two:
                    return 2;
                case ADDataTag.Three:
                    return 3;
                case ADDataTag.Four:
                case ADDataTag.NetIDMarker:
                    return 4;
                case ADDataTag.Six:
                    return 6;
                case ADDataTag.Eight:
                    return 8;
                case ADDataTag.Twelve:
                    return 12;
                case ADDataTag.Sixteen:
                    return 16;
                default:
                    return 1;
            }
        }

        public static int GetADDataTypeLength(ADDataType dataType)
        {
            switch (dataType)
            {
                case ADDataType.Empty:
                    return 0;
                case ADDataType.SByte:
                case ADDataType.Byte:
                case ADDataType.Bool:
                    return 1;
                case ADDataType.Short:
                case ADDataType.UShort:
                case ADDataType.Char:
                case ADDataType.Vector2SByte:
                    return 2;
                case ADDataType.Vector3SByte:
                    return 3;
                case ADDataType.Int:
                case ADDataType.UInt:
                case ADDataType.Float:
                case ADDataType.Vector2Short:
                case ADDataType.NetID:
                    return 4;
                case ADDataType.Vector3Short:
                    return 6;
                case ADDataType.Long:
                case ADDataType.ULong:
                case ADDataType.Double:
                case ADDataType.Vector2:
                    return 8;
                case ADDataType.Vector3:
                    return 12;
                case ADDataType.Quaternion:
                    return 16;
                default:
                    Debug.Log("Returning -1 because unknown data type.");
                    return -1;
            }
        }

        public static ADDataTag ADDataTypeToDataTag(ADDataType dataType)
        {
            switch (dataType)
            {
                case ADDataType.SByte:
                case ADDataType.Byte:
                case ADDataType.Bool:
                    return ADDataTag.One;
                case ADDataType.Short:
                case ADDataType.UShort:
                case ADDataType.Char:
                case ADDataType.Vector2SByte:
                    return ADDataTag.Two;
                case ADDataType.Vector3SByte:
                    return ADDataTag.Three;
                case ADDataType.Int:
                case ADDataType.UInt:
                case ADDataType.Float:
                case ADDataType.Vector2Short:
                    return ADDataTag.Four;
                case ADDataType.Vector3Short:
                    return ADDataTag.Six;
                case ADDataType.Long:
                case ADDataType.ULong:
                case ADDataType.Double:
                case ADDataType.Vector2:
                    return ADDataTag.Eight;
                case ADDataType.Vector3:
                    return ADDataTag.Twelve;
                case ADDataType.Quaternion:
                    return ADDataTag.Sixteen;
                case ADDataType.NetID:
                    return ADDataTag.NetIDMarker;
                default:
                    Debug.Log("Returning DataTag.Empty because unknown ArbitraryDataDataDataType");
                    return ADDataTag.Zero;
            }
        }

        #endregion Enum Converters

        public void InsertElement(byte[] _data, int index)
        {
            if (!IsValidData(_data))
            {
                Debug.LogError("Attempted to add invalid data type to ArbitraryDataSlice. Returning early.");
                return;
            }
            if (!CheckIsInRange(index, true)) { return; }
        }

        public void RemoveElement(int index)
        {
            if (CheckIsInRange(index, true)) { return; }

            int dataLength = GetTagLength(dataTag);

            List<byte> tempList = data.ToList();                                        // Extremely inefficient, I don't feel like fixing it right now
            tempList.RemoveRange(dataLength * index, dataLength);
            data = tempList.ToArray();
        }

        private bool IsValidData(byte[] _data)
        {
            return _data.Length == DataTagLength;
        }

        private bool CheckIsInRange(int index, bool allowToContinue, ADDataType? dataType = null)
        {
            if ((dataType == null && index > Length || index < 0)
                ||  // OR
                (dataType != null && index > data.Length / GetADDataTypeLength((ADDataType)dataType) || index < 0))
            {
                if (allowToContinue)
                {
                    throw new Exception("Could not get element outside of range.");
                }
                else
                {
                    Debug.LogError("Attempted to access data outside range of data. Returning early.");
                    return false;
                }
            }
            return true;
        }

        public enum ADIDLength : byte
        {
            Nonexistent = 0,
            Byte = 1,
            UShort = 2,
            Int = 3
        }
    }

    public enum ADDataType : byte
    {
        // Can have, at most, 31 different data types.
        SByte = 0,
        Byte = 1,

        Short = 2,
        UShort = 3,

        Int = 4,
        UInt = 5,

        Float = 6,

        Long = 9,
        ULong = 10,

        Double = 11,

        Char = 12,

        Empty = 13,

        Bool = 14,

        Vector2 = 15,
        Vector3 = 16,
        Vector2SByte = 17,
        Vector3SByte = 18,
        Vector2Short = 19,
        Vector3Short = 20,

        Quaternion = 21,

        NetID = 31,
        VariableLength = 32,
    }

    public enum ADDataTag : byte
    {
        // Normal byte lengths
        Zero = 0,
        One = 1,
        Two = 2,  
        Four = 3,
        Eight = 4,
        Twelve = 5,     // Vector3
        Sixteen = 6,    // Vector4, Quaternion

        //Byte variable = 7       // Following byte says interval length of data
        //UShort variable = 8     // Following ushort says interval length of data
        //UInt variable = 9       // Following uint says interval length of data
        //Consider infinite length? Read first 7 bits, then if last bit is set to 1, continue to next?

        // Abnormal byte lengths, but still sorta commonplace
        Three = 10,     // Vector3Byte
        Six = 11,       // Vector3UShort

      //12 (Unused)

        // Markers
        ChildMarker = 13,
        FrameMarker = 14,
        NetIDMarker = 15
    }
}
