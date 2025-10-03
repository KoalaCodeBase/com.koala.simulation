using System;
using UnityEngine.InputSystem;

namespace Koala.Simulation.Input
{
    /// <summary>
    /// Provides utility methods for rebinding input actions at runtime.
    /// 
    /// Wraps Unity's <c>InputAction.PerformInteractiveRebinding</c> API 
    /// and integrates with <xref uid="Koala.Simulation.Input.InputSettingsService" altProperty="fullName" displayProperty="name"/> to automatically save 
    /// binding overrides into the simulation's save system.
    /// 
    /// Key features:
    /// - Start an interactive rebind for any action and binding index
    /// - Automatically saves and restores overrides
    /// - Provides display strings for showing current bindings in UI
    /// 
    /// Commonly used from UI buttons or settings menus to allow players 
    /// to customize their controls.
    /// </summary>
    /// <example>
    /// Example: Rebinding the "Jump" action
    /// <code>
    /// public void OnClick_RebindJump()
    /// {
    ///     InputRebindService.StartRebind("Jump", 0,
    ///         onComplete: () =>
    ///         {
    ///             string display = InputRebindService.GetBindingDisplayName("Jump", 0);
    ///             Debug.Log($"Jump rebound → {display}");
    ///         });
    /// }
    /// </code>
    /// </example>
    public static class InputRebindService
    {
        /// <summary>
        /// Starts an interactive rebind for the given action and binding index.
        /// 
        /// Opens a "waiting for input" state. The next key/button pressed by the player 
        /// will be assigned to the binding. Automatically saves overrides on completion.
        /// </summary>
        /// <param name="actionName">The name of the input action to rebind.</param>
        /// <param name="bindingIndex">The binding index to rebind (usually 0).</param>
        /// <param name="onComplete">Optional callback when rebind succeeds.</param>
        /// <param name="onCancel">Optional callback when rebind is canceled.</param>
        /// <example>
        /// <code>
        /// InputRebindService.StartRebind("Attack", 0,
        ///     onComplete: () => Debug.Log("Attack rebound!"),
        ///     onCancel: () => Debug.Log("Rebind canceled."));
        /// </code>
        /// </example>
        public static void StartRebind(string actionName, int bindingIndex,
            Action onComplete = null, Action onCancel = null)
        {
            var action = InputManager.FindAction(actionName);
            if (action == null) return;

            action.Disable();

            var rebind = action.PerformInteractiveRebinding(bindingIndex)
                .OnComplete(operation =>
                {
                    operation.Dispose();
                    action.Enable();

                    InputSettingsService.SaveOverrides(action.actionMap.asset);

                    onComplete?.Invoke();
                })
                .OnCancel(operation =>
                {
                    operation.Dispose();
                    action.Enable();

                    onCancel?.Invoke();
                });

            rebind.Start();
        }

        /// <summary>
        /// Gets the human-readable display name of a binding.
        /// Example: "Space", "Left Mouse Button", "Gamepad Button South".
        /// </summary>
        /// <param name="actionName">The name of the input action.</param>
        /// <param name="bindingIndex">The binding index (default is 0).</param>
        /// <returns>A display string for the binding, or empty if not found.</returns>
        /// <example>
        /// <code>
        /// string display = InputRebindService.GetBindingDisplayName("Jump", 0);
        /// Debug.Log($"Jump is bound to: {display}");
        /// </code>
        /// </example>
        public static string GetBindingDisplayName(string actionName, int bindingIndex)
        {
            var action = InputManager.FindAction(actionName);
            if (action == null) return string.Empty;

            return action.GetBindingDisplayString(bindingIndex);
        }
    }
}