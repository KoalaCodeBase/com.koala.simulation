using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using Koala.Simulation.Common;

namespace Koala.Simulation.Interaction.Core
{
    /// <summary>
    /// Base class for all interactable objects in the simulation.
    /// 
    /// Provides a registry ID, manages attached interaction behaviours,
    /// and triggers interaction events when interacted with.
    /// 
    /// Inherit this class to create custom interactable objects by overriding
    /// lifecycle hooks such as <see cref="OnAwake"/>, <see cref="OnEnabled"/>, and <see cref="OnDisabled"/>.
    /// <note type="tip">
    /// Inherit this class and define its <see cref="InteractionBehaviour"/>s
    /// to start to interact with the object.
    /// 
    /// See: <xref uid="Koala.Simulation.Interaction.Components.Interactable" altProperty="fullName" displayProperty="name"/>
    /// </note>
    /// </summary>
    public abstract class InteractableObject : MonoBehaviour
    {
        [SerializeField] private List<InteractionBehaviour> _interactionBehaviours = new();
        [SerializeField] private UnityEvent OnInteract;
        [SerializeField] private UnityEvent OnInteractEnd;

        private readonly List<InteractionContext> _contextCache = new(12);
        private int _id;

        [DocfxIgnore]
        protected int Id => _id;


        /// <summary>
        /// Event invoked when one of the <see cref="InteractionBehaviour"/> triggers an interaction.
        /// </summary>
        public event Action OnBehaviourInteraction;

        private void Awake()
        {
            _id = InteractableRegistry.Register(this);
            OnAwake();
        }

        private void OnEnable()
        {
            foreach (var behaviour in _interactionBehaviours)
                behaviour.OnInteractionPerform += OnInteractionPerformed;

            OnEnabled();
        }

        private void OnDisable()
        {
            foreach (var behaviour in _interactionBehaviours)
                behaviour.OnInteractionPerform -= OnInteractionPerformed;

            OnDisabled();
        }

        private void OnInteractionPerformed()
        {
            OnBehaviourInteraction?.Invoke();
        }

        internal void OnInteractionEnded()
        {
            OnInteractEnd?.Invoke();
        }

        [DocfxIgnore]
        internal IReadOnlyList<InteractionContext> GetInteractions()
        {
            _contextCache.Clear();
            OnInteract?.Invoke();

            for (int i = 0; i < _interactionBehaviours.Count; i++)
            {
                var behaviour = _interactionBehaviours[i];
                if (behaviour.IsInteractable())
                {
                    _contextCache.Add(new InteractionContext(
                        behaviour.actionMapName,
                        behaviour.prompt,
                        behaviour.Interact
                    ));
                }
            }

            return _contextCache;
        }

        [DocfxIgnore]
        internal IReadOnlyList<InteractionContext> GetInteractions(int interactorId)
        {
            _contextCache.Clear();
            OnInteract?.Invoke();

            for (int i = 0; i < _interactionBehaviours.Count; i++)
            {
                var behaviour = _interactionBehaviours[i];
                if (behaviour.IsInteractable())
                {
                    _contextCache.Add(new InteractionContext(
                        behaviour.actionMapName,
                        behaviour.prompt,
                        behaviour.Interact,
                        new InteractionArgs(interactorId, _id)
                    ));
                }
            }

            return _contextCache; // yine aynı cache
        }

        /// <summary>
        /// Called during Awake. Override to implement custom initialization logic.
        /// </summary>
        protected virtual void OnAwake() { }

        /// <summary>
        /// Called during OnEnable. Override to implement custom logic when enabled.
        /// </summary>
        protected virtual void OnEnabled() { }

        /// <summary>
        /// Called during OnDisable. Override to implement custom logic when disabled.
        /// </summary>
        protected virtual void OnDisabled() { }
    }
}