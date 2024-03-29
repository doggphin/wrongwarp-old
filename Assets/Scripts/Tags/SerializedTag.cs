using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace WrongWarp
{
    [Serializable]
    public class SerializedTag
    {
        [SerializeField] public ushort id;
        [SerializeField] public TagDataType dataType;

        [ShowIf("dataType", TagDataType.Int)]
        [AllowNesting]
        [SerializeField] public int intData;

        [ShowIf("dataType", TagDataType.UShort)]
        [AllowNesting]
        [SerializeField] public ushort ushortData;

        [ShowIf("dataType", TagDataType.Float)]
        [AllowNesting]
        [SerializeField] public float floatData;

        [ShowIf("dataType", TagDataType.Bool)]
        [AllowNesting]
        [SerializeField] public bool boolData;

        [ShowIf("dataType", TagDataType.String)]
        [AllowNesting]
        [SerializeField] public string stringData;
    }

    public enum TagDataType
    {
        Int,
        UShort,
        Float,
        Bool,
        String
    }
}
