namespace CustomPlantCreator
{
    public class ClassNode : BaseNode
    {
        public override Color BackgroundColor => Color.red;

        public override string Name => "类";

        public ClassNode()
        {
            OutputPins.Add(new PinBase(this) { Data = new Data() });
        }

        private class Data : PinData
        {
            public override bool IsTyped => true;
            public override Type Type => typeof(object);
            public override string Name => "类";
            public override Color BackgroundColor => Color.red;
        }
    }
    public class FunctionNode : BaseNode
    {
        public override Color BackgroundColor => Color.red;

        public override string Name => "函数";
        public override bool CanSendTrigger => true;
        public override bool IsTriggerable => true;
        public override bool IsAssociable => true;
        public override void OnTrigger()
        {
            // Implement the function trigger logic here
        }
        public FunctionType FunctionType { get; set; }

        public FunctionNode()
        {
            FunctionType = new FunctionType(this);
            OutputPins.Add(new PinBase(this) { Data = new Data() });
        }

        private class Data : PinData
        {
            public override bool IsTyped => true;
            public override Type Type => typeof(object);
            public override string Name => "函数";
            public override Color BackgroundColor => Color.red;
        }
    }
}