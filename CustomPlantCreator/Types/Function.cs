
namespace CustomPlantCreator
{
    public class FunctionType
    {
        public bool IsStatic { get; set; } = false;
        public Type ReturnType { get; set; } = typeof(void);
        #nullable enable
        public ClassType? OwnerClass { get; set; }
        public List<ParameterType> Parameters { get; set; }
    #nullable disable
        public FunctionNode Host { get; set; }
        public FunctionType(FunctionNode host, params ParameterType[] parameters)
        {
            Host = host;
            Parameters = new List<ParameterType>(parameters);
        }
    }
    public class ParameterType
    {
        public Type Type { get; set; }
        public string Name { get; set; }
    }
}