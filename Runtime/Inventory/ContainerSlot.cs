using UnityEngine;

namespace Koala.Simulation.Inventory
{
    [System.Serializable]
    internal struct ContainerSlot
    {
        public Transform WorldPoint;
        public Item Item;
        public bool IsEmpty => Item == null;
    }

    internal struct ContainerSlotStack
    {
        private ContainerSlot[] _slots;
        private int _count;

        public ContainerSlotStack(ContainerSlot[] source)
        {
            _slots = source;
            _count = 0;
        }

        public int Capacity => _slots.Length;
        public int Count => _count;
        public bool IsFull => _count >= _slots.Length;
        public bool IsEmpty => _count == 0;

        public bool TryPush(Item item, out Transform slotPoint)
        {
            slotPoint = null;
            if (IsFull) return false;

            slotPoint = _slots[_count].WorldPoint;
            _slots[_count].Item = item;
            _count++;
            return true;
        }

        public bool TryPop(out Item item)
        {
            if (IsEmpty)
            {
                item = null;
                return false;
            }

            _count--;
            item = _slots[_count].Item;
            _slots[_count].Item = null;
            return true;
        }

        public ref ContainerSlot Peek() => ref _slots[_count - 1];
        public ref ContainerSlot this[int index] => ref _slots[index];
    }
}