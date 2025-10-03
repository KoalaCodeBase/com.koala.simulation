using System.Collections.Generic;
using UnityEngine;

namespace Koala.Simulation.Inventory
{
    [CreateAssetMenu(fileName = "PrefabRegistry", menuName = "Koala/SO/PrefabRegistry")]
    public class PrefabRegistry : ScriptableObject
    {
        [SerializeField] private List<GameObject> _containerPrefabs = new();
        [SerializeField] private List<GameObject> _itemPrefabs = new();

        private Dictionary<string, GameObject> _containerLookup;
        private Dictionary<string, GameObject> _itemLookup;

        private void OnEnable()
        {
            BuildLookups();
        }

        private void BuildLookups()
        {
            _containerLookup = new Dictionary<string, GameObject>(_containerPrefabs.Count);
            _itemLookup = new Dictionary<string, GameObject>(_itemPrefabs.Count);

            foreach (var prefab in _containerPrefabs)
            {
                if (prefab == null) continue;

                if (prefab.TryGetComponent<Container>(out var container))
                {
                    if (!string.IsNullOrEmpty(container.PrefabId) && !_containerLookup.ContainsKey(container.PrefabId))
                        _containerLookup.Add(container.PrefabId, prefab);
                }
#if UNITY_EDITOR
                else
                {
                    Debug.LogWarning($"[PrefabRegistry] Wrong type added to container prefab list: {prefab.name}");
                }
#endif
            }

            foreach (var prefab in _itemPrefabs)
            {
                if (prefab == null) continue;

                if (prefab.TryGetComponent<Item>(out var item))
                {
                    if (!string.IsNullOrEmpty(item.PrefabId) && !_itemLookup.ContainsKey(item.PrefabId))
                        _itemLookup.Add(item.PrefabId, prefab);
                }
#if UNITY_EDITOR
                else
                {
                    Debug.LogWarning($"[PrefabRegistry] Wrong type added to item prefab list: {prefab.name}");
                }
#endif
            }
        }

        public GameObject GetContainerPrefab(string prefabId)
        {
            if (_containerLookup == null) BuildLookups();
            return _containerLookup.TryGetValue(prefabId, out var prefab) ? prefab : null;
        }

        public GameObject GetItemPrefab(string prefabId)
        {
            if (_itemLookup == null) BuildLookups();
            return _itemLookup.TryGetValue(prefabId, out var prefab) ? prefab : null;
        }
    }
}