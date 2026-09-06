namespace CustomPlantCreator
{
    public class IntValueNode : BaseNode
    {
        public override Color BackgroundColor => Color.green;

        public override string Name => "整数值";

        public IntValueNode()
        {
            OutputPins.Add(new PinBase(this) { Data = new Data() });
        }

        private class Data : PinData
        {
            public override bool IsTyped => true;
            public override Type Type => typeof(int);
            public override string Name => "整数值";
            public override Color BackgroundColor => Color.green;
        }
    }
    public class FloatValueNode : BaseNode
    {
        public override Color BackgroundColor => Color.green;

        public override string Name => "浮点值";

        public FloatValueNode()
        {
            OutputPins.Add(new PinBase(this) { Data = new Data() });
        }

        private class Data : PinData
        {
            public override bool IsTyped => true;
            public override Type Type => typeof(float);
            public override string Name => "浮点值";
            public override Color BackgroundColor => Color.green;
        }
    }
    public class StringValueNode : BaseNode
    {
        public override Color BackgroundColor => Color.green;

        public override string Name => "字符串值";
        public StringValueNode()
        {
            OutputPins.Add(new PinBase(this) { Data = new Data() });
        }

        private class Data : PinData
        {
            public override bool IsTyped => true;
            public override Type Type => typeof(string);
            public override string Name => "字符串值";
            public override Color BackgroundColor => Color.green;
        }
    }
    public class BoolValueNode : BaseNode
    {
        public override Color BackgroundColor => Color.green;
        
        public override string Name => "布尔值";
        public BoolValueNode()
        {
            OutputPins.Add(new PinBase(this) { Data = new Data() });
        }

        private class Data : PinData
        {
            public override bool IsTyped => true;
            public override Type Type => typeof(bool);
            public override string Name => "布尔值";
            public override Color BackgroundColor => Color.green;
        }
    }
}