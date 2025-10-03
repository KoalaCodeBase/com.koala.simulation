using Koala.Simulation.Inventory;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Koala.Simulation.Save.Editor
{
    [CustomEditor(typeof(PrefabRegistry))]
    public class PrefabRegistryEditor : UnityEditor.Editor
    {
        private ReorderableList _containerList;
        private ReorderableList _itemList;

        private void OnEnable()
        {
            _containerList = CreateList(serializedObject.FindProperty("_containerPrefabs"), "Container Prefabs");
            _itemList = CreateList(serializedObject.FindProperty("_itemPrefabs"), "Item Prefabs");
        }

        private ReorderableList CreateList(SerializedProperty property, string header)
        {
            var list = new ReorderableList(
                serializedObject,
                property,
                true,  // draggable
                true,  // display header
                true,  // display add button
                true   // display remove button
            );

            list.drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, header);
            };

            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = property.GetArrayElementAtIndex(index);

                Rect prefabRect = new Rect(rect.x, rect.y + 2, rect.width - 70, EditorGUIUtility.singleLineHeight);
                Rect previewRect = new Rect(rect.x + rect.width - 64, rect.y, 64, 64);

                EditorGUI.PropertyField(prefabRect, element, GUIContent.none);

                GameObject prefabObj = element.objectReferenceValue as GameObject;
                if (prefabObj != null)
                {
                    Texture2D preview = AssetPreview.GetAssetPreview(prefabObj);
                    if (preview != null)
                        GUI.DrawTexture(previewRect, preview, ScaleMode.ScaleToFit);
                }
            };

            list.elementHeightCallback = index =>
            {
                return Mathf.Max(EditorGUIUtility.singleLineHeight + 6, 68);
            };

            return list;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            _containerList.DoLayoutList();
            _itemList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();
        }
    }
}