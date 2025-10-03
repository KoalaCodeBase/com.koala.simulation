namespace Koala.Simulation.Inventory
{
    internal static class InventoryBootstrapper
    {
        internal static void Initialize()
        {
            var dtos = InventorySaver.LoadAll();
            foreach (var dto in dtos)
            {
                if (dto.IsSceneElement) continue;
                Container container = InventoryFactory.Create<Container>(dto.PrefabId);
                container.Initialize();
                container.Register(dto);
            }
        }
    }
}