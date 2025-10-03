using System.Collections.Generic;
using UnityEngine;

namespace Koala.Simulation.Inventory
{
    /// <summary>
    /// Data transfer object that represents a serialized container state for saving and loading.
    /// 
    /// Stores identifiers, transform data, contained items, and nested containers.
    /// Used when persisting or restoring containers from save files.
    /// <code>
    /// public string UniqueId;
    /// public string PrefabId;
    /// public Vector3 SavedPosition;
    /// public Quaternion SavedRotation;
    /// public List&lt;ItemDTO&gt; Items;
    /// public List&lt;ContainerDTO&gt; Nested;
    /// </code>
    /// </summary>
    [System.Serializable]
    public class ContainerDTO
    {
        /// <summary>
        /// Unique identifier of the container instance.
        /// </summary>
        public string UniqueId;

        /// <summary>
        /// Prefab identifier used to recreate the container.
        /// </summary>
        public string PrefabId;

        /// <summary>
        /// World position saved when the container was registered.
        /// </summary>
        public Vector3 SavedPosition;

        /// <summary>
        /// World rotation saved when the container was registered.
        /// </summary>
        public Quaternion SavedRotation;

        /// <summary>
        /// List of serialized items currently inside the container.
        /// </summary>
        public List<ItemDTO> Items;

        /// <summary>
        /// List of serialized nested containers inside this container.
        /// </summary>
        public List<ContainerDTO> Nested;

        /// <summary>
        /// Set true for containers that are always present in the scene.
        /// </summary>
        public bool IsSceneElement;

        /// <summary>
        /// Unique name of the container in the scene.
        /// </summary>
        public string UnuqieSceneName;
    }
}