using Koala.Simulation.Common;

namespace Koala.Simulation.Interaction.Core
{
    /// <summary>
    /// Holds identifiers for the interactor and interactable involved in an interaction.
    /// </summary>
    [DocfxIgnore]
    public struct InteractionArgs
    {
        /// <summary>
        /// The unique ID of the interactor performing the interaction.
        /// </summary>
        public int interactorId;

        /// <summary>
        /// The unique ID of the interactable being interacted with.
        /// </summary>
        public int interactableId;

        /// <summary>
        /// Creates a new set of interaction arguments with interactor and interactable IDs.
        /// </summary>
        /// <param name="interactorId">The ID of the interactor.</param>
        /// <param name="interactableId">The ID of the interactable.</param>
        public InteractionArgs(int interactorId, int interactableId)
        {
            this.interactorId = interactorId;
            this.interactableId = interactableId;
        }
    }
}