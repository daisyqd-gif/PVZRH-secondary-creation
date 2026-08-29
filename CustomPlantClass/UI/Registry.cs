#nullable enable
namespace CustomPlantClass.UI
{
    public class KeyBindingEntry
    {
        public Func<string> Label { get; }
        public Action<ActionButton>? OnClicked { get; }
        public Action<ActionButton>? CustomSetup { get; }

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
    public static class KeyBindingRegistry
    {
        public static readonly List<KeyBindingEntry> Entries = new();

        public static void Add(
            Func<string> label,
            Action<ActionButton> onClicked,
            Action<ActionButton>? customSetup = null)
        {
            Entries.Add(new KeyBindingEntry(label, onClicked, customSetup));
        }
    }
}
