using Koala.Simulation.Interaction.Core;
using Koala.Simulation.Inventory;
using UnityEngine;

namespace Koala.Simulation.Demo
{
    public class ShelfTakeBehaviour : InteractionBehaviour
    {
        [SerializeField] private Container m_container;
        protected override void OnInteract()
        {
            m_container.TransferTo(Session.Player.Inventory.CurrentHoldable.As<Container>());
        }
    }
}