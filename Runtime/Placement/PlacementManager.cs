using System;
using System.Collections.Generic;
using UnityEngine;

namespace Koala.Simulation.Placement
{
    /// <summary>
    /// Centralized object placement system with snapping, rules, and ghost previews.
    /// 
    /// - Place prefabs into the world with live "ghost" preview.
    /// - Supports validation rules (e.g. overlap checks, layer restrictions).
    /// - Supports snapping strategies (surface alignment, custom logic).
    /// - Handles both initial placement and repositioning of existing objects.
    /// - Exposes utility methods for rotation, confirmation, and cancellation.
    /// 
    /// Singleton: Access via <c>PlacementManager.Instance</c>.
    /// Requires <c>SimulationManager</c> to be present in the scene.
    /// Only one instance of <c>SimulationManager</c> should exist.
    /// </summary>
    /// <example>
    /// <code>
    /// // --- Basic placement ---
    /// // Start placing a shelf prefab. 
    /// // Player confirms with LMB, cancels with RMB, rotates with E/Q.
    /// PlacementManager.Instance.Start(shelfPrefab, onSuccess: placedObj =>
    /// {
    ///     Debug.Log($"Placed: {placedObj.name}");
    /// });
    /// 
    /// // --- Reposition existing object ---
    /// PlacementManager.Instance.Reposition(existingShelf, onSuccess: updatedObj =>
    /// {
    ///     Debug.Log($"Repositioned: {updatedObj.name}");
    /// });
    /// 
    /// // --- Add placement rules ---
    /// PlacementManager.Instance.AddRule(new NoOverlapRule(LayerMask.GetMask("Default")));
    /// PlacementManager.Instance.AddRule(new LayerRule(LayerMask.GetMask("Ground")));
    /// 
    /// // --- Set snapping strategy ---
    /// PlacementManager.Instance.SetSnapStrategy(new DefaultSnapStrategy());
    /// 
    /// // --- Rotate preview manually ---
    /// PlacementManager.Instance.Rotate(clockwise: true);
    /// 
    /// // --- Confirm or cancel manually ---
    /// PlacementManager.Instance.Confirm(); // finalize placement
    /// PlacementManager.Instance.Cancel();  // abort placement
    /// 
    /// // --- Cleanup rules ---
    /// PlacementManager.Instance.ClearRules();
    /// </code>
    /// </example>
    public sealed class PlacementManager : IDisposable
    {
        /// <summary>
        /// The active placement manager instance.
        /// </summary>
        public static PlacementManager Instance => _instance ?? throw new InvalidOperationException("PlacementManager not initialized. Call Initialize first.");
        private static PlacementManager _instance;
        private readonly Camera _cam;
        private readonly float _range;
        private readonly float _rotationStep = 15f;
        private readonly float _smoothSpeed;

        private IPlaceable _placeable;
        private PlacementGhost _ghost;
        private readonly List<IPlacementRule> _rules = new();
        private ISnapStrategy _snapStrategy = new DefaultSnapStrategy();

        private bool _isReposition;
        private GameObject _prefab;
        private bool _lastValid;

        private readonly Material _defaultValidMat;
        private readonly Material _defaultInvalidMat;

        private Quaternion _extraRotation = Quaternion.identity;
        private bool _isPlacing;

        private Action<GameObject> _onSuccess;
        public event Action OnPlacementStart;
        public event Action OnPlacementEnd;

        internal PlacementManager(Camera cam, float range = 10f, float rotationStep = 15f, float smoothSpeed = 0f)
        {
            _cam = cam;
            _range = range;
            _rotationStep = rotationStep;
            _smoothSpeed = smoothSpeed;

            _defaultValidMat = CreateFallbackMaterial(new Color(0f, 1f, 0f, 0.4f));
            _defaultInvalidMat = CreateFallbackMaterial(new Color(1f, 0f, 0f, 0.4f));

            if (_instance != null)
                throw new InvalidOperationException("PlacementManager already initialized.");

            _instance = this;
        }

        private Material CreateFallbackMaterial(Color color)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            var mat = new Material(shader);
            mat.color = color;
            return mat;
        }

        /// <summary>
        /// Adds a placement validation rule. See <see cref="Samples.NoOverlapRule"/> and <see cref="Samples.LayerRule"/>
        /// </summary>
        /// <param name="rule">The rule to add.</param>
        public void AddRule(IPlacementRule rule) => _rules.Add(rule);

        /// <summary>
        /// Sets the snapping strategy for placements. See <see cref="DefaultSnapStrategy"/>
        /// </summary>
        /// <param name="strategy">The snapping strategy to use.</param>
        public void SetSnapStrategy(ISnapStrategy strategy) => _snapStrategy = strategy;

        /// <summary>
        /// Starts placing a prefab with a ghost preview. Do not use for repositioning.
        /// Prefab must implement <see cref="IPlaceable"/>.
        /// </summary>
        /// <param name="prefab">The prefab to place.</param>
        /// <param name="validMat">Optional material for valid placement preview.</param>
        /// <param name="invalidMat">Optional material for invalid placement preview.</param>
        /// <param name="onSuccess">Callback when placement is confirmed.</param>
        public void Start(GameObject prefab, Material validMat = null, Material invalidMat = null, Action<GameObject> onSuccess = null)
        {
            if (prefab == null || _isPlacing) return;

            _prefab = prefab;
            _placeable = prefab.GetComponent<IPlaceable>();

            if (_placeable == null)
            {
                Debug.LogError("Prefab does not implement IPlaceable.");
                return;
            }

            if (_ghost == null)
                _ghost = new PlacementGhost(prefab, validMat ?? _defaultValidMat, invalidMat ?? _defaultInvalidMat, true);
            else
                _ghost.Start(prefab, validMat ?? _defaultValidMat, invalidMat ?? _defaultInvalidMat, true);

            _isReposition = false;
            _extraRotation = Quaternion.identity;
            _isPlacing = true;
            _onSuccess = onSuccess;
            OnPlacementStart?.Invoke();
        }

        /// <summary>
        /// Starts repositioning an existing object with a ghost preview.
        /// </summary>
        /// <param name="target">The object to reposition.</param>
        /// <param name="validMat">Optional material for valid placement preview.</param>
        /// <param name="invalidMat">Optional material for invalid placement preview.</param>
        /// <param name="onSuccess">Callback when repositioning is confirmed.</param>
        public void Reposition(GameObject target, Material validMat = null, Material invalidMat = null, Action<GameObject> onSuccess = null)
        {
            if (target == null || _isPlacing) return;

            _placeable = target.GetComponent<IPlaceable>();
            if (_placeable == null)
            {
                Debug.LogError("Target does not implement IPlaceable.");
                return;
            }

            if (_ghost == null)
                _ghost = new PlacementGhost(target, validMat ?? _defaultValidMat, invalidMat ?? _defaultInvalidMat, false);
            else
                _ghost.Start(target, validMat ?? _defaultValidMat, invalidMat ?? _defaultInvalidMat, false);

            _isReposition = true;
            _extraRotation = Quaternion.identity;
            _isPlacing = true;
            _onSuccess = onSuccess;
        }

        /// <summary>
        /// Rotates the placement preview.
        /// </summary>
        /// <param name="clockwise">Whether to rotate clockwise (default true).</param>
        public void Rotate(bool clockwise = true)
        {
            if (!_isPlacing || _ghost == null || _placeable == null) return;

            float step = clockwise ? _rotationStep : -_rotationStep;
            _extraRotation = Quaternion.AngleAxis(step, _placeable.DefaultRotationAxis) * _extraRotation;
        }

        internal void Update()
        {
            if (!_isPlacing || _ghost == null || _placeable == null) return;

            Ray ray = new Ray(_cam.transform.position, _cam.transform.forward);
            Vector3 targetPos;
            Quaternion targetRot;

            if (Physics.Raycast(ray, out RaycastHit hit, _range))
            {
                _lastValid = true;
                for (int i = 0; i < _rules.Count; i++)
                {
                    if (!_rules[i].Validate(hit, _ghost.Placeable))
                    {
                        _lastValid = false;
                        break;
                    }
                }

                if (_lastValid)
                    (targetPos, targetRot) = _snapStrategy.GetSnappedTransform(hit, _placeable);
                else
                {
                    targetPos = hit.point;
                    targetRot = Quaternion.LookRotation(Vector3.forward);
                }
            }
            else
            {
                targetPos = ray.origin + ray.direction * _range;
                targetRot = Quaternion.LookRotation(Vector3.forward);
                _lastValid = false;
            }

            targetRot *= _extraRotation;

            if (_smoothSpeed > 0f)
            {
                var currentPos = _ghost.Transform.position;
                var currentRot = _ghost.Transform.rotation;

                targetPos = Vector3.Lerp(currentPos, targetPos, Time.deltaTime * _smoothSpeed);
                targetRot = Quaternion.Slerp(currentRot, targetRot, Time.deltaTime * _smoothSpeed);
            }

            _ghost.UpdateTransform(targetPos, targetRot, _lastValid);
        }

        /// <summary>
        /// Confirms the current placement or reposition. If it is invalid, it will not be placed.
        /// </summary>
        public void Confirm()
        {
            if (!_isPlacing || _ghost == null) return;

            if (!_lastValid) return;

            GameObject final = null;

            if (_isReposition)
            {
                _ghost.Confirm();
                final = _ghost.GameObject;
            }
            else
            {
                final = UnityEngine.Object.Instantiate(_prefab);
                final.transform.SetPositionAndRotation(
                    _ghost.Transform.position,
                    _ghost.Transform.rotation
                );
                _ghost.Confirm();
            }

            _onSuccess?.Invoke(final);
            ResetState();
            OnPlacementEnd?.Invoke();
        }

        /// <summary>
        /// Cancels the current placement or reposition.
        /// </summary>
        public void Cancel()
        {
            if (!_isPlacing || _ghost == null) return;

            _ghost.Cancel();
            ResetState();
            OnPlacementEnd?.Invoke();
        }

        private void ResetState()
        {
            _placeable = null;
            _isReposition = false;
            _extraRotation = Quaternion.identity;
            _isPlacing = false;
            _onSuccess = null;
            _lastValid = false;
        }

        /// <summary>
        /// Removes all active placement rules.
        /// </summary>
        public void ClearRules() => _rules.Clear();

        /// <summary>
        /// For internal use. Calling this incorrectly may lead to unexpected behavior.
        /// </summary>
        public void Dispose()
        {
            _instance = null;
        }
    }
}