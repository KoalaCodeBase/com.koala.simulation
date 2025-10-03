using DG.Tweening;
using Koala.Simulation.Common;
using UnityEngine;

namespace Koala.Simulation.Inventory
{
    /// <summary>
    /// Defines a transfer animation for moving an item into a target slot.
    /// </summary>
    public interface ITransferAnimation
    {
        /// <summary>
        /// Executes the transfer animation.
        /// </summary>
        /// <param name="item">The item being transferred.</param>
        /// <param name="targetSlot">The target slot transform.</param>
        void Execute(Item item, Transform targetSlot);
    }

    [DocfxIgnore]
    [System.Serializable]
    public class InstantTransfer : ITransferAnimation
    {
        /// <summary>
        /// Instantly sets the item's position, rotation, and parent to match the target slot.
        /// </summary>
        /// <param name="item">The item being transferred.</param>
        /// <param name="targetSlot">The target slot transform.</param>
        public void Execute(Item item, Transform targetSlot)
        {
            item.transform.position = targetSlot.position;
            item.transform.rotation = targetSlot.rotation;
            item.transform.SetParent(targetSlot);
        }
    }

    [DocfxIgnore]
    [System.Serializable]
    public class JumpTransfer : ITransferAnimation
    {
        [SerializeField] private float _jumpPower = 1f;
        [SerializeField] private float _duration = 0.5f;

        /// <summary>
        /// Animates the item with a jump movement into the target slot.
        /// </summary>
        /// <param name="item">The item being transferred.</param>
        /// <param name="targetSlot">The target slot transform.</param>
        public void Execute(Item item, Transform targetSlot)
        {
            item.transform.DOKill();

            float verticalDelta = targetSlot.position.y - item.transform.position.y;
            float adjustedJumpPower = _jumpPower;

            if (verticalDelta > 0)
                adjustedJumpPower += verticalDelta * 0.5f;

            else if (verticalDelta < 0)
                adjustedJumpPower = Mathf.Max(0.5f, _jumpPower + verticalDelta * 0.3f);

            item.transform.SetParent(targetSlot);
            item.transform
                .DOLocalJump(Vector3.zero, adjustedJumpPower, 1, _duration)
                .SetSpeedBased(true)
                .SetEase(Ease.InOutSine)
                .OnComplete(() => CompleteTransfer(item, targetSlot));
        }

        private static void CompleteTransfer(Item item, Transform targetSlot)
        {
            item.transform.position = targetSlot.position;
            item.transform.rotation = targetSlot.rotation;
        }
    }
}