using System;
using System.Collections.Generic;
using Koala.Simulation.Input;
using UnityEngine.InputSystem;

namespace Koala.Simulation.Interaction.Core
{
    public class InteractionSolver : IDisposable
    {
        private readonly Dictionary<string, InteractionContext> _currentInteractions = new(8);
        private readonly List<InteractableObject> _lastSerie = new();
        private readonly HashSet<string> _heldActions = new();
        private readonly bool _generateInteractionArgs;
        private readonly int _id;
        private float _nextTickTime;
        private float _tickInterval = 0.1f;

        public event Action<IReadOnlyDictionary<string, InteractionContext>> OnNewInteraction;

        public InteractionSolver(bool generateInteractionArgs, int id)
        {
            _generateInteractionArgs = generateInteractionArgs;
            _id = id;

            InputManager.OnAnyActionPhase += OnInputReceive;
            InputManager.Subscribe("MouseLeft", OnMouseButtonPerform, InputActionPhase.Started);
            InputManager.Subscribe("MouseRight", OnMouseButtonPerform, InputActionPhase.Started);
            InputManager.Subscribe("MouseLeft", OnMouseButtonCancel, InputActionPhase.Canceled);
            InputManager.Subscribe("MouseRight", OnMouseButtonCancel, InputActionPhase.Canceled);
        }

        public void Dispose()
        {
            InputManager.OnAnyActionPhase -= OnInputReceive;
            InputManager.Unsubscribe("MouseLeft", OnMouseButtonPerform);
            InputManager.Unsubscribe("MouseRight", OnMouseButtonPerform);
            InputManager.Unsubscribe("MouseLeft", OnMouseButtonCancel);
            InputManager.Unsubscribe("MouseRight", OnMouseButtonCancel);
        }

        public void Solve(List<InteractableObject> interactableObjects)
        {
            _currentInteractions.Clear();

            for (int i = 0; i < interactableObjects.Count; i++)
            {
                var interactable = interactableObjects[i];
                var interactions = _generateInteractionArgs
                    ? interactable.GetInteractions(_id)
                    : interactable.GetInteractions();

                foreach (var interaction in interactions)
                    _currentInteractions[interaction.ActionName] = interaction;
            }

            OnNewInteraction?.Invoke(_currentInteractions);

            for (int i = 0; i < _lastSerie.Count; i++)
            {
                if (!interactableObjects.Contains(_lastSerie[i]))
                    _lastSerie[i].OnInteractionEnded();
            }

            _lastSerie.Clear();
            _lastSerie.AddRange(interactableObjects);
        }

        public void Reset()
        {
            _currentInteractions.Clear();
            foreach (var interactable in _lastSerie)
                interactable.OnInteractionEnded();
            _lastSerie.Clear();
            _heldActions.Clear();
            OnNewInteraction?.Invoke(_currentInteractions);
        }

        private void OnInputReceive(InputAction.CallbackContext ctx)
        {
            if (ctx.action.name == "MouseLeft" || ctx.action.name == "MouseRight")
                return;

            if (_currentInteractions.TryGetValue(ctx.action.name, out var interaction))
            {
                if (ctx.performed)
                    interaction.OnInteract?.Invoke();
            }
        }

        private void OnMouseButtonPerform(InputAction.CallbackContext ctx)
        {
            if (!_heldActions.Contains(ctx.action.name))
                _heldActions.Add(ctx.action.name);
        }

        private void OnMouseButtonCancel(InputAction.CallbackContext ctx)
        {
            if (_heldActions.Contains(ctx.action.name))
                _heldActions.Remove(ctx.action.name);
        }

        public void Tick()
        {
            if (UnityEngine.Time.time < _nextTickTime)
                return;
            _nextTickTime = UnityEngine.Time.time + _tickInterval;

            foreach (var actionName in _heldActions)
            {
                if (_currentInteractions.TryGetValue(actionName, out var interaction))
                    interaction.OnInteract?.Invoke();
            }
        }
    }
}