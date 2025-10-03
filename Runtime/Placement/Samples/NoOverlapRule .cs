using UnityEngine;

namespace Koala.Simulation.Placement.Samples
{
    public sealed class NoOverlapRule : IPlacementRule
    {
        private readonly LayerMask _ignoreMask;
        private static readonly Collider[] _overlapBuffer = new Collider[32];

        public NoOverlapRule(LayerMask ignoreMask)
        {
            _ignoreMask = ignoreMask;
        }

        public bool Validate(RaycastHit hit, IPlaceable placeable)
        {
            if (placeable?.PlacementCollider == null)
                return true;

            if (placeable.PlacementCollider is not BoxCollider box)
            {
                return true;
            }

            bool hasOverlap = HasOverlap(box, _ignoreMask);
            if (hasOverlap)
            {
                return false;
            }

            return true;
        }

        private bool HasOverlap(BoxCollider box, LayerMask ignoreMask)
        {
            if (box == null) return false;

            Vector3 worldCenter = box.transform.TransformPoint(box.center);
            Vector3 worldHalfExtents = Vector3.Scale(box.size * 0.5f, box.transform.lossyScale);
            Quaternion worldRot = box.transform.rotation;

            int blockMask = ~ignoreMask.value;

            int count = Physics.OverlapBoxNonAlloc(
                worldCenter,
                worldHalfExtents,
                _overlapBuffer,
                worldRot,
                blockMask,
                QueryTriggerInteraction.Ignore
            );

            for (int i = 0; i < count; i++)
            {
                var hit = _overlapBuffer[i];
                if (hit != box)
                    return true;
            }

            return false;
        }

    }
}