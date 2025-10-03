using Koala.Simulation.Interaction.Core;
using Koala.Simulation.Inventory;
using UnityEngine;

namespace Koala.Simulation.Demo
{
    public class BoxSpawner : InteractionBehaviour
    {
        [SerializeField] private Container _boxPrefab;
        [SerializeField] private Transform _spawnPoint;

        protected override void OnInteract()
        {
            Container container = Instantiate(_boxPrefab, _spawnPoint.position + Vector3.up, _spawnPoint.rotation);
            container.Initialize();
            container.Register();
        }
    }
}