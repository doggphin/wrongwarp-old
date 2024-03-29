using UnityEngine;
using Mirror;
using System;

namespace WrongWarp
{
    public class NetworkEntity : NetworkBehaviour
    {
        protected ArbitraryData frameDataCache = new();

        [field:SerializeField] public float DeathLength { get; private set; } = 0;
        [field: SerializeField] public bool IsKinetic { get; private set; } = true;
        protected bool interpolatePosition = true;
        protected bool isKillable = false;

        public bool InterpolatePosition
        {
            get
            {
                return interpolatePosition;
            }
            set
            {
                if(isLocalPlayer || isServer)
                {
                    InterpolatePosition = false;
                    Debug.Log("Tried to set interpolation on local player to true, ignoring and setting to false by default.");
                    return;
                }
                else
                {
                    interpolatePosition = InterpolatePosition;
                }
            }
        }

        private void Start()
        {
            if (isLocalPlayer || isServer)
            {
                IsKinetic = false;
                interpolatePosition = false;
            }
            TickManager.PollEntities += UploadPolledData;
        }

        private void Update()
        {
            if (IsKinetic)
            {
                (visualTransform != null ? visualTransform : transform).position = Vector3.Lerp(oldPosition, newAndCurrentPosition, (Time.time % Time.fixedDeltaTime) / Time.fixedDeltaTime);
            }
        }

        [Server]
        protected void UploadPolledData()
        {
            if(frameDataCache.slices.Count > 0)
            {
                Debug.Log("More than one slice; uploading info to manager.");
                TickManager.UploadEntityInformationToCache(this, frameDataCache);
                frameDataCache = new();
            }
        }

        private Vector3 oldPosition;
        private Vector3 newAndCurrentPosition;
        [SerializeField] private Transform visualTransform = null;

        [Server]
        public void ServerKill()
        {
            frameDataCache.AddEmptyDataSlice((int)FrameActionID.PlayKillAnimation);
            TickManager.QueueSpecialEntityAction(new TickTime(DeathLength).TotalFrames, this, SpecialEntityActions.Kill);
            ClientKill();
        }

        [Client]
        public virtual void ClientKill()
        {
            gameObject.SetActive(false);
        }

        [Client]
        public void ClientSetNewPosition(Vector3 position)
        {
            if(interpolatePosition && IsKinetic)
            {
                oldPosition = newAndCurrentPosition;
                newAndCurrentPosition = position;
            }
            else if (IsKinetic)
            {
                transform.position = position;
            }
            else
            {
                Debug.LogWarning("Tried to set position of NetworkEntity while it was not kinetic.");
            }
        }

        public enum BasicEntityToggles
        {
            IsKinetic,
            InterpolatePosition,
            IsKillable
        }
    }
}
