using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace WrongWarp
{

    /// <summary>
    /// Singleton used to play advanced audio.
    /// <para> Supports audio broadcasting and receiving for echoes. </para>
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class AudioManager : NetworkBehaviour
    {
        // Constants
        /// <summary>
        /// The speed at which audio broadcasts are sent to the player and to AudioReceivers.
        /// </summary>
        public float SpeedOfSound => speedOfSound;
        [SerializeField] private float speedOfSound = 343;
        /// <summary>
        /// The doppler level shared by all AudioPlayers.
        /// </summary>
        public float DopplerLevel => dopplerLevel;
        [SerializeField] private float dopplerLevel = 0.3f;
        /// <summary>
        /// The amount of floats that can be stored in AudioPlayers' MufflednessCache.
        /// </summary>
        public static float MufflednessInc => Instance.mufflednessInc;
        [SerializeField] private float mufflednessInc = 0.03f;
        /// <summary>
        /// The squared distance Receiver-Output AudioPlayers' positions should stop being updated every frame and revert to a default position.
        /// </summary>
        [Tooltip("At what distance should Receiver-Output AudioReceivers' positions stop being calculated and use a point instead?")]
        [SerializeField] private float squaredReceiverOutputRangeThreshhold = 400;
        /// <summary>
        /// How often, in seconds, to check whether Receiver-Output AudioReceivers are in active range.
        /// </summary>
        [Tooltip("How often, in seconds, should Receiver-Output AudioReceivers be checked for whether or not they're in active range?")]
        [SerializeField] private float receiverOutputRangeCheckRate = 1;
        [Space]

        // AudioPlayer references
        /// <summary>
        /// A pool of AudioPlayer prefabs used to create audio sources.
        /// </summary>
        private ObjectPool<AudioPlayer> apPool;
        /// <summary>
        /// All audio players currently taken from the pool.
        /// </summary>
        private List<AudioPlayer> activeAPs = new();
        /// <summary>
        /// All activeAudioPlayers' coroutines that represent them playing their clip in the future.
        /// </summary>
        private Dictionary<AudioPlayer, Coroutine> audioPlayersWaitingToPlayClip = new();
        /// <summary>
        /// All Receiver-Output AudioPlayers that are being updated every frame.
        /// </summary>
        private List<AudioPlayer> activeROAPs = new();

        // Other references
        [SerializeField] private GameObject audioPlayerPrefab;
        private BoxCollider testingPlaneCollider;
        public static AudioManager Instance;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            apPool = new(
                () => { return OnCreatePooledItem(); },
                audioPlayer => { OnAccessFromPool(audioPlayer); },
                audioPlayer => { OnReleasedToPool(audioPlayer); },
                audioPlayer => { OnDestroyPooledObject(audioPlayer); },
                true, 10, 100);

            testingPlaneCollider = GetComponent<BoxCollider>();
            testingPlaneCollider.size = new Vector3(1, 1, 0);

            AudioPlayer.AudioPlayerFinished += HandleAudioPlayerFinished;
        }

        // ============================================================
        // Audio Management
        // ============================================================

        #region Audio Management

        /// <summary>
        /// Uses an AudioPlayer to play a sound clip.
        /// <para> Audio created using this function will be delayed based on distance and can be picked up by AudioReceivers. </para>
        /// </summary>
        public static void PlaySound(Vector3 position, AudioClip clip, float maxDistance)
        {
            CreateAudioSource(Instance.transform, clip, maxDistance, position);
        }

        public static void PlaySound(Transform t, AudioClip clip, float maxDistance, Vector3 offset = default)
        {
            CreateAudioSource(t, clip, maxDistance, offset);
        }

        /// <summary>
        /// Uses an AudioPlayer to play a sound clip.
        /// <para> Audio created using this function will be delayed based on distance and can be picked up by AudioReceivers. </para>
        /// </summary>
        private static void CreateAudioSource(Transform t, AudioClip clip, float maxDistance, Vector3 offset = default, APData parentAPData = default, ROAPData roAPData = null)
        {
            if (t == null) { t = Instance.transform; }

            // Set soundPosition to transform.position, and if offset was used, include that too
            Vector3 worldPos = t.TransformPoint(offset);

            // * Start initializing AudioPlayer

            AudioPlayer ap = Instance.apPool.Get();
            ap.Init(clip);
            //audioPlayer.AudioSource.maxDistance = maxDistance;
            ap.transform.localPosition = Vector3.zero;
            ap.ParentTransform.parent = t ? t : Instance.transform;
            ap.ParentTransform.localPosition = offset;


            ap.data = new APData(
                ap.AudioSource.clip.length + (maxDistance / Instance.SpeedOfSound),
                new TransformOffset(t, offset), Time.time, roAPData, maxDistance, 0);

            MuffleCheckAP(ap, true);

            if (roAPData != null && AudioPlayerInRange(ap))
            {
                Instance.activeROAPs.Add(ap);
            }

            if (!parentAPData.Equals(default(APData)))
            {
                parentAPData.roAPChildren.Add(ap);
                if (parentAPData.clipCutoffTime != null)
                {
                    ap.data.clipCutoffTime = parentAPData.clipCutoffTime;
                }
            }

            Instance.audioPlayersWaitingToPlayClip.Add(
                ap,
                Instance.StartCoroutine(
                    PlayClipAfterDelay(
                        ap,
                        // Sound will kill itself soon as it would finish playing at its maximum range
                        // To make it play if player enters range of a sound after it starts playing, play it as if it was at the max distance
                        // Sound won't make any sound until player enters range, so this works
                        Mathf.Min(DelayBetweenPoints(WWHelpers.playerCamera.transform.position, ap.ParentTransform.position), maxDistance / Instance.speedOfSound)
                        )
                    )
                );

            // * End initializing AudioPlayer

            // Check all the receivers this sound will reach
            SetROColliderActive(false);
            Collider[] audioReceivers = Physics.OverlapSphere(worldPos, Mathf.Max(maxDistance, 0), 1 << 28);
            SetROColliderActive(true);

            // For each receiver in range, play sound after a delay
            for (int i = 0; i < audioReceivers.Length; i++)
            {
                // In case the gameObject creating this sound is also a receiver, escape infinite loop
                if (audioReceivers[i].transform == t) { continue; }

                if (!audioReceivers[i].TryGetComponent(out AudioReceiver audioReceiver))
                {
                    Debug.LogError("Layer marked 28 did not have AudioReceiver component.");
                }

                Vector3 arPos = audioReceivers[i].transform.position;

                float maxDistanceAfterReceived = maxDistance - (arPos - worldPos).magnitude;
                if (maxDistanceAfterReceived < 0) { continue; }

                for (int j = 0; j < audioReceiver.OutputSources.Length; j++)
                {
                    Instance.StartCoroutine(
                        CreateSourceAfterDelay(
                            audioReceiver.OutputSources[j].outputTransform, clip, maxDistanceAfterReceived, Vector3.zero, ap.data,
                            new ROAPData(ap, audioReceiver, audioReceiver.OutputSources[j]), DelayBetweenPoints(worldPos, arPos)));
                }
            }

            // Enables or disables outputSource's collider. Necessary for objects that both make sound and receive sound.
            void SetROColliderActive(bool enabled)
            {
                if (roAPData != null)
                {
                    roAPData.audioReceiver.AudioCollider.enabled = enabled;
                }
            }
        }

        /// <summary>
        /// Calls PlayClip() on an audioPlayer after a delay.
        /// </summary>
        private static IEnumerator PlayClipAfterDelay(AudioPlayer ap, float delay)
        {
            yield return new WaitForSeconds(delay);
            Instance.audioPlayersWaitingToPlayClip.Remove(ap);
            ap.PlayClip();
            MuffleCheckAP(ap, true);
        }

        /// <summary>
        /// Creates an audio source after a delay.
        /// </summary>
        private static IEnumerator CreateSourceAfterDelay(Transform t, AudioClip clip, float maxDistance, Vector3 offset, APData parentAPData, ROAPData roapData, float delay)
        {
            yield return new WaitForSeconds(delay);
            CreateAudioSource(t, clip, maxDistance, offset, parentAPData, roapData);
        }

        /// <summary>
        /// Safely releases an audio player back to the pool.
        /// </summary>
        private void HandleAudioPlayerFinished(AudioPlayer audioPlayer)
        {
            if (!Instance.activeAPs.Contains(audioPlayer))
            {
                Debug.Log("activeAudioPlayers did not include the audioPlayer that just finished.");
                return;
            }

            // If audioPlayer stopped prematurely, update all its children to have a new cutoffTime
            // Could be optimized not to update all children if cutoff was intentional
            float timePassed = Time.time - audioPlayer.data.timeOfCreation;
            if (timePassed < audioPlayer.AudioSource.clip.length)
            {
                for (int i = 0; i < audioPlayer.data.roAPChildren.Count; i++)
                {
                    audioPlayer.data.roAPChildren[i].data.clipCutoffTime = timePassed;
                }
            }

            Instance.activeROAPs.Remove(audioPlayer);
            Instance.apPool.Release(audioPlayer);
        }

        /// <summary>
        /// Call when the player has teleported a large distance. Restarts all active sounds.
        /// </summary>
        public static void RecalibrateAudioPlayers()
        {
            for(int i=0; i<Instance.activeAPs.Count; i++)
            {
                Instance.activeAPs[i].AudioSource.dopplerLevel = 0;
            }

            foreach (var pair in Instance.audioPlayersWaitingToPlayClip)
            {
                Instance.StopCoroutine(pair.Value);
            }
            Instance.audioPlayersWaitingToPlayClip = new();

            // Iterate through every playing sound and recalibrate what time they should be playing at for a new position.
            foreach (AudioPlayer ap in Instance.activeAPs)
            {
                ap.AudioSource.dopplerLevel = 0;
                ap.AudioSource.Stop();

                float delay = Mathf.Min(DelayBetweenPoints(ap.ParentTransform.position, WWHelpers.playerCamera.transform.position), ap.AudioSource.maxDistance / Instance.SpeedOfSound);
                float timePassed = Time.time - ap.data.timeOfCreation;

                // positive timeOffset is how long to wait until playing the sound
                // negative timeOffset is how far to skip the sound forward
                float timeOffset = delay - timePassed;
                if (timeOffset >= 0)
                {
                    Instance.audioPlayersWaitingToPlayClip.Add(ap, Instance.StartCoroutine(PlayClipAfterDelay(ap, timeOffset)));
                }
                else
                {
                    if (-timeOffset > ap.AudioSource.clip.length)      // If sound is already over at this position, don't bother starting to play it again
                    {
                        continue;
                    }
                    ap.PlayClip(-timeOffset);
                    MuffleCheckAP(ap, true);
                }

                ap.AudioSource.dopplerLevel = Instance.DopplerLevel;
            }

            for (int i = 0; i < Instance.activeAPs.Count; i++)
            {
                Instance.activeAPs[i].AudioSource.dopplerLevel = Instance.dopplerLevel;
            }

            RecalibrateLazyROAPs();
        }

        #endregion Audio Management

        // ============================================================
        // Audio Position Management
        // ============================================================

        #region Audio Position Management

        private float timeSinceLastROCheck = 0;
        private AudioInformation audioInformationCache;
        /// <summary>
        /// Every update, move all activeROAudioPlayers to their correct position. Asynchronously check if it's time to update RO positions.
        /// </summary>
        private void Update()
        {
            foreach (AudioPlayer roAP in activeROAPs)
            {
                audioInformationCache = GetPerceivedOutputPosition(roAP);
                roAP.transform.position = audioInformationCache.position;
            }

            timeSinceLastROCheck += Time.deltaTime;
            if (timeSinceLastROCheck >= receiverOutputRangeCheckRate)
            {
                RecalibrateLazyROAPs();
                timeSinceLastROCheck = 0;
            }
        }

        private void FixedUpdate()
        {
            for(int i=0; i<  Instance.activeAPs.Count; i++)
            {
                if(Instance.activeAPs[i].data.roAPData != null) { return; }
                MuffleCheckAP(Instance.activeAPs[i]);
            }
        }

        /// <summary>
        /// Smoothly modifies an AudioPlayer's muffledness.
        /// </summary>
        /// <param name="fullCheck"> Should this reset the sound back to a muffledness of 1/0? </param>
        /// <param name="isBlockedOverride"> Can be used to skip linecasting between the source and player and use a given condition instead. </param>
        private static void MuffleCheckAP(AudioPlayer ap, bool fullCheck = false, bool? isBlockedOverride = null)
        {
            bool isBlocked = isBlockedOverride == null ? Physics.Linecast(WWHelpers.playerCamera.transform.position, ap.transform.position, 1 << 31) : (bool)isBlockedOverride;
            float targetMuffledness = isBlocked ? 1 : 0;

            if(fullCheck)
            {
                // If sound is currently being smoothed, don't completely switch it
                // Might be a good idea to make this a parameter instead
                if(ap.data.muffledness != targetMuffledness) { ap.data.muffledness = targetMuffledness; }
                return;
            }
            else
            {
                if (isBlocked)
                {
                    ap.data.muffledness = Mathf.Min(ap.data.muffledness + MufflednessInc, 1);
                }
                else
                {
                    ap.data.muffledness = Mathf.Max(ap.data.muffledness - MufflednessInc, 0);
                }
            }

            ap.AudioLowPassFilter.cutoffFrequency = 22000 - (21400 * Mathf.Pow(ap.data.muffledness, 0.1f));
        }

        private static bool MuffleLinecast(Vector3 position)
        {
            return Physics.Linecast(WWHelpers.playerCamera.transform.position, position, 1 << 31);
        }

        /// <summary>
        /// Finds where to play an AudioReceiver's output sound.
        /// </summary>
        /// <param name="pos"> The world space position of an audio source. </param>
        /// <param name="audioReceiver"> The AudioReceiver that wants to broadcast a sound. </param>
        /// <param name="outputSource"> The OutputSource the AudioReceiver wants to broadcast to. </param>
        /// <returns> The world space position to move the sound to. </returns>
        private AudioInformation GetPerceivedOutputPosition(AudioPlayer audioPlayer)
        {
            AudioInformation returnInfo = new();
            ROAPData roAPData = audioPlayer.data.roAPData;

            switch(roAPData.audioReceiver.ColliderShape)
            {
                case ColliderShape.Plane:

                    // If receiver is a plane, it can have several different output types
                    AudioSpacialType ast = roAPData.outputSource.spacialType;

                    // Plane to point results in it playing on a point
                    if(ast == AudioSpacialType.Point)
                    {
                        returnInfo.position = roAPData.outputSource.outputTransform.position;
                    }
                    else
                    {
                        returnInfo.position = WWGeometry.ChangeOriginOfPoint(
                                roAPData.receivedAudioPlayer.transform.position,
                                roAPData.audioReceiver.transform,
                                roAPData.outputSource.outputTransform);

                        // Shared space unconditionally just plays at the given location normally, so ignore all of this
                        if (!(ast == AudioSpacialType.SharedSpace))
                        {
                            bool isVisible;
                            bool isSevered = false;

                            if (PosVisibleThroughOutputCollider())
                            {
                                isVisible = true;
                            }
                            else
                            {
                                isVisible = false;
                            }

                            // OneWayWindow can "sever" AudioPlayers if the plane is facing away from the viewer where the position it's playing from might not be clear
                            if (ast == AudioSpacialType.OneWayWindow)
                            {
                                // This one should only be checked as a OneWayWindow
                                if(Vector3.Dot(WWHelpers.playerCamera.transform.position - roAPData.outputSource.OutputPosition, roAPData.outputSource.planeNormalVector) < 0)
                                {
                                    isSevered = true;
                                }

                                // Other case would include if sound is playing on a window from the other side for both Window and OneWayWindow- implement this
                            }

                            if(isSevered)
                            {
                                //audioPlayer.AudioSource.maxDistance = audioPlayer.data.maxDistance - (returnInfo.position - roAPData.outputSource.OutputPosition).magnitude;
                                //returnInfo.position = roAPData.outputSource.OutputPosition;
                                audioPlayer.transform.position = roAPData.outputSource.OutputPosition;
                                MuffleCheckAP(audioPlayer);
                                float totalDistance = (roAPData.outputSource.OutputPosition - returnInfo.position).magnitude + (WWHelpers.playerCamera.transform.position - roAPData.outputSource.OutputPosition).magnitude;
                                returnInfo.position = ((roAPData.outputSource.OutputPosition - WWHelpers.playerCamera.transform.position).normalized * totalDistance) + WWHelpers.playerCamera.transform.position;
                            }
                            else if(!isVisible)
                            {
                                Vector3 originalPosition = returnInfo.position;

                                Instance.transform.rotation = roAPData.outputSource.planeRotation;
                                testingPlaneCollider.size = roAPData.outputSource.planeSize;
                                testingPlaneCollider.center = Instance.transform.TransformPoint(roAPData.outputSource.OutputPosition);

                                Ray ray = new(originalPosition, WWHelpers.playerCamera.transform.position - originalPosition);
                                roAPData.outputSource.plane.Raycast(new Ray(originalPosition, WWHelpers.playerCamera.transform.position - originalPosition), out float enter);
                                returnInfo.position = Instance.testingPlaneCollider.ClosestPoint(roAPData.outputSource.plane.ClosestPointOnPlane(ray.GetPoint(enter)));

                                // Muffle check at this point
                                audioPlayer.transform.position = returnInfo.position;
                                MuffleCheckAP(audioPlayer);

                                // Get adjusted audioPlayer position
                                float totalDistance = (returnInfo.position - originalPosition).magnitude + (WWHelpers.playerCamera.transform.position - returnInfo.position).magnitude;
                                returnInfo.position = ((returnInfo.position - WWHelpers.playerCamera.transform.position).normalized * totalDistance) + WWHelpers.playerCamera.transform.position;

                                // Clean up
                                Instance.transform.rotation = Quaternion.identity;
                                Instance.testingPlaneCollider.size = Vector3.zero;
                                Instance.testingPlaneCollider.center = Vector3.zero;
                                //audioPlayer.AudioSource.maxDistance = audioPlayer.data.maxDistance;
                            }
                        }

                        // Check if sound is muffled as normal here, or just check it in the update function with all the other APs
                    }
                    break;

                // If colldierShape is a point, it doesn't matter what the output is, this is broadcasting to a point
                case ColliderShape.Point:
                    returnInfo.position = roAPData.outputSource.outputTransform.position;
                    break;
            }

            return returnInfo;

            // Checks if a linecast drawn between pos and the main camera passes through outputSource's collider.
            bool PosVisibleThroughOutputCollider()
            {
                int outputLayer = roAPData.outputSource.outputTransform.gameObject.layer;
                roAPData.outputSource.outputTransform.gameObject.layer = 7;
                bool returnValue = Physics.Linecast(WWHelpers.playerCamera.transform.position, returnInfo.position, 1 << 7);
                roAPData.outputSource.outputTransform.gameObject.layer = outputLayer;
                return returnValue;
            }
        }

        /// <summary>
        /// Updates all Receiver-Output AudioPlayers to be lazy or active based on their range to the camera.
        /// <para> Rebuilds activeROAudioPlayers. </para>
        /// </summary>
        private static void RecalibrateLazyROAPs()
        {
            // Rebuild activeROAudioPlayers
            Instance.activeROAPs = new();
            foreach (AudioPlayer player in Instance.activeAPs)
            {
                // If not a receiverOutput, skip it
                if(player.data.roAPData == null)
                {
                    continue;
                }
                // If audioPlayer is in range, add to activeROAudioPlayers;
                if(AudioPlayerInRange(player))
                {
                    Instance.activeROAPs.Add(player);
                }
                // Otherwise, reset its position to zero
                else
                {
                    player.transform.localPosition = Vector3.zero;
                }
            }
        }

        #endregion Audio Position Management

        // ============================================================
        // Helpers
        // ============================================================

        #region Helpers

        /// <summary>
        /// Returns how long it would take to travel between two points at the speed of sound in seconds.
        /// </summary>
        private static float DelayBetweenPoints(Vector3 a, Vector3 b)
        {
            return (a - b).magnitude / Instance.speedOfSound;
        }

        /// <summary>
        /// Checks if a Receiver-Output AudioPlayer is in activeRORange of the main camera.
        /// </summary>
        private static bool AudioPlayerInRange(AudioPlayer audioPlayer)
        {
            return (audioPlayer.ParentTransform.position - WWHelpers.playerCamera.transform.position).sqrMagnitude < Instance.squaredReceiverOutputRangeThreshhold;
        }

        #endregion Helpers

        // ============================================================
        // Object Pool Functions
        // ============================================================

        #region Object Pool

        private AudioPlayer OnCreatePooledItem()
        {
            return Instantiate(audioPlayerPrefab).GetComponentInChildren<AudioPlayer>();
        }

        private void OnAccessFromPool(AudioPlayer audioPlayer)
        {
            audioPlayer.ParentTransform.gameObject.SetActive(true);
            Instance.activeAPs.Add(audioPlayer);
        }

        private void OnReleasedToPool(AudioPlayer audioPlayer)
        {
            Instance.activeAPs.Remove(audioPlayer);

            audioPlayer.ParentTransform.parent = transform;
            audioPlayer.ParentTransform.gameObject.SetActive(false);
            audioPlayer.ParentTransform.gameObject.name = "Released AudioSource";
            audioPlayer.gameObject.name = "Released AudioPlayer";
        }

        private void OnDestroyPooledObject(AudioPlayer audioPlayer)
        {
            Destroy(audioPlayer.ParentTransform.gameObject);
        }

        #endregion Object Pool

        // ============================================================
        // Script-Specific Structs
        // ============================================================

        #region SSS

        private struct AudioInformation
        {
            public Vector3 position;
            public float muffledness;

            public AudioInformation(Vector3 position, float muffledness)
            {
                this.position = position;
                this.muffledness = muffledness;
            }
        }

        #endregion SSS
    }
}