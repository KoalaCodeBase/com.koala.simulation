using UnityEngine;

namespace Koala.Simulation.Placement
{
    /// <summary>
    /// Defines a strategy for snapping a placeable object to a surface.
    /// </summary>
    /// <example>
    /// <xref uid="Koala.Simulation.Placement.DefaultSnapStrategy" altProperty="fullName" displayProperty="name"/>
    /// </example>
    public interface ISnapStrategy
    {
        /// <summary>
        /// Gets the snapped position and rotation for a placeable object based on a raycast hit.
        /// </summary>
        /// <param name="hit">The raycast hit providing surface data.</param>
        /// <param name="placeable">The object being placed.</param>
        /// <returns>A tuple containing the snapped position and rotation.</returns>
        (Vector3 position, Quaternion rotation) GetSnappedTransform(RaycastHit hit, IPlaceable placeable);
    }

    /// <summary>
    /// Default snapping strategy that aligns a placeable object with the hit surface normal.
    /// </summary>
    /// <example>
    /// <code>
    /// public sealed class DefaultSnapStrategy : ISnapStrategy
    /// {
    ///     public (Vector3, Quaternion) GetSnappedTransform(RaycastHit hit, IPlaceable placeable)
    ///     {
    ///         Vector3 pos = hit.point + placeable.PlacementOffset;
    ///         Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
    ///         return (pos, rot);
    ///     }
    /// }
    /// </code>
    /// </example>
    public sealed class DefaultSnapStrategy : ISnapStrategy
    {
        /// <summary>
        /// Gets the snapped position and rotation by offsetting the hit point and aligning with the surface normal.
        /// </summary>
        /// <param name="hit">The raycast hit providing surface data.</param>
        /// <param name="placeable">The object being placed.</param>
        /// <returns>A tuple containing the snapped position and rotation.</returns>
        public (Vector3, Quaternion) GetSnappedTransform(RaycastHit hit, IPlaceable placeable)
        {
            Vector3 pos = hit.point + placeable.PlacementOffset;
            Quaternion rot = Quaternion.FromToRotation(Vector3.up, hit.normal);
            return (pos, rot);
        }
    }
}