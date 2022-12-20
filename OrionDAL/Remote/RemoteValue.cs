using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrionDAL.Remote
{
    public class RemoteValue
    {
        public bool? BoolValue { get { return (Value != null && Value.GetType() == typeof(bool)) ? (bool?)Value : null; } set { if(value!=null) Value = value; } }
        public byte? ByteValue { get { return (Value != null && Value.GetType() == typeof(byte)) ? (byte?)Value : null; } set { if(value!=null) Value = value; } }
        public long? LongValue { get { return (Value != null && Value.GetType() == typeof(long)) ? (long?)Value : null; } set { if(value!=null) Value = value; } }
        public int? IntValue { get { return (Value != null && Value.GetType() == typeof(int)) ? (int?)Value : null; } set { if(value!=null) Value = value; } }
        public decimal? DecimalValue { get { return (Value != null && Value.GetType() == typeof(decimal)) ? (decimal?)Value : null; } set { if(value!=null) Value = value; } }
        public string StringValue { get { return (Value != null && Value.GetType() == typeof(string)) ? (string)Value : null; } set { if(value!=null) Value = value; } }

        [JsonIgnore]
        public object Value { get; set; }

        public RemoteValue() { }

        public RemoteValue(object value)
        {
            this.Value = value;
        }

        public static object[] ListToArray(List<RemoteValue> values)
        {
            var result = new List<object>();

            if (values != null)
            {
                foreach (var item in values)
                {
                    result.Add(item.Value);
                }
            }

            return result.ToArray();
        }
    }
}
