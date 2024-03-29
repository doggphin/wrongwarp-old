using System.Collections.Generic;
using UnityEngine;

namespace WrongWarp
{
    public class SFXLibrary : MonoBehaviour
    {
        public static SFXLibrary Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }



        public static AudioClip GetAudioClip(SFXLibrarySFX soundEffect)
        {
            return Instance.EnumToSFX(soundEffect);
        }
        


        private AudioClip EnumToSFX(SFXLibrarySFX soundEffect)
        {
            return soundEffect switch
            {
                SFXLibrarySFX._5000SoundsBlocklandAlert => _5000SoundsBlocklandAlert,
                SFXLibrarySFX._5000SoundsRepetitiveAlert => _5000SoundsRepetitiveAlert,
                SFXLibrarySFX.BruhSoundEffect2 => bruhSoundEffect2,
                _ => errorSound,
            };
        }



        public enum SFXLibrarySFX : int
        {
            ErrorSound = 0,
            _5000SoundsBlocklandAlert = 1,
            _5000SoundsRepetitiveAlert = 2,
            BruhSoundEffect2 = 3,
        }



        [Header("Error")]
        [SerializeField] private AudioClip errorSound;

        [Header("Beeps")]
        [SerializeField] private AudioClip _5000SoundsBlocklandAlert;
        [SerializeField] private AudioClip _5000SoundsRepetitiveAlert;

        [Header("Stupid Stuff")]
        [SerializeField] private AudioClip bruhSoundEffect2;
    }

}
