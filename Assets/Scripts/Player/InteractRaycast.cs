using UnityEngine;

namespace WrongWarp
{
    /// <summary> Attach this to a player object and call checkForItems to find interactable items in a radius in front of it </summary>
    public class InteractRaycast : MonoBehaviour
    {
        [SerializeField] float interactRadius;
        [SerializeField] float interactDistance;
        [SerializeField] GameObject testSphere;
        private GameObject testSphereInstance;
        private GameObject TestSphere
        {
            get
            {
                if(testSphereInstance == null)
                {
                    testSphereInstance = Instantiate(testSphere);
                }
                return testSphereInstance;
            }
            set
            {
                testSphereInstance = TestSphere;
            }
        }

        private bool interactThisFrame = false;

        public void InitAsClient()
        {
            // In the future, this will need to be called every frame (or maybe every other) to draw the outline shader.
            //InputManager.PlayerControls.PlayerInteract.Interact.started += ctx => RaycastInteract();
            InputManager.PlayerControls.PlayerInteract.Interact.started += ctx => interactThisFrame = true;
            TickManager.PollInput += SetInputs;
        }

        private void SetInputs(InputPacket packet)
        {
            packet.SetInputValue(InputPacket.InputButton.Interact, interactThisFrame);
            if(interactThisFrame) { Debug.Log("Trying to interact."); }
            interactThisFrame = false;
        }

        /// <summary> Sends out a spherecast from a point with a direction and returns the hit object's Interactable component. <para>Returns null if nothing is found.</para><para>Defaults to 'Self'(6) excluding layer mask.</para> </summary>
        /// Needs to be gameobject to send between server and client.
        public void RaycastInteract(int layerMask = 1 << 3)
        {
            //PortalPhysics.Raycast(transform.position, transform.forward, out RaycastHit test, layerMask);
            //TestSphere.transform.position = test.point;
            if (PortalPhysics.Raycast(transform.position, transform.forward, out RaycastHit test2, 5, layerMask))
            {
                Debug.Log("Hit something");
                TestSphere.transform.position = test2.point;
                if (test2.collider.TryGetComponent(out IInteractable interactable2))
                {
                    Debug.Log("Hit an interactable");
                    interactable2.CmdInteract();
                    return;
                }
            }
            //if (Physics.SphereCast(transform.position, interactRadius, transform.forward, out RaycastHit hit, interactDistance, layerMask) && hit.collider.TryGetComponent(out IInteractable interactable))
            //{
            //    interactable.CmdInteract();
            //    return;
            //}
            Debug.Log("Didn't find an interactable.");
        }
    }
}
