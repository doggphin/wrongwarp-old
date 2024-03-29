using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WrongWarp
{
    public static class PortalPhysics
    {
        public static bool Raycast(Vector3 origin, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask = 0)
        {
            layerMask |= 1 << 29;         // Include layer 29 with layerMask

            hitInfo = default;

            Collider lastPortalCollider = null;
            while (maxDistance > 0)
            {
                if(lastPortalCollider != null) { lastPortalCollider.enabled = false; }

                Debug.DrawRay(origin, direction, Color.red, 5, false);
                if(!Physics.Raycast(origin, direction, out hitInfo, maxDistance, layerMask))
                {
                    break;
                }

                if (hitInfo.transform.gameObject.layer == 29)
                {
                    maxDistance -= hitInfo.distance;

                    Portal hitPortal = hitInfo.transform.gameObject.GetComponentInParent<Portal>();
                    if(hitPortal == null) { Debug.LogError("Gameobject marked layer 29 (Portal) missing Portal component."); break; }

                    if (lastPortalCollider != null) { lastPortalCollider.enabled = true; }
                    lastPortalCollider = hitPortal.portalCollider;
                    origin = GetNewOrigin(hitPortal, hitInfo);
                    direction = GetNewDirection(hitPortal, direction, hitInfo);
                }
                else
                {
                    break;
                }
            }

            if (lastPortalCollider != null) { lastPortalCollider.enabled = true; }
            return hitInfo.collider != null;
        }

        public static bool SphereCast(Vector3 origin, float radius, Vector3 direction, out RaycastHit hitInfo, float maxDistance, int layerMask = 0)
        {
            // Do a spherecast on layerMask and portal layer (29)
            bool didHit = Physics.SphereCast(origin, radius, direction, out hitInfo, layerMask | 1 << 29);

            // Check if spherecast hit nothing OR hit something that isn't a portal
            if((!didHit) || (didHit && hitInfo.collider.gameObject.layer != 29))
            {
                // If so, treat as a normal spherecast
                return didHit;
            }

            // Otherwise this involves portals, so do extra calculations
            // Hit layer was 29
            while (maxDistance > 0)
            {
                // Check if spherecast could have gone max distance
                if(Physics.Raycast(origin, direction, out RaycastHit raycastHit, maxDistance, 1 << 29))
                {
                    // If it could have, raycast to max distance while ignoring the portal layer
                    Physics.SphereCast(origin, radius, direction, out RaycastHit sphereCastHit, layerMask & ~(1 << 29));
                    if(sphereCastHit.distance < raycastHit.distance)
                    {
                        // If there was a collider found farther than the original spherecast but not behind the portal, 
                        hitInfo = sphereCastHit;
                        return true;
                    }
                }
            }

            return false;
        }

        private static Vector3 GetNewOrigin(Portal hitPortal, RaycastHit hit)
        {
            // Refer to notes
            Vector3 newOrigin = hit.point - hit.transform.position;
            newOrigin = Quaternion.Inverse(hit.transform.rotation) * newOrigin;
            newOrigin = hitPortal.linkedPortal.transform.rotation * newOrigin;
            newOrigin += hitPortal.linkedPortal.transform.position;
            return newOrigin;
        }

        private static Vector3 GetNewDirection(Portal hitPortal, Vector3 direction, RaycastHit hit)
        {
            Vector3 newDirection = Quaternion.Inverse(hitPortal.transform.rotation) * direction;
            newDirection = hitPortal.linkedPortal.transform.rotation * newDirection;
            return newDirection;
        }
    }
}
