using System;
using System.Collections.Generic;

/// <summary>
/// Data transfer object that represents a serialized item state for saving and loading.
/// 
/// Stores the unique identifier and prefab ID of an item.
/// <code>
/// public string UniqueId;
/// public string PrefabId;
/// </code>
/// </summary>
[Serializable]
public class ItemDTO
{
    /// <summary>
    /// Unique identifier of the item instance.
    /// </summary>
    public string UniqueId;

    /// <summary>
    /// Prefab identifier used to recreate the item.
    /// </summary>
    public string PrefabId;

    /// <summary>
    /// Properties of the item.
    /// </summary>
    public Dictionary<string, object> Properties = new();
}