using UnityEngine;
using System.Collections.Generic;
using Koala.Simulation.Common;

namespace Koala.Simulation.Placement
{
    [DocfxIgnore]
    public sealed class PlacementGhost
    {
        private GameObject _ghost;
        private readonly List<Renderer> _renderers = new List<Renderer>();
        private readonly List<Collider> _colliders = new List<Collider>();
        private readonly List<Rigidbody> _rigidbodies = new List<Rigidbody>();
        private readonly List<Material[]> _originalMats = new List<Material[]>();
        private readonly List<bool> _originalKinematics = new List<bool>();

        private bool _isClone;
        private Vector3 _originalPosition;
        private Quaternion _originalRotation;
        private static readonly Material[] _validArray = new Material[1];
        private static readonly Material[] _invalidArray = new Material[1];

        internal IPlaceable Placeable { get; private set; }

        internal PlacementGhost(GameObject target, Material validMat, Material invalidMat, bool clone = true)
        {
            Start(target, validMat, invalidMat, clone);
        }

        private void CacheMaterialArrays(Material validMat, Material invalidMat)
        {
            _validArray[0] = validMat;
            _invalidArray[0] = invalidMat;
        }

        private void EnterGhostMode()
        {
            if (_renderers.Count == 0) return;

            foreach (var r in _renderers)
                r.sharedMaterials = _invalidArray;

            foreach (var c in _colliders)
                c.enabled = false;

            foreach (var r in _rigidbodies)
                r.isKinematic = true;
        }

        internal void Start(GameObject target, Material validMat, Material invalidMat, bool clone)
        {
            CacheMaterialArrays(validMat, invalidMat);
            _isClone = clone;

            if (_ghost != null && _isClone)
            {
                _ghost = null;
            }

            if (clone)
            {
                _ghost = Object.Instantiate(target);
                Placeable = _ghost.GetComponent<IPlaceable>();
            }
            else
            {
                _ghost = target;
                Placeable = _ghost.GetComponent<IPlaceable>();
                _originalPosition = target.transform.position;
                _originalRotation = target.transform.rotation;
            }

            _renderers.Clear();
            _colliders.Clear();
            _rigidbodies.Clear();
            _originalMats.Clear();
            _originalKinematics.Clear();

            _ghost.GetComponentsInChildren(_renderers);
            _ghost.GetComponentsInChildren(_colliders);
            _ghost.GetComponentsInChildren(_rigidbodies);

            for (int i = 0; i < _rigidbodies.Count; i++)
                _originalKinematics.Add(_rigidbodies[i].isKinematic);

            for (int i = 0; i < _renderers.Count; i++)
                _originalMats.Add(_renderers[i].sharedMaterials);

            EnterGhostMode();
        }

        internal void UpdateTransform(Vector3 position, Quaternion rotation, bool valid)
        {
            if (_ghost == null) return;

            _ghost.transform.SetPositionAndRotation(position, rotation);

            var mats = valid ? _validArray : _invalidArray;
            foreach (var r in _renderers)
                r.sharedMaterials = mats;
        }

        internal void Rotate(Vector3 axis, float step)
        {
            if (_ghost == null) return;
            _ghost.transform.rotation = Quaternion.AngleAxis(step, axis) * _ghost.transform.rotation;
        }

        internal void Confirm()
        {
            if (_ghost == null) return;

            ExitGhostMode();

            if (_isClone)
                Object.Destroy(_ghost);
        }

        internal void Cancel()
        {
            if (_ghost == null) return;

            if (!_isClone)
                _ghost.transform.SetPositionAndRotation(_originalPosition, _originalRotation);

            ExitGhostMode();

            if (_isClone)
                Object.Destroy(_ghost);
        }

        private void ExitGhostMode()
        {
            if (_renderers.Count == 0) return;

            for (int i = 0; i < _renderers.Count; i++)
                _renderers[i].sharedMaterials = _originalMats[i];

            foreach (var c in _colliders)
                c.enabled = true;

            for (int i = 0; i < _rigidbodies.Count; i++)
                _rigidbodies[i].isKinematic = _originalKinematics[i];
        }

        internal Transform Transform => _ghost?.transform;
        internal GameObject GameObject => _ghost;
    }
}