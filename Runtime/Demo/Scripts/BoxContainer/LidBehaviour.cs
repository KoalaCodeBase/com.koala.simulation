using Koala.Simulation.Interaction.Core;
using Koala.Simulation.Inventory;
using UnityEngine;

namespace Koala.Simulation.Demo
{
    public class LidBehaviour : InteractionBehaviour
    {
        [SerializeField] private BoxInteractionContext m_boxInteractionContext;
        [SerializeField] private Animator m_animator;
        [SerializeField] private Container m_container;

        private void Start()
        {
            m_container.IsEnabled = m_boxInteractionContext.IsOpen;
        }

        protected override bool OnIsInteractable()
        {
            if (m_boxInteractionContext.IsCarried)
                return false;
            else
            {
                prompt = m_boxInteractionContext.IsOpen ? "Close Lid" : "Open Lid";
                return true;
            }
        }

        protected override void OnInteract()
        {
            if (m_boxInteractionContext.IsOpen)
            {
                m_animator.SetTrigger("CloseLid");
                m_boxInteractionContext.IsOpen = false;
                m_container.IsEnabled = false;
            }
            else
            {
                m_animator.SetTrigger("OpenLid");
                m_boxInteractionContext.IsOpen = true;
                m_container.IsEnabled = true;
            }
        }
    }
}