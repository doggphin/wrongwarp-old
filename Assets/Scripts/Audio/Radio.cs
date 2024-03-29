using System.Collections;
using UnityEngine;

namespace WrongWarp
{
    public class Radio : MonoBehaviour
    {
        [SerializeField] AudioClip clip;

        private void Start()
        {
            StartCoroutine(SoundPlayer());
        }

        private IEnumerator SoundPlayer()
        {
            PlaySound();
            yield return WWHelpers.GetWait(clip.length);
            StartCoroutine(SoundPlayer());
        }

        private void PlaySound()
        {
            AudioManager.PlaySound(transform, clip, 15);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                StopAllCoroutines();
                WWHelpers.DeleteChildren(transform);
            }
        }
    }
}
