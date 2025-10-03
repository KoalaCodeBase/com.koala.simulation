using Koala.Simulation.Interaction.Core;
using Koala.Simulation.Inventory;
using UnityEngine;

namespace Koala.Simulation.Demo
{
    public class ShelfPutBehaviour : InteractionBehaviour
    {
        [SerializeField] private Container m_container;

        protected override void OnInteract()
        {
            Session.Player.Inventory.CurrentHoldable.As<Container>().TransferTo(m_container);
        }
    }
}