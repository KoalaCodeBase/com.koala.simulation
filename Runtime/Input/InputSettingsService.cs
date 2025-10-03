using UnityEngine.InputSystem;

namespace Koala.Simulation.Input
{
    /// <summary>
    /// Handles saving, loading, and clearing of input binding overrides.
    /// 
    /// Works with Unity's new Input System to persist user key rebinding changes 
    /// into the simulation's save file (<c>world.es3</c>) using Easy Save (ES3).
    /// 
    /// Key features:
    /// - Saves all current binding overrides as JSON
    /// - Loads and applies saved overrides at startup
    /// - Clears overrides and resets to default bindings
    /// - Automatically invalidates the <see cref="InputManager"/> cache
    /// 
    /// This service is called internally by <see cref="InputRebindService"/>, 
    /// but can also be used directly if needed.
    /// </summary>
    /// <example>
    /// Example: Saving and loading overrides manually
    /// <code>
    /// // Save current bindings
    /// InputSettingsService.SaveOverrides(myActionAsset);
    ///
    /// // Load bindings on startup
    /// InputSettingsService.LoadOverrides(myActionAsset);
    ///
    /// // Reset all bindings to defaults
    /// InputSettingsService.ClearOverrides(myActionAsset);
    /// </code>
    /// </example>
    public static class InputSettingsService
    {
        private const string SaveFile = "world.es3";
        private const string InputKey = "InputBindings";

        /// <summary>
        /// Saves all binding overrides from the given action asset into the save file.
        /// Automatically invalidates the <see cref="InputManager"/> cache.
        /// </summary>
        /// <param name="asset">The InputActionAsset containing overrides to save.</param>
        public static void SaveOverrides(InputActionAsset asset)
        {
            string json = asset.SaveBindingOverridesAsJson();
            ES3.Save(InputKey, json, SaveFile);

            InputManager.InvalidateCache();
        }

        /// <summary>
        /// Loads binding overrides from the save file into the given action asset.
        /// Automatically invalidates the <see cref="InputManager"/> cache.
        /// </summary>
        /// <param name="asset">The InputActionAsset to apply overrides to.</param>
        public static void LoadOverrides(InputActionAsset asset)
        {
            if (!ES3.KeyExists(InputKey, SaveFile))
                return;

            string json = ES3.Load<string>(InputKey, SaveFile);
            if (!string.IsNullOrEmpty(json))
                asset.LoadBindingOverridesFromJson(json);

            InputManager.InvalidateCache();
        }

        /// <summary>
        /// Clears all binding overrides from the given action asset and deletes saved data.
        /// Automatically invalidates the <see cref="InputManager"/> cache.
        /// </summary>
        /// <param name="asset">The InputActionAsset to clear overrides from.</param>
        public static void ClearOverrides(InputActionAsset asset)
        {
            asset.RemoveAllBindingOverrides();
            if (ES3.KeyExists(InputKey, SaveFile))
                ES3.DeleteKey(InputKey, SaveFile);

            InputManager.InvalidateCache();
        }
    }
}