using UnityEngine;
namespace Koala.Simulation.Placement.Samples
{
    /// <example>
    /// <code>
    /// using UnityEngine;
    ///
    /// namespace Koala.Simulation.Placement.Samples
    /// {
    ///     public sealed class Sample_Placeable_Object : MonoBehaviour, IPlaceable
    ///     {
    ///         private Collider _collider;
    ///
    ///         public Vector3 DefaultRotationAxis =&gt; Vector3.up;
    ///
    ///         public Vector3 PlacementOffset =&gt; Vector3.zero;
    ///
    ///         public Collider PlacementCollider =&gt; _collider;
    ///
    ///         private void Awake()
    ///         {
    ///             _collider = GetComponent&lt;BoxCollider&gt;();
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    public sealed class Sample_Placeable_Object : MonoBehaviour, IPlaceable
    {
        private Collider _collider;
        public Vector3 DefaultRotationAxis => Vector3.up;
        public Vector3 PlacementOffset => Vector3.zero;
        public Collider PlacementCollider => _collider;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider>();
        }
    }
}