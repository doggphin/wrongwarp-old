using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WrongWarp
{
    public class InputManager : MonoBehaviour
    {
        private static readonly IDictionary<string, int> mapStates = new Dictionary<string, int>();

        private const int amountOfMaps = 2;
        private static PlayerInputActions playerControls;
        public static PlayerInputActions PlayerControls
        {
            get
            {
                if (playerControls != null) { return playerControls; }
                return playerControls = new PlayerInputActions();
            }
        }

        private static UIInputActions uiInput;
        public static UIInputActions UIInput
        {
            get
            {
                if (uiInput != null) { return uiInput; }
                return uiInput = new UIInputActions();
            }
        }

        private void Awake()
        {
            if (playerControls == null) { playerControls = new PlayerInputActions(); }
            if (uiInput == null) { uiInput = new UIInputActions(); }
        }

        private void OnEnable() => Enabler();
        private void OnDisable() => Disabler();
        private void OnDestroy() => playerControls = null;

        /// <summary> Fully disables all input. </summary>
        private void Disabler()
        {
            playerControls.Disable();
            uiInput.Disable();
        }

        /// <summary> Fully enables all input. </summary>
        private void Enabler()
        {
            playerControls.Disable();
            uiInput.Disable();
        }

        /// <summary> Adds one lock layer of a given map name. </summary>
        public static void AddLock(string mapName)
        {
            mapStates.TryGetValue(mapName, out int value);
            mapStates[mapName] = value + 1;
            UpdateMapState(mapName);
        }

        /// <summary> Removes one lock layer of a given map name. </summary>
        public static void RemoveLock(string mapName)
        {
            mapStates.TryGetValue(mapName, out int value);
            mapStates[mapName] = Mathf.Max(value - 1, 0);
            UpdateMapState(mapName);
        }

        /// <summary> Checks if an InputActionMap should be enabled or disabled. </summary>
        private static void UpdateMapState(string mapName)
        {
            int value = mapStates[mapName]; // Find the amount of locks on this map
            InputActionMap actionMap = FindActionMap(mapName);

            if (actionMap == null)          // If no Controls found, return and do nothing
            {
                Debug.Log($"Error: Control map of '{mapName}' not found.");
                return;
            }

            if (value > 0)                     // If at least one lock exists, disable the actionMap
            {
                actionMap.Disable();
                return;
            }

            actionMap.Enable();             // If no locks exist, enable the actionMap
        }

        /// <summary> Searches all referenced InputActionsAsset scripts in InputManager for an InputActionMap of a given name.
        /// <para> Needs to be refactored every time a new InputActionsAsset is created. Could be written to be a database in the future. </para></summary>
        private static InputActionMap FindActionMap(string mapName)
        {
            InputActionMap actionMap = null;
            for (int i = 0; i < amountOfMaps; i++)
            {
                switch (i)
                {
                    case 0:
                        actionMap = PlayerControls.asset.FindActionMap(mapName);
                        break;
                    case 1:
                        actionMap = UIInput.asset.FindActionMap(mapName);
                        break;
                    default:
                        Debug.Log($"Action map {mapName} not found.");
                        break;
                }

                if (actionMap != null)
                {
                    return actionMap;
                }
            }

            if (actionMap == null)
            {
                Debug.Log($"Action map {mapName} not found.");
                return null;
            }

            return actionMap;
        }
    }
}