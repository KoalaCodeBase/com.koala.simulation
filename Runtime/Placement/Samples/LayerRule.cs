using UnityEngine;

namespace Koala.Simulation.Placement.Samples
{
    public sealed class LayerRule : IPlacementRule
    {
        private readonly LayerMask _allowedLayers;

        public LayerRule(LayerMask allowedLayers)
        {
            _allowedLayers = allowedLayers;
        }

        public bool Validate(RaycastHit hit, IPlaceable placeable)
        {
            return ((_allowedLayers.value >> hit.collider.gameObject.layer) & 1) == 1;
        }
    }
}