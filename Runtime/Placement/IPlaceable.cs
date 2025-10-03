using UnityEngine;

namespace Koala.Simulation.Placement
{
    /// <summary>
    /// Represents an object that can be placed in the simulation.
    /// </summary>
    /// <example>
    /// <xref uid="Koala.Simulation.Placement.Samples.Sample_Placeable_Object" altProperty="fullName" displayProperty="name"/> and <xref uid="Koala.Simulation.Placement.Samples.PlacementSample" altProperty="fullName" displayProperty="name"/>
    /// </example>
    public interface IPlaceable
    {
        /// <summary>
        /// The default axis used to rotate the object.
        /// </summary>
        Vector3 DefaultRotationAxis { get; }

        /// <summary>
        /// The positional offset applied when snapping the object to a surface.
        /// </summary>
        Vector3 PlacementOffset { get; }

        /// <summary>
        /// The collider that defines the object's placement boundaries.
        /// </summary>
        Collider PlacementCollider { get; }
    }
}