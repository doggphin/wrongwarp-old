using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace WrongWarp
{
    public class UIManager : MonoBehaviour
    {
        [Header("Specialized UI Managers")]
        [SerializeField] private GameObject inventoryDisplayManagerPrefab;
        public InventoryDisplayManager inventoryDisplayManager { get; private set; }

        private List<IToggleableUI> openedUILayers = new();                     // Layers of UI, where the last index in the list is the most recently opened.
        GraphicRaycaster m_Raycaster;                                           // The current canvas GraphicRaycaster to raycast on.

        private Dictionary<UIType, IToggleableUI> typesToUIPrefabs;             // Connects enum UIType types with IToggleableUI instances 
        private Dictionary<MouseInputType, Type> clicksToInterfaces = new()     // Connects mouse inputs to the interface that uses them
        {
            { MouseInputType.LeftDown, typeof(ILeftClickable) },
            { MouseInputType.RightDown, typeof(IRightClickable) },
            { MouseInputType.LeftUp, typeof(ILeftClickDroppable) },
            { MouseInputType.RightUp, typeof(IRightClickDroppable) },
            { MouseInputType.Scroll, typeof(IScrollable) }
        };

        /// <summary> Instantiates all UI managers (eg, an InventoryDisplayManager). </summary>
        public void ClientInit()
        {
            // Instantiate UI
            inventoryDisplayManager = Instantiate(inventoryDisplayManagerPrefab, gameObject.transform).GetComponent<InventoryDisplayManager>();

            // Set up uiLayerTypes dictionary
            typesToUIPrefabs = new Dictionary<UIType, IToggleableUI>()
        {
            { UIType.InventoryMenu, inventoryDisplayManager }
        };

            // Subscribe to UI specific input actions
            InputManager.UIInput.Toggles.Inventory.started += ctx => ToggleUI(UIType.InventoryMenu);
            InputManager.UIInput.Toggles.Escape.started += ctx => Escape();
        }

        // ============================================
        // MOUSE INPUT
        // ============================================

        #region Mouse Input

        /// <summary> Raycasts a mouse input on the uppermost canvas layer searching for any GameObject that responds to the type of click used, then calls that GameObject's respective mosue input function.
        /// <para> "value" is only used for scrolling. </para></summary>
        private void MouseAction(MouseInputType mouseInputType, float value = 0)
        {
            RaycastResult result = RaycastForInterface(clicksToInterfaces[mouseInputType]);
            if (!result.isValid) { return; }    // If raycast didn't hit anything, exit.

            switch (mouseInputType)
            {
                case MouseInputType.LeftDown:
                    result.gameObject.GetComponent<ILeftClickable>().OnLeftClicked();
                    break;
                case MouseInputType.RightDown:
                    result.gameObject.GetComponent<IRightClickable>().OnRightClicked();
                    break;
                case MouseInputType.LeftUp:
                    result.gameObject.GetComponent<ILeftClickDroppable>().OnLeftClickDropped();
                    break;
                case MouseInputType.RightUp:
                    result.gameObject.GetComponent<IRightClickDroppable>().OnRightClickDropped();
                    break;
                case MouseInputType.Scroll:
                    result.gameObject.GetComponent<IScrollable>().OnScrolled(value);
                    break;
                default:
                    Debug.Log("ERROR: Mouse input type not implemented.");
                    return;
            }

            //Debug.Log("Hit " + result.gameObject.name);
        }

        /// <summary> Sends a raycast into the current canvas looking for an interface type. Returns the first GameObject it hits using the interface.
        /// <para> If no interface is found, returns a default (non-valid) RaycastResult. </para></summary
        private RaycastResult RaycastForInterface(Type interfaceToFind)
        {
            PointerEventData m_PointerEventData = new(EventSystem.current);
            m_PointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new();

            m_Raycaster.Raycast(m_PointerEventData, results);   // Raycast into the current canvas layer and store hits in results

            // Return the first result that implements interfaceToFind
            for (int i = 0; i < results.Count; i++)
            {
                foreach (RaycastResult result in results)
                {
                    if (result.gameObject.GetComponent(interfaceToFind) != null)
                    {
                        return result;
                    }
                }
            }

            return new RaycastResult();                         // If none use interfaceToFind, return a default (non-valid) RaycastResult struct
        }

        #endregion Mouse Input

        // ============================================
        // LAYERS MANAGEMENT
        // ============================================

        #region Layers Management

        /// <summary> Adds a specific layer of UI. If this is the first layer, disable mouse controls. </summary>
        private void AddLayer(UIType uiType)
        {
            typesToUIPrefabs[uiType].OpenUI();
            openedUILayers.Add(typesToUIPrefabs[uiType]);
            if (openedUILayers.Count == 1) { StartUIControls(); }         // If this is the first layer, start UI controls.

            m_Raycaster = typesToUIPrefabs[uiType].graphicRaycaster;    // Sets this layer, which is the top layer, to the canvas to be raycasted.
        }

        /// <summary> Removes the top layer of UI. If this removes the final layer, reenable mouse controls. </summary>
        private void RemoveTopLayer()
        {
            openedUILayers[^1].CloseUI();
            openedUILayers.RemoveAt(openedUILayers.Count - 1);
            if (openedUILayers.Count == 0) { StopUIControls(); }
        }

        #endregion Layers Management

        // ============================================
        // CONTROLS MANAGEMENT
        // ============================================

        #region Controls Management

        /// <summary> Opens or closes inventory display. </summary>
        public void ToggleUI(UIType uiType)
        {
            if (openedUILayers.Count == 0)
            {
                AddLayer(uiType);
                return;
            }

            if (openedUILayers[^1].isOpen && openedUILayers[^1].uiType == uiType)
            {
                RemoveTopLayer();
            }
            else
            {
                AddLayer(uiType);
            }
        }

        /// <summary> Either closes the current menu, or if none, opens the escape menu. </summary>
        public void Escape()
        {
            if (openedUILayers.Count != 0)
            {
                RemoveTopLayer();
                return;
            }
            else
            {
                // Open escape menu; not yet implemented
            }
        }


        /// <summary> Puts controls into an "opened UI" state (disable player look, show and unlock mouse). </summary>
        private void StartUIControls()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            InputManager.AddLock("PlayerLook");
            InputManager.AddLock("PlayerInteract");
            InputManager.AddLock("PlayerUse");
            SubscribeToClicks();
        }

        /// <summary> Puts controls into an "closed UI" state (enable player look, hide and lock mouse to center of screen). </summary>
        private void StopUIControls()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = true;
            InputManager.RemoveLock("PlayerLook");
            InputManager.RemoveLock("PlayerInteract");
            InputManager.RemoveLock("PlayerUse");
            UnsubscribeToClicks();
        }

        /// <summary> Makes UIManager start listening to mouse InputActions. </summary>
        private void SubscribeToClicks()
        {
            InputManager.UIInput.UIMouse.LeftClick.started += ctx => MouseAction(MouseInputType.LeftDown);
            InputManager.UIInput.UIMouse.LeftClick.canceled += ctx => MouseAction(MouseInputType.LeftUp);
            InputManager.UIInput.UIMouse.RightClick.started += ctx => MouseAction(MouseInputType.RightDown);
            InputManager.UIInput.UIMouse.RightClick.canceled += ctx => MouseAction(MouseInputType.RightUp);
            InputManager.UIInput.UIMouse.Scroll.performed += ctx => MouseAction(MouseInputType.Scroll, ctx.ReadValue<float>());
        }

        /// <summary> Stops UIManager from listening to mouse InputActions. </summary>
        private void UnsubscribeToClicks()
        {
            InputManager.UIInput.UIMouse.LeftClick.started -= ctx => MouseAction(MouseInputType.LeftDown);
            InputManager.UIInput.UIMouse.LeftClick.canceled -= ctx => MouseAction(MouseInputType.LeftUp);
            InputManager.UIInput.UIMouse.RightClick.started -= ctx => MouseAction(MouseInputType.RightDown);
            InputManager.UIInput.UIMouse.RightClick.canceled -= ctx => MouseAction(MouseInputType.RightUp);
            InputManager.UIInput.UIMouse.Scroll.performed -= ctx => MouseAction(MouseInputType.Scroll, ctx.ReadValue<float>());
        }

        #endregion Controls Management

    }

    public enum MouseInputType
    {
        LeftDown,
        RightDown,
        LeftUp,
        RightUp,
        Scroll
    }

    public enum UIType
    {
        InventoryMenu,
        EscapeMenu
    }
}