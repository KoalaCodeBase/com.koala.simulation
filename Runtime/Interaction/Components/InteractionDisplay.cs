using System.Collections.Generic;
using Koala.Simulation.Interaction.Core;
using UnityEngine;

namespace Koala.Simulation.Interaction.Components
{
    /// <summary>
    /// Displays active interaction prompts in the UI using a fixed pool of 12 labels.
    /// <note type="tip">
    /// It is strongly recommended to place this component on its own dedicated Canvas,
    /// as frequent updates may otherwise cause unnecessary layout rebuilds
    /// in other UI elements.
    /// </note>
    /// </summary>
    [AddComponentMenu("Simulation/Interaction/Interaction Display")]
    [Icon("Packages/com.koala.simulation/Editor/Icons/koala.png")]
    [DefaultExecutionOrder(1)]
    public sealed class InteractionDisplay : MonoBehaviour
    {
        /// <summary>
        /// The interactor providing interaction data to display.
        /// </summary>
        [SerializeField] private Interactor _interactor;

        /// <summary>
        /// The UI container where interaction labels are placed.
        /// </summary>
        [SerializeField] private RectTransform _viewPort;

        /// <summary>
        /// The label prefab used for displaying an interaction.
        /// </summary>
        [SerializeField] private InteractionLabel _interactionLabel;

        private readonly List<InteractionLabel> _labels = new();

        private void OnEnable()
        {
            _interactor.Solver.OnNewInteraction += UpdateView;
        }

        private void OnDisable()
        {
            _interactor.Solver.OnNewInteraction -= UpdateView;
        }

        private void Start()
        {
            _interactionLabel.gameObject.SetActive(false);

            for (int i = 0; i < 12; i++)
            {
                var label = Instantiate(_interactionLabel, _viewPort);
                label.gameObject.SetActive(false);
                _labels.Add(label);
            }
        }

        private void UpdateView(IReadOnlyDictionary<string, InteractionContext> interactions)
        {
            var enumerator = interactions.GetEnumerator();
            int index = 0;

            while (enumerator.MoveNext() && index < _labels.Count)
            {
                var label = _labels[index];
                label.Set(enumerator.Current.Value);

                if (!label.gameObject.activeSelf)
                    label.gameObject.SetActive(true);

                index++;
            }

            for (int i = index; i < _labels.Count; i++)
            {
                var label = _labels[i];
                if (label.gameObject.activeSelf)
                    label.gameObject.SetActive(false);
            }
        }

    }
}