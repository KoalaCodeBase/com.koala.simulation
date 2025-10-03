using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Koala.Simulation.Inventory
{
    /// <summary>
    /// Represents a container that stores and manages items using defined slots.
    /// 
    /// Containers can be nested, filtered, and serialized to DTOs for saving/loading.
    /// They support adding, removing, and transferring items between containers.
    /// </summary>
    [AddComponentMenu("Simulation/Inventory/Container")]
    [Icon("Packages/com.koala.simulation/Editor/Icons/koala.png")]
    public class Container : MonoBehaviour
    {
        private string m_uniqueId;
        [SerializeField] private string m_prefabId;
        [SerializeField] private ContainerSlot[] m_containerSlots;
        [SerializeReference] private List<IContainerFilter> m_filters;
        [SerializeField] private bool m_fillWithItem;
        [ShowIf(nameof(m_fillWithItem))][SerializeField] private Item m_itemToFill;
        [SerializeField] private bool m_isSceneElement;
        [ShowIf(nameof(m_isSceneElement))][SerializeField] private string m_uniqueSceneName;

        private ContainerSlotStack m_stack;
        private readonly List<Container> m_nestedContainers = new();
        private Vector3 m_savedPosition;
        private Quaternion m_savedRotation;
        private InstantTransfer m_instantTransfer = new InstantTransfer();

        internal bool IsNested { get; private set; } = false;

        /// <summary>
        /// Whether the container is enabled.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Maximum number of items the container can hold.
        /// </summary>
        public int Capacity => m_stack.Capacity;

        /// <summary>
        /// Current number of items stored in the container.
        /// </summary>
        public int Count => m_stack.Count;

        /// <summary>
        /// Whether the container is full.
        /// </summary>
        public bool IsFull => m_stack.IsFull;

        /// <summary>
        /// Whether the container is empty.
        /// </summary>
        public bool IsEmpty => m_stack.IsEmpty;

        /// <summary>
        /// Unique identifier assigned to this container.
        /// </summary>
        public Guid UniqueId => Guid.Parse(m_uniqueId);

        /// <summary>
        /// Prefab identifier used to recreate this container.
        /// </summary>
        public string PrefabId => m_prefabId;

        /// <summary>
        /// Triggered when an item is added to the container.
        /// </summary>
        public event Action<Item> OnItemAdded;

        /// <summary>
        /// Triggered when an item is removed from the container.
        /// </summary>
        public event Action<Item> OnItemRemoved;

        private void Start()
        {
            if (m_isSceneElement)
            {
                Initialize();
                ContainerDTO sceneElementDTO = InventorySaver.LoadBySceneName(m_uniqueSceneName);
                if (sceneElementDTO != null)
                {
                    Register(sceneElementDTO);
                }
                else
                {
                    Register();
                }
            }

            OnStart();
        }

        /// <summary>
        /// Use this like Start.
        /// </summary>
        protected virtual void OnStart() { }

        /// <summary>
        /// Initializes the container so it can be used for item operations.
        /// 
        /// Must be called after instantiating a container before using methods like Add or Remove.
        /// Also initializes all nested containers recursively.
        /// </summary>
        public void Initialize()
        {
            m_stack = new ContainerSlotStack(m_containerSlots);

            Container[] children = GetComponentsInChildren<Container>(includeInactive: true);
            foreach (Container child in children)
            {
                if (child == this) continue;
                if (!m_nestedContainers.Contains(child))
                {
                    m_nestedContainers.Add(child);
                    child.Initialize();
                }
            }
        }

        /// <summary>
        /// Registers a newly created container into the world and assigns it a unique ID.
        /// 
        /// Call this after <see cref="Initialize"/> when creating a container that has never been saved before.
        /// Also registers nested containers and fills slots with default items if set.
        /// 
        /// If this method is not called, the container will not be tracked or saved.
        /// </summary>
        /// <param name="isNested">Whether this container is nested inside another container.</param>
        public void Register(bool isNested = false)
        {
            m_uniqueId = Guid.NewGuid().ToString();
            IsNested = isNested;

            for (int i = 0; i < m_nestedContainers.Count; i++)
            {
                m_nestedContainers[i].Register(true);
            }

            SnapshotTransform();

            if (m_itemToFill != null)
            {
                for (int i = 0; i < m_containerSlots.Length; i++)
                {
                    if (m_containerSlots[i].IsEmpty)
                    {
                        Item itemToFill = InventoryFactory.CreateItem<Item>(m_itemToFill.PrefabId);
                        itemToFill.InitializeNew();
                        AddInstant(itemToFill);
                    }
                }
            }

            if (!IsNested) InventorySaver.Register(this);
        }

        /// <summary>
        /// Restores a container from saved data and registers it into the world.
        /// 
        /// Call this after <see cref="Initialize"/> when loading a container from a save file.
        /// Loads its unique ID, transform, stored items, and nested containers from the provided DTO.
        /// 
        /// If this method is not called, the container will not be tracked or saved.
        /// </summary>
        /// <param name="dto">The saved container data.</param>
        /// <param name="isNested">Whether this container is nested inside another container.</param>
        public void Register(ContainerDTO dto, bool isNested = false)
        {
            m_uniqueId = dto.UniqueId;
            IsNested = isNested;

            if (!IsNested)
            {
                m_savedPosition = dto.SavedPosition;
                m_savedRotation = dto.SavedRotation;
                
                if (TryGetComponent(out Rigidbody rb))
                {
                    rb.MovePosition(m_savedPosition);
                    rb.MoveRotation(m_savedRotation);
                }

                transform.SetPositionAndRotation(m_savedPosition, m_savedRotation);
            }

            for (int i = 0; i < dto.Items.Count; i++)
            {
                ItemDTO itemDto = dto.Items[i];
                Item item = InventoryFactory.CreateItem<Item>(itemDto.PrefabId);
                if (item == null) continue;

                item.LoadFromDTO(itemDto);
                AddInstant(item);
            }

            for (int i = 0; i < dto.Nested.Count; i++)
            {
                ContainerDTO nestedDto = dto.Nested[i];
                Container nested = m_nestedContainers[i];
                nested.Register(nestedDto, true);
            }

            if (!IsNested) InventorySaver.Register(this);
        }

        private void OnDisable()
        {
            InventorySaver.Unregister(this);
        }

        private bool PassesFilters(Item item)
        {
            foreach (var filter in m_filters)
            {
                if (!filter.Allows(item))
                    return false;
            }
            return true;
        }

        private bool AddInstant(Item item)
        {
            if (!PassesFilters(item))
                return false;

            if (m_stack.IsFull)
                return false;

            if (m_stack.TryPush(item, out Transform slotPoint))
            {
                OnItemAdded?.Invoke(item);
                item.PerformTransferAnimation(slotPoint, m_instantTransfer);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Adds an item to the container.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>True if the item was successfully added, false otherwise.</returns>
        public bool Add(Item item, bool performTransferAnimation = true)
        {
            if (!IsEnabled)
                return false;

            if (m_containerSlots.Length == 0)
            {
                Debug.LogError("Container has no slots");
                return false;
            }

            if (!PassesFilters(item))
                return false;

            if (m_stack.IsFull)
                return false;

            if (m_stack.TryPush(item, out Transform slotPoint))
            {
                OnItemAdded?.Invoke(item);
                if (performTransferAnimation)
                    item.PerformTransferAnimation(slotPoint);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Transfers an item from this container to another container.
        /// </summary>
        /// <param name="target">The target container to transfer to.</param>
        /// <returns>True if the transfer succeeded, false otherwise.</returns>
        public bool TransferTo(Container target)
        {
            if (!IsEnabled)
                return false;

            if (m_stack.IsEmpty)
                return false;

            if (!Remove(out Item item))
                return false;

            if (!target.Add(item))
            {
                // rollback
                Add(item, false);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Removes an item from the container.
        /// </summary>
        /// <param name="item">The removed item.</param>
        /// <returns>If true, removed item is returned.</returns>
        public bool Remove(out Item item)
        {
            if (!IsEnabled)
            {
                item = null;
                return false;
            }

            if (m_stack.TryPop(out item))
            {
                OnItemRemoved?.Invoke(item);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Returns the last added item without removing it.
        /// </summary>
        /// <returns>The last added item.</returns>
        public Item Peek()
        {
            if (!IsEnabled)
                return null;

            return m_stack.Peek().Item;
        }

        /// <summary>
        /// Saves the current world position and rotation of the container.
        /// </summary>
        public void SnapshotTransform()
        {
            if (IsNested) return;

            m_savedPosition = transform.position;
            m_savedRotation = transform.rotation;
        }

        /// <summary>
        /// Adds a filter to restrict which items the container accepts.
        /// </summary>
        /// <param name="filter">The filter to add.</param>
        public void AddFilter(IContainerFilter filter)
        {
            if (!m_filters.Contains(filter))
                m_filters.Add(filter);
        }

        /// <summary>
        /// Removes a filter from the container.
        /// </summary>
        /// <param name="filter">The filter to remove.</param>
        public void RemoveFilter(IContainerFilter filter)
        {
            m_filters.Remove(filter);
        }

        /// <summary>
        /// Creates a DTO representation of the container for saving.
        /// </summary>
        /// <returns>A container DTO with current state.</returns>
        public ContainerDTO ToDTO()
        {
            var dto = new ContainerDTO
            {
                UniqueId = m_uniqueId,
                PrefabId = m_prefabId,
                SavedPosition = m_savedPosition,
                SavedRotation = m_savedRotation,
                Items = new List<ItemDTO>(),
                Nested = new List<ContainerDTO>(),
                IsSceneElement = m_isSceneElement,
                UnuqieSceneName = m_uniqueSceneName
            };

            for (int i = 0; i < m_containerSlots.Length; i++)
            {
                if (m_containerSlots[i].Item != null)
                    dto.Items.Add(m_containerSlots[i].Item.ToDTO());
            }

            for (int i = 0; i < m_nestedContainers.Count; i++)
            {
                dto.Nested.Add(m_nestedContainers[i].ToDTO());
            }

            return dto;
        }
    }
}