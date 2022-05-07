using System.Collections.Generic;
using System.Windows.Input;

namespace Cerberus.view
{
    public class Operation
    {
        private readonly OperationType _operationType;
        private readonly Dictionary<string, string> _values = new Dictionary<string, string>();
        public Operation(OperationType operationType)
        {
            _operationType = operationType;
        }
        public OperationType GetOperationType()
        {
            return _operationType;
        }

        public void SetValue(string key, string value)
        {
            if (_values.ContainsKey(key))
            {
                _values[key] = value;
            }
            else
            {
                _values.Add(key, value);
            }
        }
        
        public string GetValue(string key)
        {
            string value = "";
            
            if (_values.ContainsKey(key))
            {
                value = _values[key];
            }
            
            return value;
        }
    }
}