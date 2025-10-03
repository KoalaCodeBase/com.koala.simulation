using System;
using System.Collections.Generic;
using Koala.Simulation.Interaction.Core;
using UnityEngine;

namespace Koala.Simulation.Interaction.Components
{
    /// <summary>
    /// A sample component showing how to use a <see cref="Koala.Simulation.Inventory.Core.Container"/> in the scene.
    /// 
    /// Demonstrates creating a container, applying filters,
    /// saving and loading container state, and interacting with items at runtime.
    /// </summary>
    /// <example>
    /// <code>
    /// public sealed class InteractionRaycaster : Interactor
    /// {
    ///     [SerializeField] private Camera _cam;
    ///     [SerializeField] private float _range = 3f;
    ///     [SerializeField] private LayerMask _interactionLayerMask;
    ///     [SerializeField] private int _maxInteractions = 3;
    ///     [SerializeField] private float _scanInterval;
    ///
    ///     private readonly List&lt;InteractableObject&gt; _interactableObjects = new();
    ///     private float _lastScanTime;
    ///
    ///     protected override void OnAwake()
    ///     {
    ///         if (_cam == null)
    ///             _cam = Camera.main;
    ///     }
    ///
    ///     private void Update()
    ///     {
    ///         if (_scanInterval &gt; 0f &amp;&amp;
    ///             Time.time - _lastScanTime &lt; _scanInterval)
    ///             return;
    ///
    ///         _interactableObjects.Clear();
    ///
    ///         var ray = new Ray(
    ///             _cam.transform.position,
    ///             _cam.transform.forward
    ///         );
    ///
    ///         var hits = Physics.RaycastAll(
    ///             ray,
    ///             _range,
    ///             _interactionLayerMask
    ///         );
    ///
    ///         Array.Sort(
    ///             hits,
    ///             (a, b) =&gt; a.distance.CompareTo(b.distance)
    ///         );
    ///
    ///         int max = Mathf.Min(hits.Length, _maxInteractions);
    ///         for (int i = 0; i &lt; max; i++)
    ///         {
    ///             var hit = hits[i];
    ///             if (hit.collider.TryGetComponent&lt;InteractableObject&gt;(
    ///                 out var interactable))
    ///             {
    ///                 _interactableObjects.Add(interactable);
    ///             }
    ///         }
    ///
    ///         if (_interactableObjects.Count == 0)
    ///             Solver.Reset();
    ///         else
    ///             Solver.Solve(_interactableObjects);
    ///     }
    /// }
    /// </code>
    /// </example>

    [AddComponentMenu("Simulation/Interaction/Interaction Raycaster")]
    [Icon("Packages/com.koala.simulation/Editor/Icons/koala.png")]
    public sealed class InteractionRaycaster : Interactor
    {
        /// <summary>
        /// The camera used to cast interaction rays. Defaults to <c>Camera.main</c> if not assigned.
        /// </summary>
        [Tooltip("Defaults to Camera.main if empty.")]
        [SerializeField] private Camera _cam;

        /// <summary>
        /// The maximum range of the interaction ray.
        /// </summary>
        [SerializeField] private float _range = 3f;

        /// <summary>
        /// The layer mask used to filter raycast hits for interactable objects.
        /// </summary>
        [SerializeField] private LayerMask _interactionLayerMask;

        /// <summary>
        /// The maximum number of interactable objects to detect per scan.
        /// </summary>
        [SerializeField] private int _maxInteractions = 3;

        /// <summary>
        /// How often to scan for interactions in seconds. 
        /// A value of 0 means every frame.
        /// </summary>
        [Tooltip("How often to scan for interactions (seconds). 0 = every frame. 1 = every second.")]
        [SerializeField] private float _scanInterval;

        private readonly List<InteractableObject> _interactableObjects = new();
        private float _lastScanTime;

        /// <summary>
        /// Ensures the raycaster has a valid camera reference.
        /// </summary>
        protected override void OnAwake()
        {
            if (_cam == null)
                _cam = Camera.main;
        }

        private void Update()
        {
            if (_scanInterval > 0f && Time.time - _lastScanTime < _scanInterval)
                return;

            _interactableObjects.Clear();

            var ray = new Ray(_cam.transform.position, _cam.transform.forward);
            var hits = Physics.RaycastAll(ray, _range, _interactionLayerMask);

            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            int max = Mathf.Min(hits.Length, _maxInteractions);
            for (int i = 0; i < max; i++)
            {
                var hit = hits[i];
                if (hit.collider.TryGetComponent<InteractableObject>(out var interactable))
                {
                    _interactableObjects.Add(interactable);
                }
            }

            if (_interactableObjects.Count == 0)
                Solver.Reset();
            else
                Solver.Solve(_interactableObjects);
        }
    }
}