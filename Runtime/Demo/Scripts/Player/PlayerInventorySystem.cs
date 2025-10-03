using System;
using Koala.Simulation.Inventory;

namespace Koala.Simulation.Demo
{
    public class PlayerInventorySystem
    {
        private PlayerData m_playerData;
        private IHoldable m_defaultHoldable;
        private IHoldable m_currentHoldable;

        public IHoldable CurrentHoldable => m_currentHoldable ?? m_defaultHoldable;
        public bool IsHandsFree => CheckIsHandsFree();
        public event Action<IHoldable> OnHold;
        public event Action<IHoldable> OnRelease;

        public PlayerInventorySystem(PlayerData playerData)
        {
            m_playerData = playerData;
        }

        public void SetCurrentHoldable(IHoldable holdable)
        {
            m_defaultHoldable = holdable;
        }

        public void Hold(IHoldable holdable)
        {
            if (m_currentHoldable == holdable)
                return;

            m_currentHoldable = holdable;
            m_currentHoldable.TaskOnHold();
            OnHold?.Invoke(holdable);
            //m_playerData.EquippedItem = holdable.transform.name;
        }

        public void Release()
        {
            OnRelease?.Invoke(m_currentHoldable);
            m_currentHoldable.TaskOnRelease();
            m_currentHoldable = null;
            //m_playerData.EquippedItem = "";
        }

        private bool CheckIsHandsFree()
        {
            if (CurrentHoldable == m_defaultHoldable)
            {
                if (m_defaultHoldable.As<Container>().Count == 0)
                    return true;
            }

            return false;
        }
    }
}