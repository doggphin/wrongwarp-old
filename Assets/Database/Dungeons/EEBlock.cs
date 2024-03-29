using UnityEngine;

namespace WrongWarp
{

    [CreateAssetMenu(fileName = "New EEBlock", menuName = "Assets/EEBlock")]
    public class EEBlock : ScriptableObject
    {
        public GameObject model;
        public EEBlockConnection[] connections;
    }
}
