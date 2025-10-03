using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Koala.Simulation.Inventory
{
    /// <summary>
    /// Represents an item instance that can be stored in a container.
    /// 
    /// Each item has a unique ID, a prefab ID for recreation, and optional transfer animations
    /// for visual movement between container slots.
    /// </summary>
    /// /// <example>
    /// <code>
    /// public void Test()
    /// {
    ///     AddProperty("Durability", 95);
    ///     AddProperty("SkinId", "RedDragon");
    ///
    ///     if (TryGetProperty&lt;int&gt;("Durability", out var d))
    ///         Debug.Log($"Item durability: {d}");
    /// }
    /// </code>
    /// </example>
    [AddComponentMenu("Simulation/Inventory/Item")]
    [Icon("Packages/com.koala.simulation/Editor/Icons/koala.png")]
    public class Item : MonoBehaviour
    {
        private string _uniqueId;
        [SerializeField] private string _prefabId;
        [SerializeReference] private ITransferAnimation _transferBehavior = new InstantTransfer();

        /// <summary>
        /// The globally unique identifier of this item.
        /// </summary>
        public Guid UniqueId => Guid.Parse(_uniqueId);

        /// <summary>
        /// The prefab identifier used to recreate this item.
        /// </summary>
        public string PrefabId => _prefabId;

        /// <summary>
        /// Triggered after the item is fully loaded from a DTO.
        /// </summary>
        public event Action OnLoaded;

        /// <summary>
        /// Properties of the item.
        /// </summary>
        public Dictionary<string, object> Properties { get; private set; } = new();

        /// <summary>
        /// Initializes the item as a new instance and assigns it a unique ID.
        /// </summary>
        public void InitializeNew()
        {
            _uniqueId = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Loads the item state from a DTO.
        /// </summary>
        /// <param name="dto">The DTO containing saved item data.</param>
        public void LoadFromDTO(ItemDTO dto)
        {
            _uniqueId = dto.UniqueId;
            _prefabId = dto.PrefabId;
            Properties = dto.Properties ?? new Dictionary<string, object>();

            OnLoaded?.Invoke();
        }

        /// <summary>
        /// Converts this item to a DTO for saving.
        /// </summary>
        /// <returns>An <see cref="ItemDTO"/> containing the item's state.</returns>
        public ItemDTO ToDTO()
        {
            return new ItemDTO
            {
                UniqueId = _uniqueId,
                PrefabId = _prefabId,
                Properties = this.Properties
            };
        }

        /// <summary>
        /// Performs the configured transfer animation to move the item to a target slot.
        /// </summary>
        /// <param name="targetSlot">The target slot transform.</param>
        public void PerformTransferAnimation(Transform targetSlot)
        {
            _transferBehavior?.Execute(this, targetSlot);
        }

        /// <summary>
        /// Adds a property to the item.
        /// </summary>
        /// <param name="key">The key of the property.</param>
        /// <param name="value">The value of the property.</param>
        public void AddProperty<T>(string key, T value) => Properties[key] = value;
        
        /// <summary>
        /// Tries to get a property from the item.
        /// </summary>
        /// <param name="key">The key of the property.</param>
        /// <param name="value">The value of the property.</param>
        /// <returns>True if the property was found, false otherwise.</returns>
        public bool TryGetProperty<T>(string key, out T value)
        {
            if (Properties.TryGetValue(key, out var obj) && obj is T casted)
            {
                value = casted;
                return true;
            }
            value = default;
            return false;
        }

        /// <summary>
        /// Removes a property from the item.
        /// </summary>
        /// <param name="key">The key of the property.</param>
        public void RemoveProperty(string key) => Properties.Remove(key);

        /// <summary>
        /// Performs a transfer animation with a specific animation behavior.
        /// </summary>
        /// <param name="targetSlot">The target slot transform.</param>
        /// <param name="transferAnimation">The animation behavior to use.</param>
        internal void PerformTransferAnimation(Transform targetSlot, ITransferAnimation transferAnimation)
        {
            transferAnimation?.Execute(this, targetSlot);
        }
    }
}