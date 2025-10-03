using UnityEngine;

namespace Koala.Simulation.Demo
{
    public class BoxInteractionContext : MonoBehaviour
    {
        [SerializeField] private GameObject m_holdable;
        public bool IsOpen;
        public bool IsCarried;
        public IHoldable Holdable;
        
        private void Awake()
        {
            Holdable = m_holdable.GetComponent<IHoldable>();
        }
    }
}