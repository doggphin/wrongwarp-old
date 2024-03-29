using UnityEngine;

namespace WrongWarp
{
    public class Billboard : MonoBehaviour
    {
        public bool stayVertical = false;
        public bool staticBillboard = true;
        public Vector3 vertical = Vector3.up;
        public Transform transformToFollow = null;

        public void Init(Transform transform, Vector3? _vertical = null, bool isStaticBillboard = true)
        {
            transformToFollow = transform;
            transform.position = transformToFollow.position;
            FinishInit(_vertical, isStaticBillboard);
        }
        public void Init(Vector3 position, Vector3? _vertical = null, bool isStaticBillboard = true)
        {
            transform.position = position;
            FinishInit(_vertical, isStaticBillboard);
        }

        private void FinishInit(Vector3? _vertical, bool isStaticBillboard)
        {
            if (_vertical != null)
            {
                vertical = (Vector3)_vertical;
                stayVertical = true;
            }
            staticBillboard = isStaticBillboard;
        }

        [field:SerializeField] public SpriteRenderer SpriteRenderer { get; private set; }

        private void LateUpdate()
        {
            if(transformToFollow != null)
            {
                transform.position = transformToFollow.position;
            }

            if(staticBillboard)
            {
                transform.rotation = WWHelpers.playerCamera.transform.rotation;
            }
            else
            {
                transform.LookAt(WWHelpers.playerCamera.transform);
            }
            
            if(stayVertical)
            {
                transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
            }
        }
    }
}
