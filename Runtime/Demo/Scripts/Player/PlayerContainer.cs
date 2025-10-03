using Koala.Simulation.Inventory;

namespace Koala.Simulation.Demo
{
    public class PlayerContainer : Container, IHoldable
    {
        protected override void OnStart()
        {
            Session.Player.Inventory.SetCurrentHoldable(this);
        }
        public T As<T>() where T : class
        {
            return this as T;
        }

        public void TaskOnHold(){}

        public void TaskOnRelease(){}
    }
}