using UnityEngine;

namespace Koala.Simulation.Demo
{
    public interface IHoldable
    {
        T As<T>() where T : class;
        Transform transform { get; }
        void TaskOnHold();
        void TaskOnRelease();
    }
}