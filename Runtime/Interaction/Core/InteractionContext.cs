using System;
using Koala.Simulation.Common;

namespace Koala.Simulation.Interaction.Core
{
    /// <summary>
    /// Holds contextual data for an interaction, including action details and callback behavior.
    /// </summary>
    [DocfxIgnore]
    public readonly struct InteractionContext
    {
        /// <summary>
        /// The input action map name associated with this interaction.
        /// </summary>
        public readonly string ActionName;

        /// <summary>
        /// The text prompt shown to describe this interaction. E.g "Take Item", "Open Door"
        /// </summary>
        public readonly string Prompt;

        /// <summary>
        /// The callback invoked when the interaction occurs.
        /// </summary>
        public readonly Action OnInteract;

        /// <summary>
        /// Additional arguments for the interaction.
        /// </summary>
        public readonly InteractionArgs InteractionArgs;

        /// <summary>
        /// Creates a new interaction context with an action name, prompt, and callback.
        /// </summary>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="prompt">The message shown to the player.</param>
        /// <param name="onInteractCallback">The callback to invoke on interaction.</param>
        public InteractionContext(string actionName, string prompt, Action onInteractCallback)
        {
            ActionName = actionName;
            Prompt = prompt;
            OnInteract = onInteractCallback;
            InteractionArgs = default;
        }

        /// <summary>
        /// Creates a new interaction context with an action name, prompt, callback, and arguments.
        /// </summary>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="prompt">The message shown to the player.</param>
        /// <param name="onInteractCallback">The callback to invoke on interaction.</param>
        /// <param name="interactionArgs">Additional data passed with the interaction.</param>
        public InteractionContext(string actionName, string prompt, Action onInteractCallback, InteractionArgs interactionArgs)
        {
            ActionName = actionName;
            Prompt = prompt;
            OnInteract = onInteractCallback;
            InteractionArgs = interactionArgs;
        }
    }
}