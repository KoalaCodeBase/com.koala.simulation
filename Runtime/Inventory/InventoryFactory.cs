using System;
using Koala.Simulation.Common;
using UnityEngine;

namespace Koala.Simulation.Inventory
{
    /// <summary>
    /// Factory class for creating container and item instances from registered prefabs.
    /// 
    /// Uses a <see cref="PrefabRegistry"/> to look up prefabs by ID and instantiate them at runtime.
    /// </summary>
    public class InventoryFactory : IDisposable
    {
        private static PrefabRegistry s_registry;

        internal InventoryFactory(PrefabRegistry registry)
        {
            s_registry = registry;
        }

        /// <summary>
        /// Creates a new container instance from the registered prefab with the given ID.
        /// </summary>
        /// <typeparam name="T">The container type to create.</typeparam>
        /// <param name="prefabId">The ID of the container prefab.</param>
        /// <returns>The created container, or null if not found.</returns>
        public static T Create<T>(string prefabId) where T : Container
        {
            var prefab = s_registry.GetContainerPrefab(prefabId);

            if (prefab == null)
            {
                Debug.LogError($"Prefab {prefabId} not found");
                return null;
            }

            GameObject containerObj = GameObject.Instantiate(prefab);
            T component = containerObj.GetComponent<T>();

            return component;
        }

        /// <summary>
        /// Creates a new item instance from the registered prefab with the given ID.
        /// </summary>
        /// <typeparam name="T">The item type to create.</typeparam>
        /// <param name="prefabId">The ID of the item prefab.</param>
        /// <returns>The created item, or null if not found.</returns>
        public static T CreateItem<T>(string prefabId) where T : Item
        {
            var prefab = s_registry.GetItemPrefab(prefabId);

            if (prefab == null)
            {
                Debug.LogError($"Prefab {prefabId} not found");
                return null;
            }

            GameObject itemObj = GameObject.Instantiate(prefab);
            T component = itemObj.GetComponent<T>();

            component.InitializeNew();

            return component;
        }

        [DocfxIgnore]
        public void Dispose()
        {
            s_registry = null;
        }
    }
}
