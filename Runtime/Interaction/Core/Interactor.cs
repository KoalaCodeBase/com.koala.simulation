using Koala.Simulation.Common;
using UnityEngine;

namespace Koala.Simulation.Interaction.Core
{
    /// <summary>
    /// Handles player interactions and manages an interaction solver.
    /// 
    /// Registers itself in the interaction system and provides a solver
    /// instance for resolving interaction logic.
    /// 
    /// Inherit this class and override <see cref="OnAwake"/> or <see cref="OnDestroyed"/>
    /// to extend setup and cleanup behavior.
    /// </summary>
    /// <example>
    /// <xref uid="Koala.Simulation.Interaction.Components.InteractionRaycaster" altProperty="fullName" displayProperty="name"/>
    /// </example>
    public abstract class Interactor : MonoBehaviour
    {
        private int _id;
        private InteractionSolver _interactionSolver;

        /// <summary>
        /// The unique identifier assigned to this interactor.
        /// </summary>
        protected int Id => _id;

        /// <summary>
        /// The solver responsible for processing and resolving interactions.
        /// </summary>
        public InteractionSolver Solver => _interactionSolver;

        /// <summary>
        /// Whether to automatically generate interaction arguments when creating the solver.
        /// </summary>
        [SerializeField] private bool _generateInteractionArgs;

        [DocfxIgnore]
        private void Awake()
        {
            _id = InteractorRegistry.Register(this);
            _interactionSolver = new InteractionSolver(_generateInteractionArgs, Id);

            OnAwake();
        }

        private void Update()
        {
            OnUpdate();
            _interactionSolver.Tick();
        }

        private void OnDisable()
        {
            _interactionSolver.Dispose();
            OnDisabled();
        }

        [DocfxIgnore]
        private void OnDestroy()
        {
            _interactionSolver.Dispose();
            OnDestroyed();
        }

        /// <summary>
        /// Called every frame.
        /// Override to add custom update logic.
        /// </summary>
        protected virtual void OnUpdate() { }

        /// <summary>
        /// Called after the interactor is registered and the solver is created.
        /// Override to add custom initialization logic.
        /// </summary>
        protected virtual void OnAwake() { }

        /// <summary>
        /// Called when the interactor is disabled.
        /// Override to add custom cleanup logic.
        /// </summary>
        protected virtual void OnDisabled() { }

        /// <summary>
        /// Called before the interactor is destroyed and the solver is disposed.
        /// Override to add custom cleanup logic.
        /// </summary>
        protected virtual void OnDestroyed() { }
    }
}