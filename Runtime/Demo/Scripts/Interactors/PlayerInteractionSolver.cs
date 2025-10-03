using System;
using System.Collections.Generic;
using Koala.Simulation.Interaction.Core;
using Koala.Simulation.Placement;
using UnityEngine;

namespace Koala.Simulation.Demo
{
    public class PlayerInteractionSolver : Interactor
    {
        /// <summary>
        /// The camera used to cast interaction rays. Defaults to <c>Camera.main</c> if not assigned.
        /// </summary>
        [Tooltip("Defaults to Camera.main if empty.")]
        [SerializeField] private Camera m_cam;

        /// <summary>
        /// The maximum range of the interaction ray.
        /// </summary>
        [SerializeField] private float m_range = 3f;

        /// <summary>
        /// The layer mask used to filter raycast hits for interactable objects.
        /// </summary>
        [SerializeField] private LayerMask m_interactionLayerMask;

        /// <summary>
        /// The maximum number of interactable objects to detect per scan.
        /// </summary>
        [SerializeField] private int _maxInteractions = 3;

        /// <summary>
        /// How often to scan for interactions in seconds. 
        /// A value of 0 means every frame.
        /// </summary>
        [Tooltip("How often to scan for interactions (seconds). 0 = every frame. 1 = every second.")]
        [SerializeField] private float m_scanInterval;

        private List<InteractableObject> m_interactableObjects;
        private float m_lastScanTime;
        private RaycastHit[] m_raycastHits = new RaycastHit[16];
        private bool m_isEnabled = true;

        /// <summary>
        /// Ensures the raycaster has a valid camera reference.
        /// </summary>
        protected override void OnAwake()
        {
            if (m_cam == null)
                m_cam = Camera.main;

            m_interactableObjects = new List<InteractableObject>(_maxInteractions + 1);
        }

        private void OnEnable()
        {
            PlacementManager.Instance.OnPlacementStart += DisableInteraction;
            PlacementManager.Instance.OnPlacementEnd += EnableInteraction;
        }

        protected override void OnDisabled()
        {
            PlacementManager.Instance.OnPlacementStart -= DisableInteraction;
            PlacementManager.Instance.OnPlacementEnd -= EnableInteraction;
        }

        protected override void OnUpdate()
        {
            if (!m_isEnabled)
                return;

            m_interactableObjects.Clear();

            if (m_scanInterval <= 0f || Time.time - m_lastScanTime >= m_scanInterval)
            {
                var ray = new Ray(m_cam.transform.position, m_cam.transform.forward);
                int hitCount = Physics.RaycastNonAlloc(ray, m_raycastHits, m_range, m_interactionLayerMask);

                Array.Sort(m_raycastHits, 0, hitCount, Comparer<RaycastHit>.Create((a, b) => a.distance.CompareTo(b.distance)));

                int max = Mathf.Min(hitCount, _maxInteractions);
                for (int i = 0; i < max; i++)
                {
                    var hit = m_raycastHits[i];
                    if (hit.collider.TryGetComponent<InteractableObject>(out var interactable))
                    {
                        m_interactableObjects.Add(interactable);
                    }
                }
            }

            if (Session.Player.Inventory.CurrentHoldable.transform.TryGetComponent(out InteractableObject interactableObj))
            {
                m_interactableObjects.Add(interactableObj);
            }

            if (m_interactableObjects.Count == 0)
                Solver.Reset();
            else
                Solver.Solve(m_interactableObjects);
        }

        private void DisableInteraction()
        {
            m_isEnabled = false;
            m_interactableObjects.Clear();
            Solver.Reset();
        }

        private void EnableInteraction()
        {
            m_isEnabled = true;
        }
    }
}