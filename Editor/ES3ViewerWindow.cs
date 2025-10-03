using UnityEditor;
using UnityEngine;
using System.IO;
using Unity.Plastic.Newtonsoft.Json.Linq;
using System.Text;
using System.Linq;

public class ES3ViewerWindow : EditorWindow
{
    private Vector2 _scrollPos;
    private string _filePath = "save.es3";
    private string _summary;

    [MenuItem("Tools/Koala/Save Viewer", false, 1)]
    private static void Open()
    {
        GetWindow<ES3ViewerWindow>("Save Viewer");
    }

    private void OnEnable()
    {
        _filePath = EditorPrefs.GetString("ES3Viewer_LastPath", _filePath);
        LoadSnapshot();
    }

    private void OnDisable()
    {
        if (!string.IsNullOrEmpty(_filePath))
            EditorPrefs.SetString("ES3Viewer_LastPath", _filePath);
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        _filePath = EditorGUILayout.TextField("File Path", _filePath);

        if (GUILayout.Button("Browse", GUILayout.Width(70)))
        {
            string selected = EditorUtility.OpenFilePanel("Select ES3 Save", Application.dataPath, "es3");
            if (!string.IsNullOrEmpty(selected))
                _filePath = selected;
        }

        if (GUILayout.Button("Refresh", GUILayout.Width(70)))
        {
            LoadSnapshot();
        }
        EditorGUILayout.EndHorizontal();

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
        if (!string.IsNullOrEmpty(_summary))
            EditorGUILayout.TextArea(_summary, GUILayout.ExpandHeight(true));
        else
            EditorGUILayout.HelpBox("No data loaded.", MessageType.Info);
        EditorGUILayout.EndScrollView();
    }

    private void LoadSnapshot()
    {
        if (File.Exists(_filePath))
        {
            try
            {
                using (var stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(stream))
                {
                    string raw = reader.ReadToEnd();
                    _summary = BuildSummary(JObject.Parse(raw));
                }
            }
            catch
            {
                _summary = "Invalid JSON";
            }
        }
        else
        {
            _summary = "File not found.";
        }
    }

    private string BuildSummary(JObject root)
    {
        var sb = new StringBuilder();

        // --- SimulationTime ---
        if (root["SimulationTime"]?["value"] is JObject sim)
        {
            sb.AppendLine("⏳ Time");
            foreach (var prop in sim.Properties())
            {
                sb.AppendLine($" │   • {prop.Name}: {prop.Value}");
            }
            sb.AppendLine();
        }

        // --- Containers ---
        if (root["Containers"]?["value"] is JArray containers)
        {
            sb.AppendLine($"📦 Containers ({containers.Count}) ├─ [PrefabId] [UniqueId]");
            foreach (var c in containers)
            {
                sb.AppendLine($" ├─ [{c["PrefabId"]}] [{c["UniqueId"]}]");

                sb.AppendLine($" │   • Capacity: {c["Capacity"]}");
                string pos = string.Join(",", c["Position"] ?? new JArray());
                string rot = string.Join(",", c["Rotation"] ?? new JArray());
                sb.AppendLine($" │   • Position: ({pos})");
                sb.AppendLine($" │   • Rotation: ({rot})");

                if (c["Items"] is JArray items && items.Count > 0)
                {
                    var itemNames = string.Join(", ", items.Select(i => i["PrefabId"]?.ToString()));
                    sb.AppendLine($" │   • Items: {itemNames}");
                }
                else sb.AppendLine($" │   • Items: []");

                sb.AppendLine($" │   • Nested: {c["Nested"]}");
            }
            sb.AppendLine();
        }

        sb.AppendLine("🐨");

        return sb.ToString();
    }
}