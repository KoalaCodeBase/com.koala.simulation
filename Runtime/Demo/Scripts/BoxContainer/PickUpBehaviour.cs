using Koala.Simulation.Interaction.Core;
using UnityEngine;

namespace Koala.Simulation.Demo
{
    public class PickUpBehaviour : InteractionBehaviour
    {
        [SerializeField] private BoxInteractionContext m_context;

        protected override bool OnIsInteractable()
        {
            if (Session.Player.Inventory.IsHandsFree)
            {
                if (!m_context.IsCarried)
                {
                    prompt = "Pick Up";
                    return true;
                }
            }
            else
            {
                if (m_context.IsCarried)
                {
                    prompt = "Drop";
                    return true;
                }
            }

            return false;
        }

        protected override void OnInteract()
        {
            if (!m_context.IsCarried)
            {
                m_context.IsCarried = true;
                Session.Player.Inventory.Hold(m_context.Holdable);
            }
            else
            {
                m_context.IsCarried = false;
                Session.Player.Inventory.Release();
            }
        }
    }
}