using UnityEngine;
using Koala.Simulation.Interaction.Core;

namespace Koala.Simulation.Demo
{
    public class DropBehaviour : InteractionBehaviour
    {
        [SerializeField] private BoxInteractionContext m_context;
        protected override bool OnIsInteractable()
        {
            return m_context.IsCarried;
        }
        protected override void OnInteract()
        {
            m_context.IsCarried = false;
            Session.Player.Inventory.Release();
        }
    }
}