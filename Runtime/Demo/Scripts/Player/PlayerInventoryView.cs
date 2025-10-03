using UnityEngine;

namespace Koala.Simulation.Demo
{
    public class PlayerInventoryView : MonoBehaviour
    {
        [SerializeField] private Transform m_itemHoldAnchor;

        private void OnEnable()
        {
            Session.Player.Inventory.OnHold += OnHold;
            Session.Player.Inventory.OnRelease += OnRelease;
        }

        private void OnDisable()
        {
            Session.Player.Inventory.OnHold -= OnHold;
            Session.Player.Inventory.OnRelease -= OnRelease;
        }

        private void OnHold(IHoldable holdable)
        {
            holdable.transform.SetParent(m_itemHoldAnchor);
            holdable.transform.localPosition = Vector3.zero;
            holdable.transform.localRotation = Quaternion.identity;
        }

        private void OnRelease(IHoldable holdable)
        {
            holdable.transform.SetParent(null);
        }
    }
}