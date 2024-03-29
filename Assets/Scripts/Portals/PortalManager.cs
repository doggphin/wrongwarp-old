using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System;

namespace WrongWarp
{
    public class PortalManager : MonoBehaviour
    {
        [SerializeField] private GameObject portalPairObject;

        private static GameObject staticPortalPairObject;
        private static List<PortalPair> portalPairs = new();

        private void Awake()
        {
            staticPortalPairObject = portalPairObject;
            DontDestroyOnLoad(gameObject);

        }

        [Server]
        public static void CreatePortalPair(Vector3 portalAPosition, Quaternion portalARotation, Vector3 portalBPosition, Quaternion portalBRotation, Vector3? scale = null)
        {
            // Spawned slightly above the ground to stop Z fighting
            PortalPair portalPair = Instantiate(staticPortalPairObject, Vector3.zero, new Quaternion()).GetComponent<PortalPair>();
            portalPair.SetPortalPosition(PortalPairSelect.PortalA, portalAPosition, portalARotation);
            portalPair.SetPortalPosition(PortalPairSelect.PortalB, portalBPosition, portalBRotation);
            portalPairs.Add(portalPair);

            NetworkServer.Spawn(portalPair.gameObject);
        }
    }
}
