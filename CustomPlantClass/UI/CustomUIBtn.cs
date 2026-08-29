using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace CustomPlantClass.UI
{
    /// <summary>
    /// A simple modder‑friendly action button.
    /// When clicked, it executes a provided Action.
    /// No key binding, no input listening, no expression trees.
    /// </summary>
    public class ActionButton : MonoBehaviour
    {
        private TextMeshProUGUI _label;

        /// <summary>
        /// The text displayed on the button.
        /// </summary>
        public string Label
        {
            get => _label != null ? _label.text : "";
            set { if (_label != null) _label.text = value; }
        }

        /// <summary>
        /// The action to execute when clicked.
        /// Modders can assign anything here.
        /// </summary>
#nullable enable
        public Action<ActionButton>? OnClicked { get; set; }

        /// <summary>
        /// Optional setup hook for modders.
        /// Runs once in Start().
        /// </summary>
        public Action<ActionButton>? OnSetup { get; set; }

        private void Awake()
        {
            _label = GetComponentInChildren<TextMeshProUGUI>(true);
        }

        private void Start()
        {
            // Run modder setup
            OnSetup?.Invoke(this);

            // Subscribe to click event
            var button = GetComponent<TheButton>();
            if (button != null)
                SubscribeToTheEvent(button);
        }

        private void SubscribeToTheEvent(TheButton button)
        {
            if (button.theEvent_up != null)
                button.theEvent_up.AddListener((UnityAction)(Action)OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            OnClicked?.Invoke(this);
        }
    }
}
