using UnityEditor;
using UnityEngine;
using Koala.Simulation.Chronos;

namespace Koala.Simulation.Editor
{
    [CustomEditor(typeof(SimulationManager))]
    public class SimulationManagerEditor : UnityEditor.Editor
    {
        private SerializedProperty _prefabRegistry;
        private SerializedProperty _mainCamera;
        private SerializedProperty _placementRange;
        private SerializedProperty _placementRotationStep;
        private SerializedProperty _placementSmoothSpeed;
        private SerializedProperty _cycleLengthInMinutes;
        private SerializedProperty _dayStartHour;
        private SerializedProperty _nightStartHour;
        private SerializedProperty _dateOnlyLoad;
        private SerializedProperty _inputActionAsset;
        private SerializedProperty _defaultActionMap;
        private SerializedProperty _inputSpriteAsset;
        private SerializedProperty _autoLoadContainers;

        private static bool _placementFoldout = false;
        private static bool _simulationTimeFoldout = false;
        private static bool _inputManagerFoldout = false;
        private static bool _inventoryFoldout = false;

        private void OnEnable()
        {
            _prefabRegistry = serializedObject.FindProperty("_prefabRegistry");
            _mainCamera = serializedObject.FindProperty("_mainCamera");
            _placementRange = serializedObject.FindProperty("_placementRange");
            _placementRotationStep = serializedObject.FindProperty("_placementRotationStep");
            _placementSmoothSpeed = serializedObject.FindProperty("_placementSmoothSpeed");
            _cycleLengthInMinutes = serializedObject.FindProperty("_cycleLengthInMinutes");
            _dayStartHour = serializedObject.FindProperty("_dayStartHour");
            _nightStartHour = serializedObject.FindProperty("_nightStartHour");
            _dateOnlyLoad = serializedObject.FindProperty("_dateOnlyLoad");
            _inputActionAsset = serializedObject.FindProperty("_inputActionAsset");
            _defaultActionMap = serializedObject.FindProperty("_defaultActionMap");
            _inputSpriteAsset = serializedObject.FindProperty("_inputSpriteAsset");
            _autoLoadContainers = serializedObject.FindProperty("_autoLoadContainers");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_mainCamera);
            
            _placementFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_placementFoldout, "Placement");
            if (_placementFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.HelpBox("Access: PlacementManager.Instance", MessageType.None);
                EditorGUILayout.PropertyField(_placementRange, new GUIContent("Max Range"));
                EditorGUILayout.PropertyField(_placementRotationStep, new GUIContent("Rotation Step"));
                EditorGUILayout.PropertyField(_placementSmoothSpeed, new GUIContent("Smooth Speed"));
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            _simulationTimeFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_simulationTimeFoldout, "Time");
            if (_simulationTimeFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.HelpBox("Access: SimulationTime.Instance", MessageType.None);
                EditorGUILayout.PropertyField(_cycleLengthInMinutes, new GUIContent("Cycle Length (Minutes)"));
                EditorGUILayout.PropertyField(_dayStartHour, new GUIContent("Day Start Hour"));
                EditorGUILayout.PropertyField(_nightStartHour, new GUIContent("Night Start Hour"));
                EditorGUILayout.PropertyField(_dateOnlyLoad, new GUIContent("Load Only Date"));

                // --- DEBUG TIME (nested inside Time group) ---
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("⏱ Debug Time (Play Mode)", EditorStyles.boldLabel);
                if (Application.isPlaying && SimulationTime.Instance != null)
                {
                    var sim = SimulationTime.Instance;

                    int year = sim.CurrentYear;
                    int month = sim.CurrentMonth;
                    int day = sim.CurrentDay;
                    int hour = sim.CurrentHour;
                    int minute = sim.CurrentMinute;
                    int second = sim.CurrentSecond;

                    EditorGUI.BeginChangeCheck();

                    year = EditorGUILayout.IntField("Year", year);
                    month = EditorGUILayout.IntSlider("Month", month, 1, 12);
                    day = EditorGUILayout.IntSlider("Day", day, 1, 30);

                    EditorGUILayout.Space();
                    hour = EditorGUILayout.IntSlider("Hour", hour, 0, 23);
                    minute = EditorGUILayout.IntSlider("Minute", minute, 0, 59);
                    second = EditorGUILayout.IntSlider("Second", second, 0, 59);

                    if (EditorGUI.EndChangeCheck())
                    {
                        typeof(SimulationTime)
                            .GetField("_year", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                            .SetValue(sim, year);
                        typeof(SimulationTime)
                            .GetField("_month", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                            .SetValue(sim, month);
                        typeof(SimulationTime)
                            .GetField("_day", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                            .SetValue(sim, day);
                        typeof(SimulationTime)
                            .GetField("_hour", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                            .SetValue(sim, hour);
                        typeof(SimulationTime)
                            .GetField("_minute", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                            .SetValue(sim, minute);
                        typeof(SimulationTime)
                            .GetField("_second", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                            .SetValue(sim, second);
                    }

                    Repaint();
                }
                else
                {
                    EditorGUILayout.HelpBox("Enter Play Mode to debug time.", MessageType.Info);
                }

                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            _inputManagerFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_inputManagerFoldout, "Input");
            if (_inputManagerFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.HelpBox("Access: InputManager", MessageType.None);
                EditorGUILayout.PropertyField(_inputActionAsset);
                EditorGUILayout.PropertyField(_defaultActionMap);
                EditorGUILayout.PropertyField(_inputSpriteAsset);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            _inventoryFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(_inventoryFoldout, "Inventory");
            if (_inventoryFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_prefabRegistry);
                EditorGUILayout.PropertyField(_autoLoadContainers);
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            serializedObject.ApplyModifiedProperties();
        }
    }
}