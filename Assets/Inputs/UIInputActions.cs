//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.3.0
//     from Assets/Inputs/UIInputActions.inputactions
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

public partial class @UIInputActions : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @UIInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""UIInputActions"",
    ""maps"": [
        {
            ""name"": ""SlotSelect"",
            ""id"": ""bb544d20-4100-475e-87ef-992a459e63b7"",
            ""actions"": [
                {
                    ""name"": ""Slot1"",
                    ""type"": ""Button"",
                    ""id"": ""37224fcd-0084-40bc-a5ea-95db0b079133"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Slot2"",
                    ""type"": ""Button"",
                    ""id"": ""362b337d-b646-4e46-9a7c-644fe96ff001"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Slot3"",
                    ""type"": ""Button"",
                    ""id"": ""f0fefd12-7c7b-48e4-81f7-44c9a0c7dcd2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Slot4"",
                    ""type"": ""Button"",
                    ""id"": ""1178d0b9-23d6-4bf6-9f2b-2b0024d1f341"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Slot5"",
                    ""type"": ""Button"",
                    ""id"": ""560f3344-0e14-43a5-98c6-e0ea9b419c63"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Slot6"",
                    ""type"": ""Button"",
                    ""id"": ""e5c4e74e-0d8c-49d9-a610-00f0d071c48f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Slot7"",
                    ""type"": ""Button"",
                    ""id"": ""fe8e0798-2153-4c87-8cb6-a220f59937d5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""dafd6fad-2f25-4da9-ad8b-8da9b4117a8a"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Slot1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fe48d65e-6461-4932-bfaa-917c73f37dbc"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Slot2"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a93fa68d-a358-4ee1-bafa-0daa33600388"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Slot3"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2ccd5647-20ef-4472-aae2-7493977abc76"",
                    ""path"": ""<Keyboard>/4"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Slot4"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9affa837-abc2-422d-b4a5-5eae211531c7"",
                    ""path"": ""<Keyboard>/5"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Slot5"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8ed61492-f635-4326-a204-0bd2edd00860"",
                    ""path"": ""<Keyboard>/6"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Slot6"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""54406f2a-f20d-4272-a2e1-ac07f7cc91a8"",
                    ""path"": ""<Keyboard>/7"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Slot7"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""UIMouse"",
            ""id"": ""be34963a-1310-4f64-b326-80845ddee6fb"",
            ""actions"": [
                {
                    ""name"": ""LeftClick"",
                    ""type"": ""Button"",
                    ""id"": ""95b7fbd5-31e6-460f-8a55-6b220889653d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""RightClick"",
                    ""type"": ""Button"",
                    ""id"": ""b9917268-422f-44dd-8d86-f1f02c14ec67"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Scroll"",
                    ""type"": ""Value"",
                    ""id"": ""00cdc137-06e1-450d-a7c0-02dee5e16df3"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""adac1489-466e-42ed-abd2-139b70767337"",
                    ""path"": ""<Mouse>/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3966e994-9d6a-490c-8217-64a7f6aa8acb"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""43978bb0-840e-4f52-8cdc-61dd5e890333"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Scroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Toggles"",
            ""id"": ""6fc83306-ee37-4448-9d0d-4d0642371c45"",
            ""actions"": [
                {
                    ""name"": ""Escape"",
                    ""type"": ""Button"",
                    ""id"": ""8c56bfa8-9fab-4c41-93fd-0437d622ec47"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Inventory"",
                    ""type"": ""Button"",
                    ""id"": ""8de5f095-33fd-44f6-8edc-11c46a4abb7b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""b48813ee-20c1-4858-88c2-0f6f82e4bfe3"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Escape"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""13ee98a4-ecbb-404c-879b-a6455ce2c4c3"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Inventory"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // SlotSelect
        m_SlotSelect = asset.FindActionMap("SlotSelect", throwIfNotFound: true);
        m_SlotSelect_Slot1 = m_SlotSelect.FindAction("Slot1", throwIfNotFound: true);
        m_SlotSelect_Slot2 = m_SlotSelect.FindAction("Slot2", throwIfNotFound: true);
        m_SlotSelect_Slot3 = m_SlotSelect.FindAction("Slot3", throwIfNotFound: true);
        m_SlotSelect_Slot4 = m_SlotSelect.FindAction("Slot4", throwIfNotFound: true);
        m_SlotSelect_Slot5 = m_SlotSelect.FindAction("Slot5", throwIfNotFound: true);
        m_SlotSelect_Slot6 = m_SlotSelect.FindAction("Slot6", throwIfNotFound: true);
        m_SlotSelect_Slot7 = m_SlotSelect.FindAction("Slot7", throwIfNotFound: true);
        // UIMouse
        m_UIMouse = asset.FindActionMap("UIMouse", throwIfNotFound: true);
        m_UIMouse_LeftClick = m_UIMouse.FindAction("LeftClick", throwIfNotFound: true);
        m_UIMouse_RightClick = m_UIMouse.FindAction("RightClick", throwIfNotFound: true);
        m_UIMouse_Scroll = m_UIMouse.FindAction("Scroll", throwIfNotFound: true);
        // Toggles
        m_Toggles = asset.FindActionMap("Toggles", throwIfNotFound: true);
        m_Toggles_Escape = m_Toggles.FindAction("Escape", throwIfNotFound: true);
        m_Toggles_Inventory = m_Toggles.FindAction("Inventory", throwIfNotFound: true);
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

    // SlotSelect
    private readonly InputActionMap m_SlotSelect;
    private ISlotSelectActions m_SlotSelectActionsCallbackInterface;
    private readonly InputAction m_SlotSelect_Slot1;
    private readonly InputAction m_SlotSelect_Slot2;
    private readonly InputAction m_SlotSelect_Slot3;
    private readonly InputAction m_SlotSelect_Slot4;
    private readonly InputAction m_SlotSelect_Slot5;
    private readonly InputAction m_SlotSelect_Slot6;
    private readonly InputAction m_SlotSelect_Slot7;
    public struct SlotSelectActions
    {
        private @UIInputActions m_Wrapper;
        public SlotSelectActions(@UIInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Slot1 => m_Wrapper.m_SlotSelect_Slot1;
        public InputAction @Slot2 => m_Wrapper.m_SlotSelect_Slot2;
        public InputAction @Slot3 => m_Wrapper.m_SlotSelect_Slot3;
        public InputAction @Slot4 => m_Wrapper.m_SlotSelect_Slot4;
        public InputAction @Slot5 => m_Wrapper.m_SlotSelect_Slot5;
        public InputAction @Slot6 => m_Wrapper.m_SlotSelect_Slot6;
        public InputAction @Slot7 => m_Wrapper.m_SlotSelect_Slot7;
        public InputActionMap Get() { return m_Wrapper.m_SlotSelect; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(SlotSelectActions set) { return set.Get(); }
        public void SetCallbacks(ISlotSelectActions instance)
        {
            if (m_Wrapper.m_SlotSelectActionsCallbackInterface != null)
            {
                @Slot1.started -= m_Wrapper.m_SlotSelectActionsCallbackInterface.OnSlot1;
                @Slot1.performed -= m_Wrapper.m_SlotSelectActionsCallbackInterface.OnSlot1;
                @Slot1.canceled -= m_Wrapper.m_SlotSelectActionsCallbackInterface.OnSlot1;
                @Slot2.started -= m_Wrapper.m_SlotSelectActionsCallbackInterface.OnSlot2;
                @Slot2.performed -= m_Wrapper.m_SlotSelectActionsCallbackInterface.OnSlot2;
                @Slot2.canceled -= m_Wrapper.m_SlotSelectActionsCallbackInterface.OnSlot2;
                @Slot3.started -= m_Wrapper.m_SlotSelectActionsCallbackInterface.OnSlot3;
                @Slot3.performed -= m_Wrapper.m_SlotSelectActionsCallbackInterface.OnSlot3;
                @Slot3.canceled -= m_Wrapper.m_SlotSelectActionsCallbackInterface.OnSlot3;
                @Slot4.started -= m_Wrapper.m_SlotSelectActionsCallbackInterface.OnSlot4;
                @Slot4.performed -= m_Wrapper.m_SlotSelectActionsCallbackInterface.OnSlot4;
                @Slot4.canceled -= m_Wrapper.m_SlotSelectActionsCallbackInterface.OnSlot4;
                @Slot5.started -= m_Wrapper.m_SlotSelectActionsCallbackInterface.OnSlot5;
                @Slot5.performed -= m_Wrapper.m_SlotSelectActionsCallbackInterface.OnSlot5;
                @Slot5.canceled -= m_Wrapper.m_SlotSelectActionsCallbackInterface.OnSlot5;
                @Slot6.started -= m_Wrapper.m_SlotSelectActionsCallbackInterface.OnSlot6;
                @Slot6.performed -= m_Wrapper.m_SlotSelectActionsCallbackInterface.OnSlot6;
                @Slot6.canceled -= m_Wrapper.m_SlotSelectActionsCallbackInterface.OnSlot6;
                @Slot7.started -= m_Wrapper.m_SlotSelectActionsCallbackInterface.OnSlot7;
                @Slot7.performed -= m_Wrapper.m_SlotSelectActionsCallbackInterface.OnSlot7;
                @Slot7.canceled -= m_Wrapper.m_SlotSelectActionsCallbackInterface.OnSlot7;
            }
            m_Wrapper.m_SlotSelectActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Slot1.started += instance.OnSlot1;
                @Slot1.performed += instance.OnSlot1;
                @Slot1.canceled += instance.OnSlot1;
                @Slot2.started += instance.OnSlot2;
                @Slot2.performed += instance.OnSlot2;
                @Slot2.canceled += instance.OnSlot2;
                @Slot3.started += instance.OnSlot3;
                @Slot3.performed += instance.OnSlot3;
                @Slot3.canceled += instance.OnSlot3;
                @Slot4.started += instance.OnSlot4;
                @Slot4.performed += instance.OnSlot4;
                @Slot4.canceled += instance.OnSlot4;
                @Slot5.started += instance.OnSlot5;
                @Slot5.performed += instance.OnSlot5;
                @Slot5.canceled += instance.OnSlot5;
                @Slot6.started += instance.OnSlot6;
                @Slot6.performed += instance.OnSlot6;
                @Slot6.canceled += instance.OnSlot6;
                @Slot7.started += instance.OnSlot7;
                @Slot7.performed += instance.OnSlot7;
                @Slot7.canceled += instance.OnSlot7;
            }
        }
    }
    public SlotSelectActions @SlotSelect => new SlotSelectActions(this);

    // UIMouse
    private readonly InputActionMap m_UIMouse;
    private IUIMouseActions m_UIMouseActionsCallbackInterface;
    private readonly InputAction m_UIMouse_LeftClick;
    private readonly InputAction m_UIMouse_RightClick;
    private readonly InputAction m_UIMouse_Scroll;
    public struct UIMouseActions
    {
        private @UIInputActions m_Wrapper;
        public UIMouseActions(@UIInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @LeftClick => m_Wrapper.m_UIMouse_LeftClick;
        public InputAction @RightClick => m_Wrapper.m_UIMouse_RightClick;
        public InputAction @Scroll => m_Wrapper.m_UIMouse_Scroll;
        public InputActionMap Get() { return m_Wrapper.m_UIMouse; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UIMouseActions set) { return set.Get(); }
        public void SetCallbacks(IUIMouseActions instance)
        {
            if (m_Wrapper.m_UIMouseActionsCallbackInterface != null)
            {
                @LeftClick.started -= m_Wrapper.m_UIMouseActionsCallbackInterface.OnLeftClick;
                @LeftClick.performed -= m_Wrapper.m_UIMouseActionsCallbackInterface.OnLeftClick;
                @LeftClick.canceled -= m_Wrapper.m_UIMouseActionsCallbackInterface.OnLeftClick;
                @RightClick.started -= m_Wrapper.m_UIMouseActionsCallbackInterface.OnRightClick;
                @RightClick.performed -= m_Wrapper.m_UIMouseActionsCallbackInterface.OnRightClick;
                @RightClick.canceled -= m_Wrapper.m_UIMouseActionsCallbackInterface.OnRightClick;
                @Scroll.started -= m_Wrapper.m_UIMouseActionsCallbackInterface.OnScroll;
                @Scroll.performed -= m_Wrapper.m_UIMouseActionsCallbackInterface.OnScroll;
                @Scroll.canceled -= m_Wrapper.m_UIMouseActionsCallbackInterface.OnScroll;
            }
            m_Wrapper.m_UIMouseActionsCallbackInterface = instance;
            if (instance != null)
            {
                @LeftClick.started += instance.OnLeftClick;
                @LeftClick.performed += instance.OnLeftClick;
                @LeftClick.canceled += instance.OnLeftClick;
                @RightClick.started += instance.OnRightClick;
                @RightClick.performed += instance.OnRightClick;
                @RightClick.canceled += instance.OnRightClick;
                @Scroll.started += instance.OnScroll;
                @Scroll.performed += instance.OnScroll;
                @Scroll.canceled += instance.OnScroll;
            }
        }
    }
    public UIMouseActions @UIMouse => new UIMouseActions(this);

    // Toggles
    private readonly InputActionMap m_Toggles;
    private ITogglesActions m_TogglesActionsCallbackInterface;
    private readonly InputAction m_Toggles_Escape;
    private readonly InputAction m_Toggles_Inventory;
    public struct TogglesActions
    {
        private @UIInputActions m_Wrapper;
        public TogglesActions(@UIInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Escape => m_Wrapper.m_Toggles_Escape;
        public InputAction @Inventory => m_Wrapper.m_Toggles_Inventory;
        public InputActionMap Get() { return m_Wrapper.m_Toggles; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(TogglesActions set) { return set.Get(); }
        public void SetCallbacks(ITogglesActions instance)
        {
            if (m_Wrapper.m_TogglesActionsCallbackInterface != null)
            {
                @Escape.started -= m_Wrapper.m_TogglesActionsCallbackInterface.OnEscape;
                @Escape.performed -= m_Wrapper.m_TogglesActionsCallbackInterface.OnEscape;
                @Escape.canceled -= m_Wrapper.m_TogglesActionsCallbackInterface.OnEscape;
                @Inventory.started -= m_Wrapper.m_TogglesActionsCallbackInterface.OnInventory;
                @Inventory.performed -= m_Wrapper.m_TogglesActionsCallbackInterface.OnInventory;
                @Inventory.canceled -= m_Wrapper.m_TogglesActionsCallbackInterface.OnInventory;
            }
            m_Wrapper.m_TogglesActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Escape.started += instance.OnEscape;
                @Escape.performed += instance.OnEscape;
                @Escape.canceled += instance.OnEscape;
                @Inventory.started += instance.OnInventory;
                @Inventory.performed += instance.OnInventory;
                @Inventory.canceled += instance.OnInventory;
            }
        }
    }
    public TogglesActions @Toggles => new TogglesActions(this);
    public interface ISlotSelectActions
    {
        void OnSlot1(InputAction.CallbackContext context);
        void OnSlot2(InputAction.CallbackContext context);
        void OnSlot3(InputAction.CallbackContext context);
        void OnSlot4(InputAction.CallbackContext context);
        void OnSlot5(InputAction.CallbackContext context);
        void OnSlot6(InputAction.CallbackContext context);
        void OnSlot7(InputAction.CallbackContext context);
    }
    public interface IUIMouseActions
    {
        void OnLeftClick(InputAction.CallbackContext context);
        void OnRightClick(InputAction.CallbackContext context);
        void OnScroll(InputAction.CallbackContext context);
    }
    public interface ITogglesActions
    {
        void OnEscape(InputAction.CallbackContext context);
        void OnInventory(InputAction.CallbackContext context);
    }
}
