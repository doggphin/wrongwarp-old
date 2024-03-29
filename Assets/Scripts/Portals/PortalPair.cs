using UnityEngine;
using Mirror;

namespace WrongWarp
{
    public class PortalPair : NetworkBehaviour
    {
        [SerializeField] private Portal portalA;
        [SerializeField] private Portal portalB;
        private Vector3 antiGroundClipOffset = Vector3.up * 0.11f;

        public override void OnStartClient()
        {
            if(isServer) { return; }
            portalA.Init();
            portalB.Init();
        }

        [Server]
        public override void OnStartServer()
        {
            portalA.Init();
            portalB.Init();
        }

        [Server]
        public void SetPortalPosition(PortalPairSelect selection, Vector3 position, Quaternion rotation)
        {
            Transform selectedPortalTransform = selection == PortalPairSelect.PortalA ? portalA.gameObject.transform : portalB.gameObject.transform;
            selectedPortalTransform.SetPositionAndRotation(position + antiGroundClipOffset, rotation);
        }
    }

    public enum PortalPairSelect
    {
        PortalA,
        PortalB
    }
}
