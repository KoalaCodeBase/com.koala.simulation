#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace Koala.Simulation.Inventory
{
    [CustomPropertyDrawer(typeof(ContainerSlot))]
    public class ContainerSlotDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var worldPointProp = property.FindPropertyRelative("WorldPoint");
            var itemProp = property.FindPropertyRelative("Item");

            float halfWidth = position.width * 0.5f - 5f;
            Rect worldRect = new Rect(position.x, position.y, halfWidth, EditorGUIUtility.singleLineHeight);
            Rect itemRect = new Rect(position.x + halfWidth + 10, position.y, halfWidth, EditorGUIUtility.singleLineHeight);

            EditorGUI.PropertyField(worldRect, worldPointProp, GUIContent.none);
            EditorGUI.PropertyField(itemRect, itemProp, GUIContent.none);

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + 2;
        }
    }
}
#endif