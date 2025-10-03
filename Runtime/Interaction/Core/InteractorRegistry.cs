using System.Collections.Generic;
using Koala.Simulation.Common;

namespace Koala.Simulation.Interaction.Core
{
    /// <summary>
    /// Provides registration and lookup for interactors by ID.
    /// </summary>
    [DocfxIgnore]
    public static class InteractorRegistry
    {
        private static int _nextId = 1;
        private static readonly Dictionary<int, Interactor> _map = new();

        /// <summary>
        /// Registers a new interactor and assigns it a unique ID.
        /// </summary>
        /// <param name="interactor">The interactor instance to register.</param>
        /// <returns>The assigned interactor ID.</returns>
        public static int Register(Interactor interactor)
        {
            int id = _nextId++;
            _map[id] = interactor;
            return id;
        }

        /// <summary>
        /// Retrieves an interactor by its ID.
        /// </summary>
        /// <param name="id">The interactor ID.</param>
        /// <returns>The registered interactor instance.</returns>
        public static Interactor Get(int id) => _map[id];
    }

    /// <summary>
    /// Provides registration and lookup for interactable objects by ID.
    /// </summary>
    [DocfxIgnore]
    public static class InteractableRegistry
    {
        private static int _nextId = 1;
        private static readonly Dictionary<int, InteractableObject> _map = new();

        /// <summary>
        /// Registers a new interactable object and assigns it a unique ID.
        /// </summary>
        /// <param name="interactableObject">The interactable object to register.</param>
        /// <returns>The assigned interactable ID.</returns>
        public static int Register(InteractableObject interactableObject)
        {
            int id = _nextId++;
            _map[id] = interactableObject;
            return id;
        }

        /// <summary>
        /// Retrieves an interactable object by its ID.
        /// </summary>
        /// <param name="id">The interactable ID.</param>
        /// <returns>The registered interactable object.</returns>
        public static InteractableObject Get(int id) => _map[id];
    }
}