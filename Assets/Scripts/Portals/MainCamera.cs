using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using RenderPipeline = UnityEngine.Rendering;

// Blatantly copied from https://www.youtube.com/watch?v=cWpFZbjtSQg&ab_channel=SebastianLague
namespace WrongWarp
{
    [RequireComponent(typeof(Camera))]
    public class MainCamera : MonoBehaviour {

        public List<Portal> portals = new();
        public static Action<Camera> mainCameraInstantiated = delegate { };

        private void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += UpdateCamera;
        }
        public void Init ()
        {
            SearchForPortals();
            //mainCameraInstantiated(gameObject.GetComponent<Camera>());
            WWHelpers.playerCamera = GetComponent<Camera>();
        }

        public void SearchForPortals()
        {
            portals = new List<Portal>(FindObjectsOfType<Portal>());
        }

        private void UpdateCamera(ScriptableRenderContext SRC, Camera camera)
        {
            for (int i = 0; i < portals.Count; i++)
            {
                if(portals[i].playerCam == null)
                {
                    portals[i].playerCam = GetComponent<Camera>();
                }
            }

            for (int i = 0; i < portals.Count; i++) {
                portals[i].PrePortalRender();
            }
            for (int i = 0; i < portals.Count; i++) {
                portals[i].Render(SRC);
            }

            for (int i = 0; i < portals.Count; i++) {
                portals[i].PostPortalRender();
            }
        }
    }
}