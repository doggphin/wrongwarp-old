using UnityEngine;

namespace WrongWarp
{
    public class PlayerPortalTraveller : PortalTraveller
    {
        public override void Teleport(Transform fromPortal, Transform toPortal, Vector3 pos, Quaternion rot)
        {
            transform.position = pos;
            transform.rotation = rot;
            //Vector3 newVelocity = toPortal.TransformVector(fromPortal.InverseTransformVector(new Vector3(movingScript.newVelocity.x, 0, movingScript.newVelocity.y)));
            //movingScript.newVelocity = new Vector2(newVelocity.x, newVelocity.z);
            Physics.SyncTransforms();
            AudioManager.RecalibrateAudioPlayers();
        }
    }
}
