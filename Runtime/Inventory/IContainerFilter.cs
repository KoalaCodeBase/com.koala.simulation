using Koala.Simulation.Common;

namespace Koala.Simulation.Inventory
{
    /// <summary>
    /// Defines a filter that determines whether an item is allowed in a container.
    /// </summary>
    public interface IContainerFilter
    {
        /// <summary>
        /// Checks if the given item passes the filter.
        /// </summary>
        /// <param name="item">The item to evaluate.</param>
        /// <returns>True if the item is allowed, false otherwise.</returns>
        bool Allows(Item item);
    }

    [DocfxIgnore]
    [System.Serializable]
    public class ItemPrefabFilter : IContainerFilter
    {
        /// <summary>
        /// The prefab ID that items must match to be allowed.
        /// </summary>
        public string ItemPrefabId;

        /// <summary>
        /// Checks if the item's prefab ID matches the filter's ID.
        /// </summary>
        /// <param name="item">The item to evaluate.</param>
        /// <returns>True if the item matches the prefab ID, false otherwise.</returns>
        public bool Allows(Item item)
        {
            return item.PrefabId == ItemPrefabId;
        }
    }
}