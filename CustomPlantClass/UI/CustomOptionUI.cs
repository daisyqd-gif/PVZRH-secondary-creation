using System;
using System.Collections.Generic;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.Injection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace CustomPlantClass.UI
{
    /// <summary>
    /// A generic mod options UI builder.
    /// Builds rows dynamically from KeyBindingRegistry, but now uses ActionButton
    /// instead of keybinding logic.
    /// </summary>
    public class KeyBindingUI : MonoBehaviour
    {
        private IEnumerable<KeyBindingEntry> Entries => KeyBindingRegistry.Entries;

        #region Il2Cpp Constructors

        public KeyBindingUI() : base(ClassInjector.DerivedConstructorPointer<KeyBindingUI>())
        {
            ClassInjector.DerivedConstructorBody(this);
        }

        public KeyBindingUI(IntPtr ptr) : base(ptr)
        {
        }

        #endregion

        #region Unity

        public void Start()
        {
            // Disable original config menu
            var cfg = GetComponent<UIConfigMenu>();
            if (cfg != null)
                cfg.enabled = false;

            // Title
            var title = transform.GetChild(0).GetChild(0);
            title.GetComponent<TextMeshProUGUI>().text = "Mod Options";

            // Layout container
            var layout = transform.GetChild(2);

            // Hide default fields
            foreach (var field in layout)
                field.TryCast<Transform>()?.gameObject.SetActive(false);

            // Build template
            var template = BuildTemplate(layout);

            // Build rows
            foreach (var entry in Entries)
            {
                var row = Instantiate(template, layout.transform);
                row.SetActive(true);

                // Set label
                row.GetComponent<TextMeshProUGUI>().text = entry.Label();

                // Get ActionButton
                var button = row.transform.GetChild(1).GetComponent<ActionButton>();

                // Apply modder setup
                entry.CustomSetup?.Invoke(button);

                // Assign click action
                button.OnClicked = entry.OnClicked;
            }

            // Cleanup template
            Destroy(template);
        }

        #endregion

        #region Template builder

        private static GameObject BuildTemplate(Transform layout)
        {
            var template = Instantiate(layout.GetChild(0).gameObject);

            // Hide default Input field
            var input = template.transform.FindChild("Input");
            if (input != null)
                input.gameObject.SetActive(false);

            // Load button prefab
            var buttonPrefab = Resources.Load<GameObject>("ui\\prefabs\\sample\\Button");
            var button = Instantiate(buttonPrefab);
            button.transform.SetParent(template.transform);

            // Replace with ActionButton
            button.AddComponent<ActionButton>();

            // Layout adjustments
            var rect = button.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(-40, 60);
            rect.anchoredPosition = new Vector2(0, -30);

            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().fontSizeMax = 32;

            return template;
        }

        #endregion
    }
}
