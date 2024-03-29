using System.Collections.Generic;
using UnityEngine;

namespace WrongWarp
{
    [CreateAssetMenu(fileName = "New EEBlockSet", menuName = "Assets/EEBlockSet")]
    public class EEBlockSet : ScriptableObject
    {
        public EEBlock[] blocks;
    }
}
