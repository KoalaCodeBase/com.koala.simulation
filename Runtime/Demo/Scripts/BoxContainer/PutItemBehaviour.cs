using Koala.Simulation.Interaction.Core;
using Koala.Simulation.Inventory;
using UnityEngine;

namespace Koala.Simulation.Demo
{
    public class PutItemBehaviour : InteractionBehaviour
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
            Session.Player.Inventory.CurrentHoldable.As<Container>().TransferTo(m_container);
        }
    }
}
