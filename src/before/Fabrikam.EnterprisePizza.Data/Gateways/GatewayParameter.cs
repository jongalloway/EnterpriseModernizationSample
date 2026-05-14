namespace Fabrikam.EnterprisePizza.Data.Gateways
{
    public class GatewayParameter
    {
        public GatewayParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; private set; }

        public object Value { get; private set; }
    }
}
