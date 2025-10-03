#if UNITY_EDITOR
using Koala.Simulation;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class SimulationManagerIcon
{
    static Texture2D icon;

    static SimulationManagerIcon()
    {
        EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Packages/com.koala.simulation/Editor/Icons/koala_important.png");
    }

    static void OnHierarchyGUI(int instanceID, Rect selectionRect)
    {
        GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (obj == null) return;

        if (obj.GetComponent<SimulationManager>() != null)
        {
            Rect r = new Rect(selectionRect.xMax - 16, selectionRect.y, 16, 16);
            GUI.DrawTexture(r, icon, ScaleMode.ScaleToFit, true);
        }
    }
}
#endif