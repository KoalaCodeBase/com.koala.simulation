using System;
using UnityEngine;

namespace Koala.Simulation.Interaction.Core
{
    /// <summary>
    /// Base behaviour that defines how an object can be interacted with.
    /// </summary>
    public abstract class InteractionBehaviour : MonoBehaviour
    {
        /// <summary>
        /// The input action map name associated with this interaction.
        /// </summary>
        [SerializeField] internal string actionMapName;

        /// <summary>
        /// The text prompt shown to describe this interaction. E.g "Take Item", "Open Door"
        /// </summary>
        [SerializeField] internal string prompt;
        internal event Action OnInteractionPerform;

        public void Interact(InteractionArgs args)
        {
            if (!IsInteractable())
                return;

            OnInteract(InteractorRegistry.Get(args.interactorId), InteractableRegistry.Get(args.interactableId));
            OnInteractionPerform?.Invoke();
        }

        public void Interact()
        {
            if (!IsInteractable())
                return;

            OnInteract();
            OnInteractionPerform?.Invoke();
        }
        
        public bool IsInteractable()
        {
            return OnIsInteractable();
        }

        /// <summary>
        /// Checks if this interaction is currently available.
        /// Override to define custom interactability logic.
        /// </summary>
        /// <returns><c>true</c> if interactable, otherwise <c>false</c>.</returns>
        protected virtual bool OnIsInteractable()
        {
            return true;
        }

        /// <summary>
        /// Performs the interaction without arguments if allowed.
        /// Override to define custom interaction logic.
        /// </summary>
        protected virtual void OnInteract() { }

        /// <summary>
        /// Performs the interaction with the given arguments if allowed.
        /// Override to define custom interaction logic.
        /// </summary>
        /// <param name="args">Interaction arguments containing interactor and interactable IDs.</param>
        protected virtual void OnInteract(Interactor interactor, InteractableObject interactable) { }
    }
}