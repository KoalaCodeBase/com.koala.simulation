using Koala.Simulation.Common;
using UnityEngine;

namespace Koala.Simulation.Placement.Samples
{
    [DocfxIgnore]
    public sealed class SurfaceNormalRule : IPlacementRule
    {
        private readonly Vector3 _allowedNormal;
        private readonly float _tolerance;

        public SurfaceNormalRule(Vector3 allowedNormal, float tolerance = 0.1f)
        {
            _allowedNormal = allowedNormal.normalized;
            _tolerance = tolerance;
        }

        public bool Validate(RaycastHit hit, IPlaceable placeable)
        {
            float dot = Vector3.Dot(hit.normal.normalized, _allowedNormal);
            return dot >= 1f - _tolerance;
        }
    }
}