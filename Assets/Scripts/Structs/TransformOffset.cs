using UnityEngine;

namespace WrongWarp
{
    public struct TransformOffset
    {
        public Transform t;
        public Vector3 offset;
        public Vector3 TotalOffset { get; private set; }

        public TransformOffset(Transform t, Vector3 offset)
        {
            this.t = t;
            this.offset = offset;
            TotalOffset = t.rotation * offset;
        }
    }
}
