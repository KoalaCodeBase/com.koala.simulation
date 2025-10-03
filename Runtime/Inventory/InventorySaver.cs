using System.Collections.Generic;
using UnityEngine;

namespace Koala.Simulation.Inventory
{
    /// <summary>
    /// Provides saving and loading functionality for all registered containers.
    /// 
    /// Tracks containers, serializes them to DTOs, and persists data using ES3.
    /// Only top-level containers are saved; nested ones are handled through their parents.
    /// </summary>
    public static class InventorySaver
    {
        private const string SaveFile = "world.es3";
        private const string ContainersKey = "Containers";
        private static readonly HashSet<Container> _containers = new();
        private static readonly List<ContainerDTO> _sceneElements = new();
        private static bool _sceneElementsLoaded = false;

        /// <summary>
        /// Registers a container so it can be saved.
        /// </summary>
        /// <param name="container">The container to register.</param>
        public static void Register(Container container) => _containers.Add(container);

        /// <summary>
        /// Unregisters a container so it will no longer be saved.
        /// </summary>
        /// <param name="container">The container to unregister.</param>
        public static void Unregister(Container container) => _containers.Remove(container);

        /// <summary>
        /// Saves all registered top-level containers to persistent storage.
        /// </summary>
        public static void SaveAll()
        {
            var dtos = new List<ContainerDTO>();

            foreach (var c in _containers)
            {
                if (c.IsNested) continue;
                dtos.Add(c.ToDTO());
            }

            ES3.Save(ContainersKey, dtos, SaveFile);
        }

        /// <summary>
        /// Loads all containers previously saved to persistent storage.
        /// </summary>
        /// <returns>A list of container DTOs, or an empty list if none exist.</returns>
        public static List<ContainerDTO> LoadAll()
        {
            if (!ES3.KeyExists(ContainersKey, SaveFile))
            {
                _sceneElementsLoaded = false;
                _sceneElements.Clear();
                return new List<ContainerDTO>();
            }

            var dtos = ES3.Load<List<ContainerDTO>>(ContainersKey, SaveFile);

            if (!_sceneElementsLoaded)
            {
                _sceneElements.Clear();
                foreach (var dto in dtos)
                {
                    if (dto.IsSceneElement)
                        _sceneElements.Add(dto);
                }
                _sceneElementsLoaded = true;
            }

            return dtos;
        }

        /// <summary>
        /// Removes a container with the given ID from the save file.
        /// </summary>
        /// <param name="containerId">The unique ID of the container to remove.</param>
        public static void RemoveFromSave(string containerId)
        {
            if (!ES3.KeyExists(ContainersKey, SaveFile))
                return;

            var dtos = ES3.Load<List<ContainerDTO>>(ContainersKey, SaveFile);
            dtos.RemoveAll(c => c.UniqueId == containerId);

            ES3.Save(ContainersKey, dtos, SaveFile);
        }

        /// <summary>
        /// Removes the given container from the save file.
        /// </summary>
        /// <param name="container">The container to remove.</param>
        public static void RemoveFromSave(Container container)
        {
            RemoveFromSave(container.UniqueId.ToString());
        }

        /// <summary>
        /// Gets the first scene element DTO with the given prefab id.
        /// </summary>
        public static ContainerDTO LoadBySceneName(string uniqueSceneName)
        {
            return _sceneElements.Find(c => c.UnuqieSceneName == uniqueSceneName);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Dispose()
        {
            _containers.Clear();
            _sceneElements.Clear();
            _sceneElementsLoaded = false;
        }
    }
}