using Koala.Simulation.Interaction.Core;
using UnityEngine;

namespace Koala.Simulation.Interaction.Components
{
    [AddComponentMenu("Simulation/Interaction/Interactable")]
    [DisallowMultipleComponent]
    [Icon("Packages/com.koala.simulation/Editor/Icons/koala.png")]
    /// <summary>
    /// Base class for all interactable objects in the simulation.
    /// 
    /// Provides a registry ID, manages attached interaction behaviours,
    /// and triggers interaction events when interacted with.
    /// 
    /// Inherit this class to create custom interactable objects by overriding
    /// lifecycle hooks such as <see cref="OnAwake"/>, <see cref="OnEnabled"/>, and <see cref="OnDisabled"/>.
    /// <note type="tip">
    /// Basically, add this component to a GameObject and define its <see cref="InteractionBehaviour"/>s
    /// to start to interact with the object.
    /// </note>
    /// </summary>
    public class Interactable : InteractableObject
    {
        
    }
}