using UnityEngine;
using Mirror;

namespace WrongWarp
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class PlayerMovement : NetworkBehaviour
    {
        #region Serialized Fields

        public Camera PlayerCamera => playerCamera;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private CapsuleCollider cc;
        [SerializeField] Transform visualTransform;

        [SerializeField] private float mouseSensitivity = 0.2f;
        [SerializeField] private float joystickAcceleration = 100;
        [SerializeField] private float joystickGravity = 100;
        [Space]
        [SerializeField] private float standingSpeedCap = 4;
        [SerializeField] private float crouchingSpeedCap = 2;
        [SerializeField] private float proneSpeedCap = 1;
        [SerializeField] private float groundPlayerAcceleration = 50;
        [SerializeField] private float airPlayerAcceleration = 7;
        [SerializeField][Range(0, 1)] private float airFriction = 0.8f;
        [SerializeField][Range(0, 1)] private float groundFriction = 0.0000001f;
        [SerializeField] private float zeroSpeedThreshhold = 0.05f;
        [Space]
        [SerializeField] private float jumpStrength = 5;
        [SerializeField] private float maxSlopeAngle = 45f;
        [SerializeField] private float gravityStrength = 15;
        [SerializeField] private float snapDistance = 0.05f;
        [Space]
        [SerializeField] private float standingHeight = 2f;
        [SerializeField] private float crouchingHeight = 1.5f;
        [SerializeField] private float proneHeight = 1f;
        [SerializeField] private float raycastSafetyDist = 0.2f;
        [SerializeField] private int[] collisionLayers;

        #endregion Serialized Fields

        #region Initializers

        private void OnValidate()
        {
            mouseSensitivity = SetAboveZero(mouseSensitivity);
            joystickGravity = SetAboveZero(joystickGravity);
            joystickAcceleration = SetAboveZero(joystickAcceleration);
            groundPlayerAcceleration = SetAboveZero(groundPlayerAcceleration);
            snapDistance = SetAboveZero(snapDistance);
            standingSpeedCap = SetAboveZero(standingSpeedCap);
            crouchingSpeedCap = SetAboveZero(crouchingSpeedCap);
            proneSpeedCap = SetAboveZero(proneSpeedCap);
            raycastSafetyDist = SetAboveZero(raycastSafetyDist);
            zeroSpeedThreshhold = SetAboveZero(zeroSpeedThreshhold);

            airFriction = Mathf.Clamp(airFriction, 0, 1);
            groundFriction = Mathf.Clamp(groundFriction, 0, 1);

            float SetAboveZero(float value)
            {
                return Mathf.Max(value, 0);
            }
        }

        public void InitAsClient()
        {
            InputManager.PlayerControls.PlayerMovement.Move.performed += ctx => wasdInput = ctx.ReadValue<Vector2>();
            InputManager.PlayerControls.PlayerMovement.Move.canceled += ctx => wasdInput = Vector3.zero;
            InputManager.PlayerControls.PlayerMovement.Jump.started += ctx => jumpInput = true;
            InputManager.PlayerControls.PlayerLook.Look.performed += ctx => cumulativeLook += ctx.ReadValue<Vector2>();
            InputManager.PlayerControls.PlayerChangeMovementState.Crouch.started += ctx => crouchInput = true;
            InputManager.PlayerControls.PlayerChangeMovementState.Prone.started += ctx => proneInput = true;

            TickManager.PollInput += SetInputs;
            TickManager.pollInputs = true;
        }

        private void Awake()
        {
            collisionsLayerMask = WWHelpers.CreateLayerMaskFromInts(collisionLayers);
        }

        private void Start()
        {
            if (!isLocalPlayer)
            {
                WWHelpers.SetChildrenLayers(transform, LayerMask.NameToLayer("Default"), true);
            }
        }

        #endregion Initializers

        #region Helpers

        private Vector3 GravityVector => gravityStrength * -playerVerticalAxis;
        private Vector3 VerticalVelocity => Vector3.Project(velocity, playerVerticalAxis);
        private float VerticalSpeedSigned => (Quaternion.Inverse(transform.rotation) * VerticalVelocity).y;
        private Vector3 TopDownVelocity {
            get
            {
                return Quaternion.Inverse(transform.rotation) * (velocity - VerticalVelocity);
            }
            set
            {
                velocity = new Vector3(TopDownVelocity.x, VerticalSpeedSigned, TopDownVelocity.z);
            }
        }
        private float PlayerHeight => Mathf.Max(cc.radius * 2, cc.height);
        private Vector3 ColliderBot => transform.position;
        private Vector3 ColliderBotHemiCenter => ColliderBot + cc.radius * playerVerticalAxis;
        private Vector3 ColliderTop => transform.position + playerVerticalAxis * PlayerHeight;
        private Vector3 ColliderTopHemiCenter => ColliderTop - cc.radius * playerVerticalAxis;
        private float Drag => isGrounded ? groundFriction : airFriction;
        private float GetMovementStateSpeedCap(PlayerMovementState movementState)
        {
            return movementState switch
            {
                PlayerMovementState.Standing => standingSpeedCap,
                PlayerMovementState.Crouching => crouchingSpeedCap,
                PlayerMovementState.Prone => proneSpeedCap,
                _ => 0
            };
        }
        private float GetMovementStateHeight(PlayerMovementState movementState)
        {
            return movementState switch
            {
                PlayerMovementState.Standing => standingHeight,
                PlayerMovementState.Crouching => crouchingHeight,
                PlayerMovementState.Prone => proneHeight,
                _ => 0
            };
        }
        private float PlayerAcceleration => isGrounded ? groundPlayerAcceleration : airPlayerAcceleration;

        #endregion Helpers

        private float cameraPitch;
        private Vector2 wasdInput;
        private Vector2 joystickVector;
        private Vector3 velocity;
        private Vector3 playerVerticalAxis = Vector3.up;
        private bool isGrounded = false;
        private PlayerMovementState movementState = PlayerMovementState.Standing;
        private bool jumpInput;
        private bool crouchInput;
        private bool proneInput;
        private LayerMask collisionsLayerMask;
        Vector3 oldPosition = Vector3.zero;
        Vector3 currentPosition = Vector3.zero;
        Vector2 cumulativeLook = Vector2.zero;
        private bool switchingMovementState = false;

        public void UpdatePosition(InputPacket packet)
        {
            WWHelpers.SetChildrenLayers(transform, LayerMask.NameToLayer("Self"), true);

            if (packet == null) { packet = new InputPacket(); }
            oldPosition = transform.position;

            transform.localEulerAngles = new Vector3(0, packet.lookVector.y, 0);
            playerCamera.transform.localEulerAngles = new Vector3(packet.lookVector.x, 0, 0);

            crouchInput = packet.ReadInputValue(InputPacket.InputButton.Crouch);
            proneInput = packet.ReadInputValue(InputPacket.InputButton.Prone);

            if (crouchInput)
            {
                if (movementState == PlayerMovementState.Crouching)
                {
                    SetMovementState(PlayerMovementState.Standing);
                }
                else
                {
                    SetMovementState(PlayerMovementState.Crouching);
                }
            }
            else
            {
                if (proneInput)
                {
                    if (movementState == PlayerMovementState.Prone)
                    {
                        SetMovementState(PlayerMovementState.Standing);
                    }
                    else
                    {
                        SetMovementState(PlayerMovementState.Prone);
                    }
                }
            }

            ApplyGravity(packet);
            if (ApplyVelocityChanges(packet))
            {
                MoveCollider();
            }
            CollisionCheck();

            currentPosition = transform.position;

            WWHelpers.SetChildrenLayers(transform, LayerMask.NameToLayer("Default"), true);
            jumpInput = false;
            crouchInput = false;
            proneInput = false;
        }

        private void Update()
        {
            if (isLocalPlayer)
            {
                visualTransform.position = Vector3.Lerp(oldPosition, currentPosition, (Time.time % Time.fixedDeltaTime) / Time.fixedDeltaTime);
            }
        }

        /// <summary>
        /// Injects inputs into a packet using the inputs cached in this component, then runs those inputs on the client.
        /// </summary>
        /// <param name="packet">The packet to inject inputs into.</param>
        private void SetInputs(InputPacket packet)
        {
            // Run own inputs on client
            cameraPitch -= cumulativeLook.y * mouseSensitivity;
            cameraPitch = Mathf.Clamp(cameraPitch, -90, 90f);
            playerCamera.transform.localEulerAngles = Vector3.right * cameraPitch;
            transform.Rotate(cumulativeLook.x * mouseSensitivity * Vector3.up);
            cumulativeLook = Vector2.zero;

            packet.lookVector = new Vector2(playerCamera.transform.localEulerAngles.x, transform.localEulerAngles.y);

            // Get player look and input values
            packet.SetInputValue(InputPacket.InputButton.Up, wasdInput.y > 0);
            packet.SetInputValue(InputPacket.InputButton.Left, wasdInput.x < 0);
            packet.SetInputValue(InputPacket.InputButton.Down, wasdInput.y < 0);
            packet.SetInputValue(InputPacket.InputButton.Right, wasdInput.x > 0);
            packet.SetInputValue(InputPacket.InputButton.Jump, jumpInput);
            packet.SetInputValue(InputPacket.InputButton.Crouch, crouchInput);
            packet.SetInputValue(InputPacket.InputButton.Prone, proneInput);
        }

        private void ApplyGravity(InputPacket packet)
        {
            isGrounded = false;

            bool applyGravity = true;
            float radiusReduction = 0f;
            if (Vector3.Dot(VerticalVelocity, playerVerticalAxis) <= 0)
            {
                while (radiusReduction < cc.radius)
                {
                    if (!SphereCastDown(radiusReduction, out RaycastHit sphereHit))
                    {
                        break;
                    }
                    if (Vector3.Angle(-playerVerticalAxis, sphereHit.point - ColliderBotHemiCenter) > maxSlopeAngle)
                    {
                        radiusReduction += 0.1f;
                        continue;
                    }
                    applyGravity = false;
                    isGrounded = true;
                    if (packet.ReadInputValue(InputPacket.InputButton.Jump))
                    {
                        velocity += playerVerticalAxis * jumpStrength;
                        break;
                    }
                    if (Physics.Raycast(new Ray(ColliderBot + playerVerticalAxis * raycastSafetyDist, -playerVerticalAxis), out RaycastHit rayHit, snapDistance + raycastSafetyDist, collisionsLayerMask))
                    {
                        if ((rayHit.point - ColliderBotHemiCenter).magnitude <= (sphereHit.point - ColliderBotHemiCenter).magnitude)
                        {
                            transform.position += Vector3.Project(sphereHit.point - ColliderBotHemiCenter, -playerVerticalAxis);
                            velocity -= Vector3.Project(velocity, playerVerticalAxis);
                        }
                    }
                    break;
                }
            }
            if (applyGravity) { velocity += Time.deltaTime * GravityVector; }

            bool SphereCastDown(float radiusReductionAmount, out RaycastHit returnHit)
            {
                return Physics.SphereCast(new Ray(transform.position + (playerVerticalAxis * (cc.radius + raycastSafetyDist)), -playerVerticalAxis), cc.radius - radiusReductionAmount, out returnHit, snapDistance + raycastSafetyDist + radiusReductionAmount, collisionsLayerMask);
            }
        }

        private bool ApplyVelocityChanges(InputPacket packet)
        {
            Vector2 packetWASDInput = new Vector2(
                -WWBinaryFunctions.BoolToInt(packet.ReadInputValue(InputPacket.InputButton.Left)) + WWBinaryFunctions.BoolToInt(packet.ReadInputValue(InputPacket.InputButton.Right)),
                -WWBinaryFunctions.BoolToInt(packet.ReadInputValue(InputPacket.InputButton.Down)) + WWBinaryFunctions.BoolToInt(packet.ReadInputValue(InputPacket.InputButton.Up))
                );
            bool playerIsTryingToMove = !(packetWASDInput == Vector2.zero && joystickVector != Vector2.zero);
            if (!playerIsTryingToMove) 
            {
                if (joystickVector != Vector2.zero)
                {
                    joystickVector -= Mathf.Min(joystickVector.magnitude, joystickGravity * Time.deltaTime) * joystickVector.normalized;
                }
                if (velocity.magnitude < zeroSpeedThreshhold)
                {
                    velocity = Vector3.zero;
                    return false;
                }    
            }
            else
            {
                float _joystickMagnitude = (joystickVector += Time.deltaTime * joystickAcceleration * packetWASDInput).magnitude;
                if (_joystickMagnitude > 1)
                {
                    joystickVector /= _joystickMagnitude;
                }

                Vector3 joystickAddingVelocity = new(joystickVector.x, 0, joystickVector.y);
                Vector3 _topDownVelocity = TopDownVelocity;
                bool wasAboveSpeedLimit = _topDownVelocity.magnitude > GetMovementStateSpeedCap(movementState);
                if (wasAboveSpeedLimit)
                {
                    if (Vector3.Dot(_topDownVelocity, joystickAddingVelocity) > 0)
                    {
                        joystickAddingVelocity -= Vector3.Project(joystickAddingVelocity, _topDownVelocity);
                    }
                }
                joystickAddingVelocity = PlayerAcceleration * (joystickAddingVelocity.x * transform.right + joystickAddingVelocity.z * transform.forward);
                velocity += Time.deltaTime * (Quaternion.FromToRotation(Vector3.up, playerVerticalAxis) * joystickAddingVelocity);
                if (_topDownVelocity.magnitude > GetMovementStateSpeedCap(movementState) && !wasAboveSpeedLimit)
                {
                    velocity = (velocity - VerticalVelocity).normalized * GetMovementStateSpeedCap(movementState) + VerticalVelocity;
                }
            }
            return true;
        }

        private void MoveCollider()
        {
            if(velocity == Vector3.zero) { return; }
            // Source: https://forums.tigsource.com/index.php?topic=18807.0
            float positionCoefficient = (Mathf.Pow(Drag, Time.deltaTime * Time.deltaTime) - 1) / (Time.deltaTime * Mathf.Log(Drag));

            float _velocityMagnitude = velocity.magnitude;
            transform.position +=  
                Physics.CapsuleCast(ColliderBot, ColliderTop, cc.radius - 0.01f, velocity, out RaycastHit hit, _velocityMagnitude * positionCoefficient, collisionsLayerMask)
                ? (hit.distance + 0.01f) * (velocity / _velocityMagnitude)
                : velocity * positionCoefficient;

            // Source: https://forums.tigsource.com/index.php?topic=18807.0
            velocity *= Mathf.Pow(Drag, Time.deltaTime);
        }

        private void CollisionCheck()
        {
            // Source: https://www.youtube.com/watch?v=ona6VvuWLkI&ab_channel=AJTech
            Collider[] overlaps = Physics.OverlapCapsule(transform.position, transform.position + (playerVerticalAxis * PlayerHeight), cc.radius, collisionsLayerMask, QueryTriggerInteraction.Ignore);

            for(int i=0; i<overlaps.Length; i++)
            {
                Transform t = overlaps[i].transform;
                if (Physics.ComputePenetration(cc, transform.position, transform.rotation, overlaps[i], t.position, t.rotation, out Vector3 dir, out float dist))
                {
                    transform.position = transform.position + dir * dist;
                    velocity -= Vector3.Project(velocity, -dir);
                }
            }
        }

        private void SetMovementState(PlayerMovementState _movementState, bool checkBounds = true)
        {
            if (checkBounds)
            {
                if(!Physics.CheckCapsule(ColliderBotHemiCenter, ColliderBotHemiCenter + playerVerticalAxis * GetMovementStateHeight(_movementState), cc.radius - 0.01f, collisionsLayerMask))
                {
                    SetState();
                }
                else
                {
                    Debug.Log("Couldn't stand up because it's too short.");
                }
            }
            else
            {
                SetState();
            }

            void SetState()
            {
                movementState = _movementState;
                playerCamera.transform.position = ColliderTopHemiCenter;
            }
        }

        private enum PlayerMovementState
        {
            Standing = 0,
            Crouching = 1,
            Prone = 2
        }
    }
}
