using CustomPlantClass.Networking;

namespace CustomPlantCreator
{
    public abstract class EventNode : BaseNode, ICommandListener
    {
        public override Color BackgroundColor => new Color(1f, 0.5f, 0f);
        public override bool CanSendTrigger => true;
        public override bool IsTriggerable => false;
        public override bool IsAssociable => true;
        public abstract string CommandName { get; }

        public void OnCommandReceived(string data)
        {
            Trigger();
        }

        public void Trigger()
        {
            OnTrigger();
            TriggerOutputPins();
        }
    }
}