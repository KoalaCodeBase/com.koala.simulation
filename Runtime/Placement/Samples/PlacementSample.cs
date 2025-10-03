using Koala.Simulation.Input;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Koala.Simulation.Placement.Samples
{
    /// <summary>
    /// Demonstrates how to use the placement system with input controls.
    /// </summary>
    /// <example>
    /// <code>
    /// using Koala.Simulation.Placement;
    /// using Koala.Simulation.Placement.Samples;
    /// using UnityEngine;
    /// using UnityEngine.InputSystem;
    ///
    /// namespace Koala.Simulation.Samples
    /// {
    ///     public class PlacementSample : MonoBehaviour
    ///     {
    ///         [SerializeField]
    ///         private GameObject _prefab;
    ///
    ///         private void Start()
    ///         {
    ///             PlacementManager.Instance.AddRule(
    ///                 new NoOverlapRule(LayerMask.GetMask("Default"))
    ///             );
    ///             PlacementManager.Instance.AddRule(
    ///                 new LayerRule(LayerMask.GetMask("Default"))
    ///             );
    ///         }
    ///
    ///         private void Update()
    ///         {
    ///             // Start Placement (F)
    ///             if (Keyboard.current.fKey.wasPressedThisFrame)
    ///             {
    ///                 PlacementManager.Instance.Start(_prefab);
    ///             }
    ///
    ///             // Rotate Clockwise (E)
    ///             if (Keyboard.current.eKey.wasPressedThisFrame)
    ///             {
    ///                 PlacementManager.Instance.Rotate(true);
    ///             }
    ///
    ///             // Rotate Counterclockwise (Q)
    ///             if (Keyboard.current.qKey.wasPressedThisFrame)
    ///             {
    ///                 PlacementManager.Instance.Rotate(false);
    ///             }
    ///
    ///             // Confirm Placement (LMB)
    ///             if (Mouse.current.leftButton.wasPressedThisFrame)
    ///             {
    ///                 PlacementManager.Instance.Confirm();
    ///             }
    ///
    ///             // Cancel Placement (RMB)
    ///             if (Mouse.current.rightButton.wasPressedThisFrame)
    ///             {
    ///                 PlacementManager.Instance.Cancel();
    ///             }
    ///
    ///             // Reposition Placement (R)
    ///             if (Keyboard.current.rKey.wasPressedThisFrame)
    ///             {
    ///                 var ray = Camera.main.ScreenPointToRay(
    ///                     Mouse.current.position.ReadValue()
    ///                 );
    ///                 if (Physics.Raycast(ray, out var hit))
    ///                 {
    ///                     var placeable = hit.collider
    ///                         .GetComponentInParent&lt;IPlaceable&gt;();
    ///
    ///                     if (placeable != null)
    ///                         PlacementManager.Instance.Reposition(
    ///                             hit.collider.gameObject
    ///                         );
    ///                 }
    ///             }
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    public class PlacementSample : MonoBehaviour
    {
        /// <summary>
        /// The prefab used for placement during the sample. Prefab must implement <see cref="IPlaceable"/>.
        /// </summary>
        [SerializeField] private GameObject _prefab;

        private void Start()
        {
            PlacementManager.Instance.AddRule(new NoOverlapRule(LayerMask.GetMask("Default")));
            PlacementManager.Instance.AddRule(new LayerRule(LayerMask.GetMask("Default")));

            InputManager.Subscribe("Scroll", OnScroll, InputActionPhase.Started);
        }

        private void OnScroll(InputAction.CallbackContext ctx)
        {
            float scrollY = ctx.ReadValue<float>();

            if (scrollY > 0)
                PlacementManager.Instance.Rotate(true);
            else if (scrollY < 0)
                PlacementManager.Instance.Rotate(false);
        }

        private void Update()
        {
            // Start Placement (F)
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                PlacementManager.Instance.Start(_prefab);
            }

            // Rotate Clockwise (E)
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                PlacementManager.Instance.Rotate(true);
            }

            // Rotate Counterclockwise (Q)
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                PlacementManager.Instance.Rotate(false);
            }

            // Confirm Placement (LMB)
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                PlacementManager.Instance.Confirm();
            }

            // Cancel Placement (RMB)
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                PlacementManager.Instance.Cancel();
            }

            // Reposition Placement (R)
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out var hit))
                {
                    var placeable = hit.collider.GetComponentInParent<IPlaceable>();
                    if (placeable != null)
                        PlacementManager.Instance.Reposition(hit.collider.gameObject);
                }
            }
        }
    }
}