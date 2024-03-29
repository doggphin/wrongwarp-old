using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

namespace WrongWarp
{
    public class ArbitraryData
    {
        public List<ArbitraryDataSlice> slices = new();

        public ArbitraryData(List<ArbitraryDataSlice> _slices = null)
        {
            slices = _slices == null ? new List<ArbitraryDataSlice>() : new List<ArbitraryDataSlice>();
        }

        /// <summary>
        /// Packs ArbitraryData into a byte array for sending across the network or just to attach to something.
        /// </summary>
        public byte[] Packed {
            get {
                List<byte> _packedData = new();

                foreach (ArbitraryDataSlice slice in slices)
                {
                    bool isArray = slice.DataTag != ADDataTag.Zero && slice.Length > 1;
                    ArbitraryDataSlice.ADIDLength idLength = slice.IDLength;
                    byte headerData = 0;

                    // Write first bit: whether data is little endian
                    if (BitConverter.IsLittleEndian)
                    {
                        headerData |= 0b10000000;
                    }

                    // Write second and third bit: whether data has an ID (reserves ushort for ID)
                    if (idLength == ArbitraryDataSlice.ADIDLength.UShort || idLength == ArbitraryDataSlice.ADIDLength.Int)
                    {
                        headerData |= 0b01000000;
                    }
                    if (idLength == ArbitraryDataSlice.ADIDLength.Byte || idLength == ArbitraryDataSlice.ADIDLength.Int)
                    {
                        headerData |= 0b00100000;
                    }

                    // Write fourth bit (whether data is an array, reserves ushort for array length) to the header
                    if (isArray)
                    {
                        headerData |= 0b00010000;
                    }

                    // Write bits 5, 6, 7 and 8 (the data type) to the header
                    headerData |= (byte)slice.DataTag;

                    // Attach the header,
                    _packedData.Add(headerData);

                    // Attach ID (if applicable),
                    if (idLength != ArbitraryDataSlice.ADIDLength.Nonexistent)
                    {
                        switch (idLength)
                        {
                            case ArbitraryDataSlice.ADIDLength.Byte:
                                _packedData.Add((byte)slice.ID);
                                break;
                            case ArbitraryDataSlice.ADIDLength.UShort:
                                _packedData.AddRange(BitConverter.GetBytes((ushort)slice.ID));
                                break;
                            case ArbitraryDataSlice.ADIDLength.Int:
                                _packedData.AddRange(BitConverter.GetBytes((int)slice.ID));
                                break;
                        }
                    }

                    // Attach array length (if applicable),
                    if (isArray)
                    {
                        _packedData.AddRange(BitConverter.GetBytes(slice.Length));
                    }

                    // Then attach the data.
                    _packedData.AddRange(slice.data);
                }

                return _packedData.ToArray();
            }
        }

        /// <summary>
        /// Unpacks a byte array into an ArbitraryData.
        /// </summary>
        public ArbitraryData(byte[] packedData)
        {
            int index = 0;
            slices = new();

            while (index < packedData.Length - 1)
            {

                // Extract  isLittleEndian, idType, isArray, and dataTag from the header
                // Endianness might not need to be used? Could it just be set to endian instead?
                bool isLittleEndian = WWBinaryFunctions.ReadBit(packedData[index], 0);
                byte idType = 0;
                if (WWBinaryFunctions.ReadBit(packedData[index], 1))
                {
                    idType += 2;
                }
                if (WWBinaryFunctions.ReadBit(packedData[index], 2))
                {
                    idType += 1;
                }
                bool isArray = WWBinaryFunctions.ReadBit(packedData[index], 3);
                ArbitraryDataSlice.ADIDLength idLength = (ArbitraryDataSlice.ADIDLength)idType;
                byte dataTypeValue = 0;
                // Convert the last four bits, a nibble that represents the data tag, into a usable byte
                for (int i = 0; i < 4; i++)
                {
                    dataTypeValue += (byte)(WWBinaryFunctions.ReadBit(packedData[index], 7 - i) ? (1 << i) : 0);
                }
                ADDataTag dataTag = (ADDataTag)dataTypeValue;
                index += 1;

                // Get the ID if it exists
                int? id = null;
                switch (idLength)
                {
                    case ArbitraryDataSlice.ADIDLength.Nonexistent:
                        id = null;
                        break;
                    case ArbitraryDataSlice.ADIDLength.Byte:
                        id = packedData[index];
                        index += 1;
                        break;
                    case ArbitraryDataSlice.ADIDLength.UShort:
                        id = WWBinaryFunctions.RemoveShortSign(BitConverter.ToInt16(packedData, index));
                        index += 2;
                        break;
                    case ArbitraryDataSlice.ADIDLength.Int:
                        id = BitConverter.ToInt32(packedData, index);
                        index += 4;
                        break;
                }

                // Create the slice using the dataTag and ID
                ArbitraryDataSlice slice = new ArbitraryDataSlice(dataTag, id);

                // Add the data to the slice
                int lengthOfDataType = ArbitraryDataSlice.GetTagLength(dataTag);
                if (lengthOfDataType != 0)
                {
                    ushort amountOfIndices = 1;
                    if (isArray)
                    {
                        // If the slice had an array, read it and advance two bytes
                        amountOfIndices = WWBinaryFunctions.RemoveShortSign(BitConverter.ToInt16(packedData, index));
                        index += 2;
                    }
                    for (int i = 0; i < amountOfIndices; i++)
                    {
                        slice.data = slice.data.Concat(packedData.ToList().GetRange(index, lengthOfDataType)).ToArray();        // Could REALLY use some optimization
                        index += lengthOfDataType;
                    }
                }

                // Add the slice to slices
                slices.Add(slice);
            }
        }

        public ArbitraryDataSlice AddDataSlice<T>(ADDataType dataType, T data, int? id = null)
        {
            ArbitraryDataSlice sliceToAppend = new(dataType, id);
            sliceToAppend.AddData(data, dataType);
            slices.Add(sliceToAppend);
            return sliceToAppend;
        }
        
        public ArbitraryDataSlice AddEmptyDataSlice(int id)
        {
            ArbitraryDataSlice slice = new ArbitraryDataSlice(ADDataTag.Zero, id);
            slices.Add(slice);
            return slice;
        }

        public ArbitraryDataSlice AddFrameMarker()
        {
            ArbitraryDataSlice slice = new ArbitraryDataSlice(ADDataTag.FrameMarker);
            slices.Add(slice);
            return slice;
        }

        /// <summary>
        /// Checks if a slice exists within this data's slices. If so, returns its index; if not, returns null.
        /// </summary>
        /// <param name="slice"></param>
        /// <returns></returns>
        public int? GetIndexOfSlice(ArbitraryDataSlice slice)
        {
            for (int i = 0; i < slices.Count; i++)
            {
                if (slices[i] == slice)
                {
                    return i;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns true if this arbitrary data contains a slice with a given ID.
        /// </summary>
        public bool ContainsID(int id)
        {
            return TryGetID(id, out _);
        }

        /// <summary>
        /// Tries to find and cache the slice of an ID if it exists. If not, returns false.
        /// </summary>
        /// <param name="slice"> Cache to store the slice into. </param>
        /// <returns></returns>
        public bool TryGetID(int id, out ArbitraryDataSlice slice)
        {
            return GetIndexOfID(id, out slice) != null;
        }

        /// <summary>
        /// Tries to get the index of an ID. If it isn't found, returns null.
        /// </summary>
        public int? GetIndexOfID(int id, out ArbitraryDataSlice slice)
        {
            for (int i = 0; i < slices.Count; i++)
            {
                if (slices[i].ID == id)
                {
                    slice = slices[i];
                    return i;
                }
            }
            slice = null;
            return null;
        }

        /// <summary>
        /// Tries to get the index of an ID. If it isn't found, returns null.
        /// </summary>
        public int? GetIndexOfID(int id)
        {
            return GetIndexOfID(id, out _);
        }

        /// <summary>
        /// Tries to find and remove a slice of a given ID. If found, returns true; otherwise, returns false.
        /// </summary>
        public bool TryRemoveSliceWithID(int id)
        {
            for(int i=0; i<slices.Count; i++)
            {
                if(slices[i].ID == id)
                {
                    slices.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the first found slice with a given ID if it exists, otherwise returns null.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ArbitraryDataSlice GetSliceByID(int id)
        {
            for(int i=0; i<slices.Count; i++)
            {
                if(slices[i].ID == id)
                {
                    return slices[i];
                }
            }
            return null;
        }
    }
}
