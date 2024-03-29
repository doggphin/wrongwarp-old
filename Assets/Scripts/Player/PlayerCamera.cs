using UnityEngine;

namespace WrongWarp
{
    public class PlayerCamera : MonoBehaviour
    {
        public void InitAsClient()
        {
            enabled = true;
            gameObject.SetActive(true);
            GetComponent<AudioListener>().enabled = true;
            GetComponent<Camera>().enabled = true;
            GetComponent<InteractRaycast>().InitAsClient();
        }
    }
}