//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.3.0
//     from Assets/Inputs/PlayerInputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerInputActions : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputActions"",
    ""maps"": [
        {
            ""name"": ""PlayerMovement"",
            ""id"": ""15b6b348-38c5-4a34-b9c7-c904d8431edd"",
            ""actions"": [
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""69558c49-0428-45c3-b55f-0e0f26631f80"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""61e2baaf-453f-431d-af1f-cd073e628bb2"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""99e7b565-f524-446d-9ed6-ef52d5ff19a8"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Run"",
                    ""type"": ""Button"",
                    ""id"": ""53cde820-605c-4190-a72f-e74fc630e969"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""ad8a9f0b-374c-4412-92aa-023c32dc2470"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""becf4d6c-70f6-48bf-b01c-c2b0f2ae7bde"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""f0c8cf93-74a3-43db-8546-0a0a0a0bc36a"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""864dec57-21c3-4007-98a7-94a6594c124c"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""0706e254-6b0e-4572-8ec9-53c96167050c"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""c050d2f9-532c-419f-9c3b-3d9b90d04349"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""63d00c11-e02b-4d5d-88b1-6459b8f309f4"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1a46d6cc-6112-4734-ab34-6fd1dda22703"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""PlayerLook"",
            ""id"": ""60c5d926-6fee-435a-9cac-6a721f0e8b02"",
            ""actions"": [
                {
                    ""name"": ""New action"",
                    ""type"": ""Button"",
                    ""id"": ""93aae8eb-c4d5-4c39-bac7-a3aaa9f65a0c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""1312ed55-538f-49e7-a948-cf6f58230f6b"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""AltLook"",
                    ""type"": ""Button"",
                    ""id"": ""944899fe-7eb7-4761-a9dc-54eae17d43ab"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""838dd959-fddd-463c-9346-c0d95e9ef280"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""New action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c8c0b3d2-b701-4895-b1db-d975954c20a3"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d753d015-c17d-4a0c-a9cc-b24daf95c47a"",
                    ""path"": ""<Keyboard>/leftAlt"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""AltLook"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""PlayerInteract"",
            ""id"": ""62bd3bd9-8654-451a-8dcd-bc9a878c784a"",
            ""actions"": [
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""9c4fbd17-764e-40a8-894a-844d7a53d25e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""2e88d07b-9af1-47ac-a749-ec57f6021a8b"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""PlayerUse"",
            ""id"": ""14ecf25e-c31a-4c43-a88a-6926c6190f34"",
            ""actions"": [
                {
                    ""name"": ""Use"",
                    ""type"": ""Button"",
                    ""id"": ""e450ddcd-6cd4-4b2d-aad6-74ee7faeab0a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""AltUse"",
                    ""type"": ""Button"",
                    ""id"": ""34de94e6-12e3-4bd0-bf73-86f0c2b04f01"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""995450a9-f7f4-4cc6-963a-d99d54479056"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Use"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""900ffd59-2f50-4a71-bfb8-494130bfa521"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AltUse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""PlayerChangeMovementState"",
            ""id"": ""7f68826d-14c3-42fe-9e10-672c21543cb5"",
            ""actions"": [
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""37e8d373-77f3-4065-9298-980c9ebc1551"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Prone"",
                    ""type"": ""Button"",
                    ""id"": ""f6f6a0b0-5c72-4427-ac03-4d590003c2c2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""899a69d0-015f-4f48-9669-a62ee379df2e"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0332752f-4b0a-4532-b701-3f784734cfaa"",
                    ""path"": ""<Keyboard>/z"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Prone"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""PlayerEmote"",
            ""id"": ""abf1b466-0021-41f9-8eda-1d06e0981368"",
            ""actions"": [
                {
                    ""name"": ""Alert"",
                    ""type"": ""Button"",
                    ""id"": ""3184b192-5e92-4695-aa74-4ae8c1778bc3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Bruh"",
                    ""type"": ""Button"",
                    ""id"": ""f0f1504f-5146-4705-ab12-d18b31d07655"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""dd67d0ab-a726-4eba-9ad5-71f3eef9fc1f"",
                    ""path"": ""<Keyboard>/h"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Alert"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fd1a8be3-af5d-4fbc-ab5f-919265d42297"",
                    ""path"": ""<Keyboard>/b"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Bruh"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard and Mouse"",
            ""bindingGroup"": ""Keyboard and Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // PlayerMovement
        m_PlayerMovement = asset.FindActionMap("PlayerMovement", throwIfNotFound: true);
        m_PlayerMovement_Jump = m_PlayerMovement.FindAction("Jump", throwIfNotFound: true);
        m_PlayerMovement_Move = m_PlayerMovement.FindAction("Move", throwIfNotFound: true);
        m_PlayerMovement_Crouch = m_PlayerMovement.FindAction("Crouch", throwIfNotFound: true);
        m_PlayerMovement_Run = m_PlayerMovement.FindAction("Run", throwIfNotFound: true);
        // PlayerLook
        m_PlayerLook = asset.FindActionMap("PlayerLook", throwIfNotFound: true);
        m_PlayerLook_Newaction = m_PlayerLook.FindAction("New action", throwIfNotFound: true);
        m_PlayerLook_Look = m_PlayerLook.FindAction("Look", throwIfNotFound: true);
        m_PlayerLook_AltLook = m_PlayerLook.FindAction("AltLook", throwIfNotFound: true);
        // PlayerInteract
        m_PlayerInteract = asset.FindActionMap("PlayerInteract", throwIfNotFound: true);
        m_PlayerInteract_Interact = m_PlayerInteract.FindAction("Interact", throwIfNotFound: true);
        // PlayerUse
        m_PlayerUse = asset.FindActionMap("PlayerUse", throwIfNotFound: true);
        m_PlayerUse_Use = m_PlayerUse.FindAction("Use", throwIfNotFound: true);
        m_PlayerUse_AltUse = m_PlayerUse.FindAction("AltUse", throwIfNotFound: true);
        // PlayerChangeMovementState
        m_PlayerChangeMovementState = asset.FindActionMap("PlayerChangeMovementState", throwIfNotFound: true);
        m_PlayerChangeMovementState_Crouch = m_PlayerChangeMovementState.FindAction("Crouch", throwIfNotFound: true);
        m_PlayerChangeMovementState_Prone = m_PlayerChangeMovementState.FindAction("Prone", throwIfNotFound: true);
        // PlayerEmote
        m_PlayerEmote = asset.FindActionMap("PlayerEmote", throwIfNotFound: true);
        m_PlayerEmote_Alert = m_PlayerEmote.FindAction("Alert", throwIfNotFound: true);
        m_PlayerEmote_Bruh = m_PlayerEmote.FindAction("Bruh", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // PlayerMovement
    private readonly InputActionMap m_PlayerMovement;
    private IPlayerMovementActions m_PlayerMovementActionsCallbackInterface;
    private readonly InputAction m_PlayerMovement_Jump;
    private readonly InputAction m_PlayerMovement_Move;
    private readonly InputAction m_PlayerMovement_Crouch;
    private readonly InputAction m_PlayerMovement_Run;
    public struct PlayerMovementActions
    {
        private @PlayerInputActions m_Wrapper;
        public PlayerMovementActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Jump => m_Wrapper.m_PlayerMovement_Jump;
        public InputAction @Move => m_Wrapper.m_PlayerMovement_Move;
        public InputAction @Crouch => m_Wrapper.m_PlayerMovement_Crouch;
        public InputAction @Run => m_Wrapper.m_PlayerMovement_Run;
        public InputActionMap Get() { return m_Wrapper.m_PlayerMovement; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerMovementActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerMovementActions instance)
        {
            if (m_Wrapper.m_PlayerMovementActionsCallbackInterface != null)
            {
                @Jump.started -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnJump;
                @Move.started -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnMove;
                @Crouch.started -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnCrouch;
                @Crouch.performed -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnCrouch;
                @Crouch.canceled -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnCrouch;
                @Run.started -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnRun;
                @Run.performed -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnRun;
                @Run.canceled -= m_Wrapper.m_PlayerMovementActionsCallbackInterface.OnRun;
            }
            m_Wrapper.m_PlayerMovementActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Crouch.started += instance.OnCrouch;
                @Crouch.performed += instance.OnCrouch;
                @Crouch.canceled += instance.OnCrouch;
                @Run.started += instance.OnRun;
                @Run.performed += instance.OnRun;
                @Run.canceled += instance.OnRun;
            }
        }
    }
    public PlayerMovementActions @PlayerMovement => new PlayerMovementActions(this);

    // PlayerLook
    private readonly InputActionMap m_PlayerLook;
    private IPlayerLookActions m_PlayerLookActionsCallbackInterface;
    private readonly InputAction m_PlayerLook_Newaction;
    private readonly InputAction m_PlayerLook_Look;
    private readonly InputAction m_PlayerLook_AltLook;
    public struct PlayerLookActions
    {
        private @PlayerInputActions m_Wrapper;
        public PlayerLookActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Newaction => m_Wrapper.m_PlayerLook_Newaction;
        public InputAction @Look => m_Wrapper.m_PlayerLook_Look;
        public InputAction @AltLook => m_Wrapper.m_PlayerLook_AltLook;
        public InputActionMap Get() { return m_Wrapper.m_PlayerLook; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerLookActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerLookActions instance)
        {
            if (m_Wrapper.m_PlayerLookActionsCallbackInterface != null)
            {
                @Newaction.started -= m_Wrapper.m_PlayerLookActionsCallbackInterface.OnNewaction;
                @Newaction.performed -= m_Wrapper.m_PlayerLookActionsCallbackInterface.OnNewaction;
                @Newaction.canceled -= m_Wrapper.m_PlayerLookActionsCallbackInterface.OnNewaction;
                @Look.started -= m_Wrapper.m_PlayerLookActionsCallbackInterface.OnLook;
                @Look.performed -= m_Wrapper.m_PlayerLookActionsCallbackInterface.OnLook;
                @Look.canceled -= m_Wrapper.m_PlayerLookActionsCallbackInterface.OnLook;
                @AltLook.started -= m_Wrapper.m_PlayerLookActionsCallbackInterface.OnAltLook;
                @AltLook.performed -= m_Wrapper.m_PlayerLookActionsCallbackInterface.OnAltLook;
                @AltLook.canceled -= m_Wrapper.m_PlayerLookActionsCallbackInterface.OnAltLook;
            }
            m_Wrapper.m_PlayerLookActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Newaction.started += instance.OnNewaction;
                @Newaction.performed += instance.OnNewaction;
                @Newaction.canceled += instance.OnNewaction;
                @Look.started += instance.OnLook;
                @Look.performed += instance.OnLook;
                @Look.canceled += instance.OnLook;
                @AltLook.started += instance.OnAltLook;
                @AltLook.performed += instance.OnAltLook;
                @AltLook.canceled += instance.OnAltLook;
            }
        }
    }
    public PlayerLookActions @PlayerLook => new PlayerLookActions(this);

    // PlayerInteract
    private readonly InputActionMap m_PlayerInteract;
    private IPlayerInteractActions m_PlayerInteractActionsCallbackInterface;
    private readonly InputAction m_PlayerInteract_Interact;
    public struct PlayerInteractActions
    {
        private @PlayerInputActions m_Wrapper;
        public PlayerInteractActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Interact => m_Wrapper.m_PlayerInteract_Interact;
        public InputActionMap Get() { return m_Wrapper.m_PlayerInteract; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerInteractActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerInteractActions instance)
        {
            if (m_Wrapper.m_PlayerInteractActionsCallbackInterface != null)
            {
                @Interact.started -= m_Wrapper.m_PlayerInteractActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_PlayerInteractActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_PlayerInteractActionsCallbackInterface.OnInteract;
            }
            m_Wrapper.m_PlayerInteractActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
            }
        }
    }
    public PlayerInteractActions @PlayerInteract => new PlayerInteractActions(this);

    // PlayerUse
    private readonly InputActionMap m_PlayerUse;
    private IPlayerUseActions m_PlayerUseActionsCallbackInterface;
    private readonly InputAction m_PlayerUse_Use;
    private readonly InputAction m_PlayerUse_AltUse;
    public struct PlayerUseActions
    {
        private @PlayerInputActions m_Wrapper;
        public PlayerUseActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Use => m_Wrapper.m_PlayerUse_Use;
        public InputAction @AltUse => m_Wrapper.m_PlayerUse_AltUse;
        public InputActionMap Get() { return m_Wrapper.m_PlayerUse; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerUseActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerUseActions instance)
        {
            if (m_Wrapper.m_PlayerUseActionsCallbackInterface != null)
            {
                @Use.started -= m_Wrapper.m_PlayerUseActionsCallbackInterface.OnUse;
                @Use.performed -= m_Wrapper.m_PlayerUseActionsCallbackInterface.OnUse;
                @Use.canceled -= m_Wrapper.m_PlayerUseActionsCallbackInterface.OnUse;
                @AltUse.started -= m_Wrapper.m_PlayerUseActionsCallbackInterface.OnAltUse;
                @AltUse.performed -= m_Wrapper.m_PlayerUseActionsCallbackInterface.OnAltUse;
                @AltUse.canceled -= m_Wrapper.m_PlayerUseActionsCallbackInterface.OnAltUse;
            }
            m_Wrapper.m_PlayerUseActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Use.started += instance.OnUse;
                @Use.performed += instance.OnUse;
                @Use.canceled += instance.OnUse;
                @AltUse.started += instance.OnAltUse;
                @AltUse.performed += instance.OnAltUse;
                @AltUse.canceled += instance.OnAltUse;
            }
        }
    }
    public PlayerUseActions @PlayerUse => new PlayerUseActions(this);

    // PlayerChangeMovementState
    private readonly InputActionMap m_PlayerChangeMovementState;
    private IPlayerChangeMovementStateActions m_PlayerChangeMovementStateActionsCallbackInterface;
    private readonly InputAction m_PlayerChangeMovementState_Crouch;
    private readonly InputAction m_PlayerChangeMovementState_Prone;
    public struct PlayerChangeMovementStateActions
    {
        private @PlayerInputActions m_Wrapper;
        public PlayerChangeMovementStateActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Crouch => m_Wrapper.m_PlayerChangeMovementState_Crouch;
        public InputAction @Prone => m_Wrapper.m_PlayerChangeMovementState_Prone;
        public InputActionMap Get() { return m_Wrapper.m_PlayerChangeMovementState; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerChangeMovementStateActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerChangeMovementStateActions instance)
        {
            if (m_Wrapper.m_PlayerChangeMovementStateActionsCallbackInterface != null)
            {
                @Crouch.started -= m_Wrapper.m_PlayerChangeMovementStateActionsCallbackInterface.OnCrouch;
                @Crouch.performed -= m_Wrapper.m_PlayerChangeMovementStateActionsCallbackInterface.OnCrouch;
                @Crouch.canceled -= m_Wrapper.m_PlayerChangeMovementStateActionsCallbackInterface.OnCrouch;
                @Prone.started -= m_Wrapper.m_PlayerChangeMovementStateActionsCallbackInterface.OnProne;
                @Prone.performed -= m_Wrapper.m_PlayerChangeMovementStateActionsCallbackInterface.OnProne;
                @Prone.canceled -= m_Wrapper.m_PlayerChangeMovementStateActionsCallbackInterface.OnProne;
            }
            m_Wrapper.m_PlayerChangeMovementStateActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Crouch.started += instance.OnCrouch;
                @Crouch.performed += instance.OnCrouch;
                @Crouch.canceled += instance.OnCrouch;
                @Prone.started += instance.OnProne;
                @Prone.performed += instance.OnProne;
                @Prone.canceled += instance.OnProne;
            }
        }
    }
    public PlayerChangeMovementStateActions @PlayerChangeMovementState => new PlayerChangeMovementStateActions(this);

    // PlayerEmote
    private readonly InputActionMap m_PlayerEmote;
    private IPlayerEmoteActions m_PlayerEmoteActionsCallbackInterface;
    private readonly InputAction m_PlayerEmote_Alert;
    private readonly InputAction m_PlayerEmote_Bruh;
    public struct PlayerEmoteActions
    {
        private @PlayerInputActions m_Wrapper;
        public PlayerEmoteActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Alert => m_Wrapper.m_PlayerEmote_Alert;
        public InputAction @Bruh => m_Wrapper.m_PlayerEmote_Bruh;
        public InputActionMap Get() { return m_Wrapper.m_PlayerEmote; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerEmoteActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerEmoteActions instance)
        {
            if (m_Wrapper.m_PlayerEmoteActionsCallbackInterface != null)
            {
                @Alert.started -= m_Wrapper.m_PlayerEmoteActionsCallbackInterface.OnAlert;
                @Alert.performed -= m_Wrapper.m_PlayerEmoteActionsCallbackInterface.OnAlert;
                @Alert.canceled -= m_Wrapper.m_PlayerEmoteActionsCallbackInterface.OnAlert;
                @Bruh.started -= m_Wrapper.m_PlayerEmoteActionsCallbackInterface.OnBruh;
                @Bruh.performed -= m_Wrapper.m_PlayerEmoteActionsCallbackInterface.OnBruh;
                @Bruh.canceled -= m_Wrapper.m_PlayerEmoteActionsCallbackInterface.OnBruh;
            }
            m_Wrapper.m_PlayerEmoteActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Alert.started += instance.OnAlert;
                @Alert.performed += instance.OnAlert;
                @Alert.canceled += instance.OnAlert;
                @Bruh.started += instance.OnBruh;
                @Bruh.performed += instance.OnBruh;
                @Bruh.canceled += instance.OnBruh;
            }
        }
    }
    public PlayerEmoteActions @PlayerEmote => new PlayerEmoteActions(this);
    private int m_KeyboardandMouseSchemeIndex = -1;
    public InputControlScheme KeyboardandMouseScheme
    {
        get
        {
            if (m_KeyboardandMouseSchemeIndex == -1) m_KeyboardandMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard and Mouse");
            return asset.controlSchemes[m_KeyboardandMouseSchemeIndex];
        }
    }
    public interface IPlayerMovementActions
    {
        void OnJump(InputAction.CallbackContext context);
        void OnMove(InputAction.CallbackContext context);
        void OnCrouch(InputAction.CallbackContext context);
        void OnRun(InputAction.CallbackContext context);
    }
    public interface IPlayerLookActions
    {
        void OnNewaction(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnAltLook(InputAction.CallbackContext context);
    }
    public interface IPlayerInteractActions
    {
        void OnInteract(InputAction.CallbackContext context);
    }
    public interface IPlayerUseActions
    {
        void OnUse(InputAction.CallbackContext context);
        void OnAltUse(InputAction.CallbackContext context);
    }
    public interface IPlayerChangeMovementStateActions
    {
        void OnCrouch(InputAction.CallbackContext context);
        void OnProne(InputAction.CallbackContext context);
    }
    public interface IPlayerEmoteActions
    {
        void OnAlert(InputAction.CallbackContext context);
        void OnBruh(InputAction.CallbackContext context);
    }
}
