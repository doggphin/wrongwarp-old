using System;
using System.Collections.Generic;

namespace WrongWarp
{

    /// <summary>
    /// Contains data as an array of bytes.
    /// </summary>
    public struct Tag
    {

        /// <summary>
        /// The data stored within this this tag.
        /// </summary>
        public byte[] data;

        public Tag(byte[] data)
        {
            this.data = data;
        }

        public int[] DataAsInts
        {
            get
            {
                List<int> intsToReturn = new();
                for (int i = 0; i < data.Length / 4; i++)
                {
                    intsToReturn.Add(BitConverter.ToInt32(data, i * 4));
                }
                return intsToReturn.ToArray();
            }
        }

        public ushort[] DataAsUShorts
        {
            get
            {
                List<ushort> ushortsToReturn = new();
                for (int i = 0; i < data.Length / 2; i++)
                {
                    ushortsToReturn.Add(BitConverter.ToUInt16(data, i * 2));
                }
                return ushortsToReturn.ToArray();
            }
        }

        public float[] DataAsFloats
        {
            get
            {
                List<float> floatsToReturn = new();
                for (int i = 0; i < data.Length / 4; i++)
                {
                    floatsToReturn.Add(BitConverter.ToSingle(data, i * 4));
                }
                return floatsToReturn.ToArray();
            }
        }

        public bool[] DataAsBools
        {
            get
            {
                List<bool> boolsToReturn = new();
                for (int i = 0; i < data.Length; i++)
                {
                    boolsToReturn.Add(BitConverter.ToBoolean(data, i));
                }
                return boolsToReturn.ToArray();
            }
        }

        /// <summary>
        /// Strings must be '$' terminated.
        /// </summary>
        public string[] DataAsStrings
        {
            get
            {
                List<string> stringsToReturn = new();
                List<char> charCache = new();
                for (int i = 0; i < data.Length; i++)
                {
                    char currentChar = BitConverter.ToChar(data, i);
                    if (currentChar == '$')
                    {
                        stringsToReturn.Add(new string(charCache.ToArray()));
                        charCache = new();
                    }
                    else
                    {
                        charCache.Add(currentChar);
                    }
                }
                if (charCache.Count > 0)
                {
                    stringsToReturn.Add(new string(charCache.ToArray()));
                }
                return stringsToReturn.ToArray();
            }
        }
    }
}