using Koala.Simulation.Input;
using Koala.Simulation.Interaction.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Koala.Simulation.Interaction.Components
{
    /// <summary>
    /// UI label that displays an interaction prompt with its corresponding input key.
    /// </summary>
    [AddComponentMenu("Simulation/Interaction/Interaction Label")]
    [Icon("Packages/com.koala.simulation/Editor/Icons/koala.png")]
    public sealed class InteractionLabel : MonoBehaviour
    {
        /// <summary>
        /// Text field used to display the input key.
        /// </summary>
        [SerializeField] private TextMeshProUGUI _keyTMP;

        /// <summary>
        /// Image field used to display keyboard key icon.
        /// </summary>
        [SerializeField] private Image _keyImage;

        /// <summary>
        /// Image field used to display the input sprite.
        /// </summary>
        [SerializeField] private Image _keyMappedSpriteImage;

        /// <summary>
        /// Text field used to display the interaction prompt message.
        /// </summary>
        [SerializeField] private TextMeshProUGUI _promptTMP;

        /// <summary>
        /// Updates the label with the given interaction context.
        /// </summary>
        /// <param name="context">The interaction context containing action name and prompt.</param>
        public void Set(InteractionContext context)
        {
            if (InputManager.TryGetSprite(context.ActionName, out var sprite))
            {
                _keyMappedSpriteImage.sprite = sprite;
                _keyMappedSpriteImage.gameObject.SetActive(true);
                _keyImage.gameObject.SetActive(false);
                _keyTMP.gameObject.SetActive(false);
            }
            else
            {
                _keyTMP.text = InputManager.GetKeyDisplayName(context.ActionName);
                _keyTMP.gameObject.SetActive(true);
                _keyImage.gameObject.SetActive(true);
                _keyMappedSpriteImage.gameObject.SetActive(false);
            }
            
            _promptTMP.text = context.Prompt;
        }
    }
}