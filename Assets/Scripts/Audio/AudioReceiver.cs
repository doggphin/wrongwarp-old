using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace WrongWarp
{
    /// <summary>
    /// When the AudioManager tries to play a sound and an AudioReceiver is in range of it, the AudioManager will play the sound again on all of the AudioReceiver's OutputSources.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class AudioReceiver : MonoBehaviour
    {
        private void Awake()
        {
            audioCollider = GetComponent<Collider>();

            for (int i = 0; i < outputSources.Length; i++)
            {
                outputSources[i].planeNormalVector = (outputSources[i].planeRotation * Vector3.forward).normalized;
                outputSources[i].plane = new Plane(outputSources[i].planeNormalVector, outputSources[i].outputTransform.position + outputSources[i].offset);
            }
        }

        /// <summary>
        /// The collider this receiver receives sounds from.
        /// </summary>
        public Collider AudioCollider => audioCollider;
        private Collider audioCollider;

        /// <summary>
        /// The shape this receiver should be interpereted as.
        /// </summary>
        public ColliderShape ColliderShape { get { return colliderShape; } }
        [Tooltip("What shape is this receiver's collider?")]
        [SerializeField] private ColliderShape colliderShape;

        /// <summary>
        /// The sources this receiver relays sounds to.
        /// </summary>
        public OutputSource[] OutputSources => outputSources;
        [Tooltip("The transforms and data of places this receiver should broadcast sound to.")]
        [SerializeField] private OutputSource[] outputSources;
    }

    /// <summary>
    /// Represents an AudioReceiver's output transform, SpacialType and an offset OR rotation from its transform.
    /// </summary>
    [Serializable]
    public struct OutputSource
    {
        /// <summary>
        /// The transform AudioPlayers should be routed to.
        /// </summary>
        public Transform outputTransform;
        public AudioSpacialType spacialType;
        public Vector3 offset;

        [AllowNesting]
        [HideIf("spacialType", AudioSpacialType.Point)]
        public Quaternion planeRotation;

        [AllowNesting]
        [HideIf("spacialType", AudioSpacialType.Point)]
        public Vector2 planeSize;

        [AllowNesting]
        [HideIf("spacialType", AudioSpacialType.Point)]
        [EnableIf("spacialType", AudioSpacialType.Point)]
        public Vector3 planeNormalVector;

        [HideInInspector] public Plane plane;

        /// <summary>
        /// The position, including offset, of the output's center.
        /// </summary>
        public Vector3 OutputPosition => outputTransform.position + offset;
    }

    /// <summary>
    /// Represents the shape of a collider.
    /// </summary>
    public enum ColliderShape
    {
        Point,
        Plane
    }

    /// <summary>
    /// Represents different kinds of ways audio should be output.
    /// </summary>
    public enum AudioSpacialType
    {
        /// <summary>
        /// Point is used to just play the sound at at a supplied offset in the OutputSource struct.
        /// </summary>
        Point,
        /// <summary>
        /// SharedSpace is used in situations to maintain an illusion of a continuous space. 
        /// </summary>
        SharedSpace,
        /// <summary>
        /// If an audio source isn't visible through a plane, Window will move it to be on its border and make the sound 3D.
        /// </summary>
        Window,
        /// <summary>
        /// <para> The portal AudioSpacialType. </para>
        /// If an audio source isn't visible through a plane, OneWayWindow will move it to be on its border and make the sound 3D.
        /// <para> OneWayWindow differs from Window in that it makes all sounds 3D if the viewer is behind the window. </para>
        /// </summary>
        OneWayWindow
    }
}
