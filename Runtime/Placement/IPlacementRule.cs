using UnityEngine;

namespace Koala.Simulation.Placement
{
    /// <summary>
    /// Defines a rule that determines if a placeable object can be placed at a given surface.
    /// </summary>
    /// <example>
    /// <xref uid="Koala.Simulation.Placement.Samples.NoOverlapRule" altProperty="fullName" displayProperty="name"/>,
    /// <xref uid="Koala.Simulation.Placement.Samples.LayerRule" altProperty="fullName" displayProperty="name"/>
    /// </example>
    public interface IPlacementRule
    {
        /// <summary>
        /// Validates whether a placeable object can be placed at the raycast hit location.
        /// </summary>
        /// <param name="hit">The raycast hit providing surface data.</param>
        /// <param name="placeable">The object being placed.</param>
        /// <returns>True if placement is valid, false otherwise.</returns>
        bool Validate(RaycastHit hit, IPlaceable placeable);
    }
}