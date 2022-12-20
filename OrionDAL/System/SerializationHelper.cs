using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace System
{
    public class SerializationHelper
    {
        public static object GetObject(Type type, string xml)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(type);
            StringReader reader = new StringReader(xml.
                Replace("'True'", "'true'").
                Replace("'False'", "'false'"));
            object result = serializer.Deserialize(reader);

            return result;
        }

        public static string GetXml(object value)
        {
            if (value == null) return "";

            System.Xml.Serialization.XmlSerializer xml2 = new System.Xml.Serialization.XmlSerializer(value.GetType());
            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);
            xml2.Serialize(writer, value);
            string result = builder.ToString();

            return result;
        }
    }
}
