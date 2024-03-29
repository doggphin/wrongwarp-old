using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WrongWarp
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : MonoBehaviour
    {
        public static Action<AudioPlayer> AudioPlayerFinished = delegate { };

        /// <summary>
        /// ParentTransform is the actual location of this AudioPlayer as far as the AudioManager is concerned.
        /// </summary>
        public Transform ParentTransform => parentTransform;
        [SerializeField] private Transform parentTransform;

        /// <summary>
        /// Cached GetComponent AudioSource
        /// </summary>
        public AudioSource AudioSource => audioSource;
        private AudioSource audioSource;

        /// <summary>
        /// Cached GetComponent AudioLowPassFilter
        /// </summary>
        public AudioLowPassFilter AudioLowPassFilter => audioLowPassFilter;
        [SerializeField] private AudioLowPassFilter audioLowPassFilter;

        [HideInInspector] public APData data;

        [HideInInspector] public bool isPlaying;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioLowPassFilter = GetComponent<AudioLowPassFilter>();
        }

        private void Update()
        {
            if(data.Equals(default(APData))) { return; }

            data.lifeRemaining -= Time.deltaTime;
            if (data.lifeRemaining <= 0)
            {
                AudioPlayerFinished(this);
            }

            if(isPlaying && AudioSource.time >= data.clipCutoffTime)
            {
                AudioSource.Stop();
            }
        }

        public void Init(AudioClip clip)
        {
            isPlaying = false;

            parentTransform.gameObject.name = $"'{clip.name}' Audio Source";
            gameObject.name = $"'{clip.name}' Audio Player";
            AudioSource.clip = clip;
        }

        /// <summary>
        /// Plays a sound at a given time offset.
        /// </summary>
        public void PlayClip(float timeToStartClipAt = 0)
        {
            isPlaying = true;

            if(AudioSource.clip == null)
            {
                Debug.LogError("Clip not set, cannot play sound.");
                return;
            }
            if(timeToStartClipAt < 0)
            {
                Debug.LogError("Can't start clip at a negative time.");
                return;
            }

            AudioSource.time = timeToStartClipAt;
            Debug.Log($"Playing sound starting at {timeToStartClipAt}");
            AudioSource.Play();
        }
             
        private void OnDestroy()
        {
            AudioPlayerFinished(this);
        }
    }

    public struct APData
    {
        /// <summary>
        /// If not null, will dictate when clip needs to stop playing.
        /// <para> Used for when a sound is cut off or a receiver is disabled while playing a source. </para>
        /// </summary>
        public float? clipCutoffTime;

        public float lifeRemaining;

        // Previously struct ActiveAPInfo
        public TransformOffset transformOffset;
        public float timeOfCreation;

        // Previously class ROAPData
        public ROAPData roAPData;

        public List<AudioPlayer> roAPChildren;

        public float maxDistance;

        public float muffledness;

        public APData(float lifetime, TransformOffset transformOffset, float timeOfCreation, ROAPData roAPData, float maxDistance, float initialLifetime = 0)
        {
            clipCutoffTime = null;

            lifeRemaining = Mathf.Max(lifetime - initialLifetime, 0);

            this.transformOffset = transformOffset;
            this.timeOfCreation = timeOfCreation;
            this.roAPData = roAPData;

            roAPChildren = new();

            this.maxDistance = maxDistance;
            muffledness = 0;
        }
    }

    public class ROAPData
    {
        public AudioPlayer receivedAudioPlayer;
        public AudioReceiver audioReceiver;
        public OutputSource outputSource;

        public ROAPData(AudioPlayer receivedAudioPlayer, AudioReceiver audioReceiver, OutputSource outputSource)
        {
            this.receivedAudioPlayer = receivedAudioPlayer;
            this.audioReceiver = audioReceiver;
            this.outputSource = outputSource;
        }
    }
}
