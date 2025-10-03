using System;
using System.Collections.Generic;
using Koala.Simulation.Common;
using UnityEngine;

namespace Koala.Simulation.Input
{
    /// <summary>
    /// A collection of <xref uid="Koala.Simulation.Input.SpriteEntry" altProperty="fullName" displayProperty="name"/> objects.
    /// </summary>
    [CreateAssetMenu(fileName = "InputSpriteAsset", menuName = "Koala/SO/Input Sprite Asset")]
    public class InputSpriteAsset : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private List<SpriteEntry> _entries = new();
        private Dictionary<string, Sprite> _lookup;

        [DocfxIgnore]
        public Sprite this[string key] => _lookup != null && _lookup.TryGetValue(key, out var sprite) ? sprite : null;

        [DocfxIgnore]
        public bool Contains(string key) => _lookup != null && _lookup.ContainsKey(key);

        [DocfxIgnore]
        public void OnBeforeSerialize()
        {
        }

        [DocfxIgnore]
        public void OnAfterDeserialize()
        {
            BuildLookup();
        }

        private void OnEnable()
        {
            BuildLookup();
        }

        private void BuildLookup()
        {
            _lookup = new Dictionary<string, Sprite>(_entries.Count);
            foreach (var entry in _entries)
            {
                if (!string.IsNullOrEmpty(entry.Key) && !_lookup.ContainsKey(entry.Key))
                    _lookup.Add(entry.Key, entry.Value);
            }
        }

        [DocfxIgnore]
        public bool TryGetSprite(string key, out Sprite sprite)
        {
            return _lookup.TryGetValue(key, out sprite);
        }
    }

    /// <example>
    /// <code>
    /// [Serializable]
    /// public struct SpriteEntry
    /// {
    ///     // Action name inside the InputActionAsset.
    ///     public string Key;
    ///
    ///     // The sprite to be used.
    ///     public Sprite Value;
    /// }
    /// </code>
    /// </example>
    [Serializable]
    public struct SpriteEntry
    {
        /// <summary>
        /// Action name inside the InputActionAsset.
        /// </summary>
        public string Key;

        /// <summary>
        /// The sprite to be used.
        /// </summary>
        public Sprite Value;
    }
}