using UnityEngine;

namespace Koala.Simulation.Demo
{
    [DefaultExecutionOrder(-99)]
    public class Session : MonoBehaviour
    {
        public static PlayerService Player { get; private set; }

        private void Awake()
        {
            Player = new PlayerService(new PlayerData());
        }
    }
}