using System;
using System.Collections.Generic;
using Koala.Simulation.Common;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Koala.Simulation.Input
{
    /// <summary>
    /// Centralized input system built on Unity's new Input System.
    /// 
    /// Provides event-based access to all input actions, automatic handling of 
    /// action maps, and support for runtime map switching with stack semantics.
    /// 
    /// Key features:
    /// - Subscribe/unsubscribe to any action by name
    /// - Global <see cref="OnAnyActionStarted"/> event for catching all input started phase
    /// - Global <see cref="OnAnyActionPhase"/> event for catching all input phase (started, performed, canceled)
    /// - Switch, push, and pop between multiple action maps
    /// - Notifies when the active action map changes
    /// - Integrates with <c>InputSettingsService</c> for binding overrides
    /// 
    /// Access this class directly via <c>InputManager</c>.
    /// Requires <c>SimulationManager</c> to be present in the scene.
    /// Only one instance of <c>SimulationManager</c> should exist.
    /// </summary>
    /// <example>
    /// Example: Subscribing to the "Jump" action
    /// <code>
    /// private void OnEnable()
    /// {
    ///     InputManager.Subscribe("Jump", OnJump);
    /// }
    ///
    /// private void OnDisable()
    /// {
    ///     InputManager.Unsubscribe("Jump", OnJump);
    /// }
    ///
    /// private void OnJump(InputAction.CallbackContext ctx)
    /// {
    ///     if (ctx.performed)
    ///         Debug.Log("Jump pressed!");
    /// }
    /// </code>
    /// </example>
    /// <example>
    /// Example: Listening to all actions
    /// <code>
    /// private void OnEnable()
    /// {
    ///     InputManager.OnAnyAction += OnAny;
    /// }
    ///
    /// private void OnDisable()
    /// {
    ///     InputManager.OnAnyAction -= OnAny;
    /// }
    ///
    /// private void OnAny(InputAction.CallbackContext ctx)
    /// {
    ///     Debug.Log($"Action fired: {ctx.action.name}");
    /// }
    /// </code>
    /// </example>
    /// <example>
    /// Example: Switching action maps
    /// <code>
    /// // Set UI as the active map (stack cleared)
    /// InputManager.SetActionMap("UI");
    ///
    /// // Push Pause menu map (UI stays underneath)
    /// InputManager.PushActionMap("Pause");
    ///
    /// // Pop Pause -> returns to UI
    /// InputManager.PopActionMap();
    /// </code>
    /// </example>
    public sealed class InputManager : IDisposable
    {
        internal InputActionAsset _actionAsset;
        internal string _defaultActionMap = "Gameplay";
        internal InputSpriteAsset _inputSpriteAsset;

        private static InputManager _instance;
        private InputActionMap _actionMap;

        private readonly Dictionary<string, Action<InputAction.CallbackContext>> _actionEvents = new();
        private static readonly Action<InputAction.CallbackContext> EmptyAction = _ => { };
        private readonly Stack<string> _mapStack = new();
        private readonly Dictionary<string, string> _displayNameCache = new();

        /// <summary>
        /// Raised when any input action enters the Started phase.
        /// Provides the <see cref="InputAction.CallbackContext"/> for the event.
        /// </summary>
        public static event Action<InputAction.CallbackContext> OnAnyActionStarted;
        /// <summary>
        /// Raised for every input action, regardless of phase.
        /// Provides the <see cref="InputAction.CallbackContext"/> for the event.
        /// </summary>
        public static event Action<InputAction.CallbackContext> OnAnyActionPhase;
        /// <summary>
        /// Raised whenever the active action map changes.
        /// Provides the new action map's name.
        /// </summary>
        public static event Action<string> OnActionMapChanged;

        public InputManager(InputActionAsset actionAsset, string defaultActionMap, InputSpriteAsset inputSpriteAsset)
        {
            if (_instance != null)
                throw new InvalidOperationException("InputManager already initialized.");

            _instance = this;

            _actionAsset = actionAsset;
            _defaultActionMap = defaultActionMap;
            _inputSpriteAsset = inputSpriteAsset;

            SetActionMap(_defaultActionMap);
            InputSettingsService.LoadOverrides(_actionAsset);
        }

        /// <summary>
        /// For internal use. Calling this incorrectly may lead to unexpected behavior.
        /// </summary>
        [DocfxIgnore]
        public void Dispose()
        {
            if (_instance == this)
            {
                if (_actionMap != null)
                {
                    foreach (var action in _actionMap.actions)
                    {
                        action.started -= HandleAction;
                        action.performed -= HandleAction;
                        action.canceled -= HandleAction;
                    }
                }

                OnAnyActionStarted = null;
                OnAnyActionPhase = null;
                OnActionMapChanged = null;
                _instance = null;
            }
        }

        private void HandleAction(InputAction.CallbackContext ctx)
        {
            OnAnyActionPhase?.Invoke(ctx);

            if (ctx.phase == InputActionPhase.Started)
            {
                OnAnyActionStarted?.Invoke(ctx);
            }

            if (_actionEvents.TryGetValue(ctx.action.name, out var evt))
            {
                evt?.Invoke(ctx);
            }
        }

        /// <summary>
        /// Subscribes a callback to the specified action by name. The callback is invoked for **all phases** of the action (Started, Performed, and Canceled).
        /// Use this when you need the full <see cref="InputAction.CallbackContext"/>.
        /// <para> Example: <c>InputManager.Subscribe("Jump", OnJump)</c>.</para>
        /// </summary>
        public static void Subscribe(string actionName, Action<InputAction.CallbackContext> callback)
        {
            if (_instance == null) return;

            if (!_instance._actionEvents.ContainsKey(actionName))
                _instance._actionEvents[actionName] = EmptyAction;

            _instance._actionEvents[actionName] += callback;
        }
        /// <summary>
        /// Subscribes a callback to the specified action by name, filtered by phase.
        /// 
        /// The callback is only invoked when the action enters the given phase 
        /// (Started, Performed, or Canceled).
        /// Defaults to <see cref="InputActionPhase.Started"/> for 
        /// button-like behavior.
        /// <para> Example:
        /// Called only once when "Jump" starts
        /// <c>InputManager.Subscribe("Jump", OnJump, InputActionPhase.Started);</c>
        ///
        /// Called every frame while "Move" is performed
        /// <c>InputManager.Subscribe("Move", OnMove, InputActionPhase.Performed);</c>
        /// </para>
        /// </summary>
        public static void Subscribe(string actionName, Action<InputAction.CallbackContext> callback, InputActionPhase phase = InputActionPhase.Started)
        {
            if (_instance == null) return;

            if (!_instance._actionEvents.ContainsKey(actionName))
                _instance._actionEvents[actionName] = EmptyAction;

            _instance._actionEvents[actionName] += ctx =>
            {
                if (ctx.phase == phase)
                    callback(ctx);
            };
        }
        /// <summary>
        /// Unsubscribe from a previously subscribed action.
        /// </summary>
        public static void Unsubscribe(string actionName, Action<InputAction.CallbackContext> callback)
        {
            if (_instance == null) return;

            if (_instance._actionEvents.ContainsKey(actionName))
                _instance._actionEvents[actionName] -= callback;
        }
        /// <summary>
        /// Find an action by name in the currently active action map.
        /// Returns <c>null</c> if not found.
        /// </summary>
        public static InputAction FindAction(string actionName)
        {
            if (_instance == null) return null;
            return _instance._actionMap.FindAction(actionName, true);
        }
        /// <summary>
        /// Sets the active action map directly.
        /// Clears the internal stack and replaces it with the given map.
        /// </summary>
        public static void SetActionMap(string mapName)
        {
            if (_instance == null) return;

            _instance.SwitchMap(mapName);
            _instance._mapStack.Clear();
            _instance._mapStack.Push(mapName);
        }
        /// <summary>
        /// Pushes a new action map onto the stack and activates it.
        /// When popped, the previous map will be restored.
        /// </summary>
        public static void PushActionMap(string mapName)
        {
            if (_instance == null) return;

            _instance.SwitchMap(mapName);
            _instance._mapStack.Push(mapName);
        }
        /// <summary>
        /// Pops the current action map from the stack and restores the previous one.
        /// Does nothing if there is only one map left.
        /// </summary>
        public static void PopActionMap()
        {
            if (_instance == null) return;
            if (_instance._mapStack.Count <= 1) return;

            _instance._mapStack.Pop();
            string prevMap = _instance._mapStack.Peek();
            _instance.SwitchMap(prevMap);
        }

        private void SwitchMap(string mapName)
        {
            if (_actionMap != null)
            {
                foreach (var action in _actionMap.actions)
                {
                    action.started -= HandleAction;
                    action.performed -= HandleAction;
                    action.canceled -= HandleAction;
                }
                _actionMap.Disable();
            }

            _actionMap = _actionAsset.FindActionMap(mapName, true);

            foreach (var action in _actionMap.actions)
            {
                action.started += HandleAction;
                action.performed += HandleAction;
                action.canceled += HandleAction;
            }

            _actionMap.Enable();

            OnActionMapChanged?.Invoke(mapName);
        }
        /// <summary>
        /// Gets the display name of the binding for a given action.
        /// Uses caching for fast repeated access. 
        /// Call <see cref="InvalidateCache"/> after rebinding.
        /// </summary>
        public static string GetKeyDisplayName(string actionName, int bindingIndex = 0)
        {
            if (_instance == null) return null;

            string cacheKey = $"{actionName}_{bindingIndex}";
            if (_instance._displayNameCache.TryGetValue(cacheKey, out string cached))
                return cached;

            var action = _instance._actionMap.FindAction(actionName, false);
            if (action == null || bindingIndex < 0 || bindingIndex >= action.bindings.Count)
                return null;

            string display = action.GetBindingDisplayString(bindingIndex);
            _instance._displayNameCache[cacheKey] = display;
            return display;
        }

        /// <summary>
        /// Clears the cached display names. 
        /// Call this after rebinding or loading overrides.
        /// </summary>
        internal static void InvalidateCache()
        {
            if (_instance == null) return;
            _instance._displayNameCache.Clear();
        }

        /// <summary>
        /// Gets a sprite for a given action name if it exists. e.g LMB, RMB
        /// </summary>
        /// <param name="actionName">The name of the input action.</param>
        /// <param name="sprite">The corresponding sprite if found.</param>
        /// <returns>True if a sprite was found; otherwise, false.</returns>
        public static bool TryGetSprite(string actionName, out Sprite sprite)
        {
            return _instance._inputSpriteAsset.TryGetSprite(actionName, out sprite);
        }
        /// <summary>
        /// Gets the name of the currently active action map.
        /// Returns <c>null</c> if no map is active.
        /// </summary>
        public static string CurrentActionMap
        {
            get
            {
                if (_instance == null || _instance._actionMap == null) return null;
                return _instance._actionMap.name;
            }
        }
    }
}