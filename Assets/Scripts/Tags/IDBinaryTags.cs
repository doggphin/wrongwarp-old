using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace WrongWarp
{
    /// <summary>
    /// Short for ID-Binary Tags. Contains a (ushort, Tag) Dictionary 'tags' to hold arbitrary data as byte arrays and functions to read, add and remove them.
    /// </summary>
    public class IDBinaryTags
    {
        private Dictionary<ushort, Tag> idbTags;
        public Dictionary<ushort, Tag> IDBTags
        {
            get
            {
                if(idbTags == null) { idbTags = new(); }
                return idbTags;
            }
        }

        public IDBinaryTags(Dictionary<ushort, Tag> idbTags = null)
        {
            this.idbTags = idbTags;
        }

        /// <summary>
        /// Adds an ID and Tag to idbTags.
        /// </summary>
        /// <param name="id"> The ID of the tag to add. </param>
        /// <param name="data"> The data to store as bytes. Only allows string, int, float and bool. </param>
        /// <param name="replaceIfExists"> If this tag already exists, should it be replaced or should the operation be cancelled? </param>
        /// <returns> Returns whether the tag was successfully added. </returns>
        public bool AddTag<T>(ushort id, T[] data, TagExistsAction actionIfExists = TagExistsAction.Add)
        {
            // First, do checks for if tag ID already exists
            if (IDBTags.TryGetValue(id, out Tag existingTag))
            {
                if (actionIfExists == TagExistsAction.Skip) { return false; }
                if (actionIfExists == TagExistsAction.Add)
                {
                    // If tag already exists and doesn't have a null array of byte data, concat the data onto it.
                    if (existingTag.data != null)
                    {
                        idbTags[id] = new Tag(existingTag.data.Concat(WWTagConverter.GenericArrayToByteArray(data)).ToArray());
                        return true;
                    }
                    // Otherwise, the list is empty, so just fall through and treat as if nothing was there:
                }
            }

            // Set the tag ID's data to the data.
            idbTags[id] = new Tag(WWTagConverter.GenericArrayToByteArray(data));
            return true;
        }

        public void AddSerializedTags(SerializedTag[] serializedTags)
        {
            foreach(SerializedTag serializedTag in serializedTags)
            {
                switch(serializedTag.dataType)
                {
                    case TagDataType.Int:
                        AddTag(serializedTag.id, new int[] { serializedTag.intData });
                        break;
                    case TagDataType.Bool:
                        AddTag(serializedTag.id, new bool[] { serializedTag.boolData });
                        break;
                    case TagDataType.Float:
                        AddTag(serializedTag.id, new float[] { serializedTag.floatData });
                        break;
                    case TagDataType.String:
                        AddTag(serializedTag.id, new string[] { serializedTag.stringData });
                        break;
                    case TagDataType.UShort:
                        AddTag(serializedTag.id, new ushort[] { serializedTag.ushortData });
                        break;
                    default:
                        break;
                }
            }
        }

    }
}
