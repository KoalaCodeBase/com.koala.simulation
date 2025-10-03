using Koala.Simulation.Inventory;
using Koala.Simulation.Placement;
using UnityEngine;

namespace Koala.Simulation.Demo
{
    public class PlaceableContainer : Container, IPlaceable
    {
        private Collider m_collider;

        public Vector3 DefaultRotationAxis => Vector3.up;

        public Vector3 PlacementOffset => Vector3.zero;

        public Collider PlacementCollider => m_collider;

        private void Awake()
        {
            m_collider = GetComponent<Collider>();
        }
    }
}