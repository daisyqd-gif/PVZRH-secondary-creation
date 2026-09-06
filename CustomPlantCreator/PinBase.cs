namespace CustomPlantCreator
{
    public class PinBase
    {
        public BaseNode ParentNode { get; }
        public PinData Data { get; set; }
        public PinConnection Connection { get; protected set; }
        public virtual PinConnection Connect(PinBase otherPin)
        {
            if(otherPin.Data.IsTyped && Data.IsTyped && otherPin.Data.Type != Data.Type)
            {
                return new InvalidPinConnection(otherPin, this);
            }
            var connection = new PinConnection(this, otherPin);
            Connection = connection;
            return null;
        }
        public PinBase(BaseNode parentNode)
        {
            ParentNode = parentNode;
        }
    }
    public class TriggerPin : PinBase
    {
        public void Trigger()
        {
            if (ParentNode.IsTriggerable)
            {
                ParentNode.OnTrigger();
            }
        }
        public override PinConnection Connect(PinBase otherPin)
        {
            if (otherPin is TriggerPin triggerPin)
            {
                var connection = new TriggerPinConnection(this, triggerPin);
                Connection = connection;
                return connection;
            }
            return new InvalidPinConnection(otherPin, this); // Cannot connect to a non-trigger pin
        }
        public TriggerPin(BaseNode parentNode) : base(parentNode)
        {
        }
    }
    public class AssociationPin : PinBase
    {
        public override PinConnection Connect(PinBase otherPin)
        {
            if (otherPin is AssociationPin associationPin)
            {
                var connection = new PinConnection(this, associationPin);
                Connection = connection;
                if(ParentNode is ClassNode classNode && associationPin.ParentNode is ClassNode otherClassNode)
                {
                    // Handle the association between two ClassNodes
                    // For example, you might want to store a reference to the associated class in the ClassNode
                    // classNode.AssociatedClass = otherClassNode;
                }
                return connection;
            }
            return new InvalidPinConnection(otherPin, this); // Cannot connect to a non-association pin
        }
        public AssociationPin(BaseNode parentNode) : base(parentNode)
        {
        }
    }
    public class PinConnection
    {
        public PinBase InputPin { get; protected set; }
        public PinBase OutputPin { get; protected set; }

        public PinConnection(PinBase inputPin, PinBase outputPin)
        {
            InputPin = inputPin;
            OutputPin = outputPin;
        }
    }
    public class TriggerPinConnection : PinConnection
    {
        public TriggerPinConnection(TriggerPin inputPin, TriggerPin outputPin) : base(inputPin, outputPin)
        {
            InputPin = inputPin;
            OutputPin = outputPin;
        }

        public void Trigger()
        {
            if (OutputPin.ParentNode.CanSendTrigger && OutputPin is TriggerPin triggerPin)
            {
                triggerPin.Trigger();
            }
        }
    }
    public class InvalidPinConnection : PinConnection
    {
        public InvalidPinConnection(PinBase inputPin, PinBase outputPin) : base(inputPin, outputPin)
        {
            InputPin = inputPin;
            OutputPin = outputPin;
        }
    }
    public abstract class PinData
    {
        public abstract string Name { get; }
        public abstract Color BackgroundColor { get; }
        public abstract Type Type { get; }

        public object Data { get; protected set; }
        public virtual bool IsTyped => false;

        public void SetData(object data)
        {
            if (data != null && !Type.IsInstanceOfType(data))
                throw new ArgumentException($"Data must be of type {Type.Name}");

            Data = data;
        }
    }
    public abstract class PinData<T> : PinData
    {
        public override Type Type => typeof(T);
        public override bool IsTyped => true;

        public T TypedData
        {
            get => (T)Data;
            set => Data = value;
        }
    }
}