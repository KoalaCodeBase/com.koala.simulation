using Koala.Simulation.Inventory;
using UnityEngine;

namespace Koala.Simulation.Demo
{
    public class BoxContainer : Container, IHoldable
    {   
        private Rigidbody m_rigidbody;
        private Collider m_collider;
        
        private void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody>();
            m_collider = GetComponent<Collider>();
        }
        
        public T As<T>() where T : class
        {
            return this as T;
        }

        public void TaskOnHold()
        {
            m_rigidbody.isKinematic = true;
            m_rigidbody.interpolation = RigidbodyInterpolation.None;
            m_collider.enabled = false;
        }

        public void TaskOnRelease()
        {
            m_rigidbody.isKinematic = false;
            m_rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            m_collider.enabled = true;
        }
    }
}