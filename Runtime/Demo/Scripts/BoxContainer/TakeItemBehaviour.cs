using Koala.Simulation.Interaction.Core;
using Koala.Simulation.Inventory;
using UnityEngine;

namespace Koala.Simulation.Demo
{
    public class TakeItemBehaviour : InteractionBehaviour
    {
        [SerializeField] private BoxInteractionContext m_context;
        [SerializeField] private Container m_container;
        
        protected override bool OnIsInteractable()
        {
            if (!m_context.IsOpen || m_context.IsCarried)
                return false;

            return true;
        }

        protected override void OnInteract()
        {
            m_container.TransferTo(Session.Player.Inventory.CurrentHoldable.As<Container>());
        }
    }
}