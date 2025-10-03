namespace Koala.Simulation.Demo
{
    public class PlayerService
    {
        private PlayerData m_playerData;
        public PlayerInventorySystem Inventory { get; private set; }

        public PlayerService(PlayerData playerData)
        {
            m_playerData = playerData;
            Inventory = new PlayerInventorySystem(m_playerData);
        }
    }
}