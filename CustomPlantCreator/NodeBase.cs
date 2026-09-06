namespace CustomPlantCreator
{
    public abstract class BaseNode
    {
        public abstract Color BackgroundColor { get; }
        public abstract string Name { get; }

        public List<PinBase> InputPins { get; } = new();
        public List<PinBase> OutputPins { get; } = new();

        public virtual bool IsAssociable => false;
        public virtual bool IsTriggerable => false;
        public virtual bool CanSendTrigger => false;

        public TriggerPin OutputTriggerPin { get; }

        protected BaseNode()
        {
            // Create the trigger pin once
            OutputTriggerPin = new TriggerPin(this);

            // Add it to the output pin list so the editor can see it
            OutputPins.Add(OutputTriggerPin);
        }

        public virtual void OnTrigger() { }

        public virtual void TriggerOutputPins()
        {
            if (!CanSendTrigger) return;

            // Trigger all connected trigger pins
            if (OutputTriggerPin.Connection is TriggerPinConnection conn)
            {
                conn.Trigger();
            }
        }
    }
}