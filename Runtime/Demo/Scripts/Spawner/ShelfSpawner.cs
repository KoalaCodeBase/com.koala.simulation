using Koala.Simulation.Interaction.Core;
using Koala.Simulation.Inventory;
using Koala.Simulation.Placement;
using Koala.Simulation.Placement.Samples;
using UnityEngine;

namespace Koala.Simulation.Demo
{
    public class ShelfSpawner : InteractionBehaviour
    {
        [SerializeField] private Container _containerPrefab;

        private void Start()
        {
            PlacementManager.Instance.AddRule(new NoOverlapRule(LayerMask.GetMask("Default")));
            PlacementManager.Instance.AddRule(new LayerRule(LayerMask.GetMask("Default")));
        }

        protected override void OnInteract()
        {
            PlacementManager.Instance.Start(_containerPrefab.gameObject, null, null, (gameObject) =>
            {
                Container container = gameObject.GetComponent<Container>();
                container.Initialize();
                container.Register();
            });
        }
    }
}