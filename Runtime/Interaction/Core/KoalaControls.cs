using Koala.Simulation.Common;
using UnityEditor;
using UnityEngine;

namespace Koala.Simulation.Interaction.Core
{
    /// <summary>
    /// Provides editor utilities for creating interaction display UI elements.
    /// </summary>
    [DocfxIgnore]
    public class KoalaControls
    {
        /// <summary>
        /// Creates a new interaction display UI element.
        /// </summary>
        /// <param name="menuCommand">The menu command used to create the interaction display.</param>
        [MenuItem("GameObject/UI/Interaction Display", false, 10)]
        public static void CreateInteractionDisplay(MenuCommand menuCommand)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(
                "Packages/com.koala.simulation/Runtime/Interaction/Prefab/InteractionDisplay.prefab"
            );

            if (prefab == null)
            {
                Debug.LogError("Prefab not found: Packages/com.koala.simulation/Runtime/Interaction/Prefab/InteractionDisplay.prefab");
                return;
            }

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            instance.name = "Interaction Display";

            PrefabUtility.UnpackPrefabInstance(
                instance,
                PrefabUnpackMode.Completely,
                InteractionMode.UserAction
            );

            PlaceUIElementRoot(instance, menuCommand);
        }

        private static void PlaceUIElementRoot(GameObject element, MenuCommand menuCommand)
        {
            GameObject parent = menuCommand.context as GameObject;
            if (parent == null)
                parent = GetOrCreateCanvasGameObject();

            GameObjectUtility.SetParentAndAlign(element, parent);
            Undo.RegisterCreatedObjectUndo(element, "Create " + element.name);
            Selection.activeObject = element;
        }

        private static GameObject GetOrCreateCanvasGameObject()
        {
            var canvasGO = new GameObject("Canvas");
            canvasGO.layer = LayerMask.NameToLayer("UI");
            canvasGO.AddComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            Undo.RegisterCreatedObjectUndo(canvasGO, "Create " + canvasGO.name);
            return canvasGO;
        }
    }
}