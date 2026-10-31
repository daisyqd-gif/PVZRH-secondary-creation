#nullable enable
namespace CustomPlantClass.UI
{
    /// <summary>Describes one labeled action button in the mod options UI.</summary>
    public class KeyBindingEntry
    {
        /// <summary>Gets the function that supplies the entry's displayed label.</summary>
        public Func<string> Label { get; }
        /// <summary>Gets the action invoked when the entry's button is clicked.</summary>
        public Action<ActionButton>? OnClicked { get; }
        /// <summary>Gets the optional callback that configures the entry's button.</summary>
        public Action<ActionButton>? CustomSetup { get; }

        /// <summary>Creates a UI entry with its label, click action, and optional setup callback.</summary>
        /// <param name="label">Supplies the text displayed for the entry.</param>
        /// <param name="onClicked">Handles clicks on the entry's button.</param>
        /// <param name="customSetup">Optionally configures the entry's button.</param>
        public KeyBindingEntry(
            Func<string> label,
            Action<ActionButton>? onClicked,
            Action<ActionButton>? customSetup = null)
        {
            Label = label;
            OnClicked = onClicked;
            CustomSetup = customSetup;
        }
    }
    /// <summary>Stores the entries displayed by the custom options UI.</summary>
    public static class KeyBindingRegistry
    {
        /// <summary>Gets the registered options UI entries.</summary>
        public static readonly List<KeyBindingEntry> Entries = new();

        /// <summary>Registers an entry to display in the custom options UI.</summary>
        /// <param name="label">Supplies the text displayed for the entry.</param>
        /// <param name="onClicked">Handles clicks on the entry's button.</param>
        /// <param name="customSetup">Optionally configures the entry's button.</param>
        public static void Add(
            Func<string> label,
            Action<ActionButton> onClicked,
            Action<ActionButton>? customSetup = null)
        {
            Entries.Add(new KeyBindingEntry(label, onClicked, customSetup));
        }
    }
}
