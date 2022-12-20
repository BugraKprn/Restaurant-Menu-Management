using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;
using System.Reflection;

namespace System
{
    public static class ObjectXmlHelper
    {
        public static string GetXml(object obj)
        {
            return GetXml("Root", obj, null);
        }

        /// <summary>
        /// innert_text değerlerini value olarak kabul eder
        /// </summary>
        public static void GetObjectSimple(string xml, object output)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            GetObjectSimple(document.DocumentElement, output);
        }

        /// <summary>
        /// innert_text değerlerini value olarak kabul eder
        /// </summary>
        public static void GetObjectSimple(XmlNode objectNode, object output)
        {
            ProcessNodeSimple(objectNode, output);
        }
        
        private static void ProcessNodeSimple(XmlNode node, object output)
        {
            if (node.ChildNodes != null)
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    PropertyInfo p = output.GetType().GetProperty(child.Name);
                    object value = null;
                    if (p.PropertyType.IsEnum)
                        value = Enum.Parse(p.PropertyType, child.InnerText);
                    else
                        value = Convert.ChangeType(child.InnerText, p.PropertyType);

                    p.SetValue(output, value, null);
                }
            }            
        }

        public static void GetObject(string xml, object output, bool ignoreUndefined)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);

            ProcessNode(document.DocumentElement, output, ignoreUndefined);
        }


        private static void ProcessNode(XmlNode node, object output, bool ignoreUndefined)
        {
            if (node.Attributes != null)
            {
                foreach (XmlAttribute attr in node.Attributes)
                {
                    PropertyInfo p = output.GetType().GetProperty(attr.LocalName);
                    
                    if (p == null && ignoreUndefined) continue;

                    object value = null;
                    if (p.PropertyType.IsEnum)
                        value = Enum.Parse(p.PropertyType, attr.Value);
                    else
                        value = Convert.ChangeType(attr.Value, p.PropertyType);

                    p.SetValue(output, value, null);                    
                }
            }
            foreach (XmlNode child in node.ChildNodes)
            {
                bool isCollection = child.Attributes != null && child.Attributes["IsCollection"] != null;
                if (isCollection)
                {
                    PropertyInfo p = output.GetType().GetProperty(child.LocalName);
                    object value = p.GetValue(output, null);
                    
                    string itemTypeName = child.Attributes["ItemType"].Value;
                    Type itemType = Type.GetType(itemTypeName);
                    foreach (XmlNode item in child.ChildNodes)
                    {
                        if (itemType != typeof(string))
                        {
                            object itemValue = Activator.CreateInstance(itemType);
                            ((IList)value).Add(itemValue);
                            ProcessNode(item, itemValue, ignoreUndefined);
                        }
                        else
                        {
                            ((IList)value).Add(item.InnerText);
                        }
                    }
                }
                else if (child.HasChildNodes)
                {
                    PropertyInfo p = output.GetType().GetProperty(child.LocalName);
                    object value = Convert.ChangeType(child.InnerText, p.PropertyType);
                    ProcessNode(child, value, ignoreUndefined);
                }
                else
                {
                    PropertyInfo p = output.GetType().GetProperty(child.LocalName);
                    object value = null;
                    if (p.PropertyType.IsEnum)
                        value = Enum.Parse(p.PropertyType, child.Attributes["Value"].Value);
                    else
                        value = Convert.ChangeType(child.Attributes["Value"].Value, p.PropertyType);

                    p.SetValue(output, value, null);
                }
            }
        }

        private static string GetXml(string name, object obj, XmlWriter w)
        {
            if (obj == null) return "";

            Type t = obj.GetType();

            StringBuilder builder = null;
            if (w == null)
            {
                builder = new StringBuilder();
                w = XmlWriter.Create(builder);

            }

            w.WriteStartElement(name);
            if(obj as IEnumerable == null)
            {
                foreach (var property in t.GetProperties())
                {
                    if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
                    {
                        w.WriteStartElement(property.Name);
                        w.WriteAttributeString("Value", (property.GetValue(obj, null) ?? "").ToString());
                        w.WriteEndElement();
                    }
                    else
                    {
                        object value = property.GetValue(obj, null);
                        GetXml(property.Name, value, w);
                    }
                }
            }
            else
            {
                if (obj.GetType().IsGenericType)
                {
                    w.WriteAttributeString("IsCollection", "true");
                    w.WriteAttributeString("ItemType", obj.GetType().GetGenericArguments()[0].AssemblyQualifiedName);
                }
                foreach (object item in (IEnumerable)obj)
                {
                    GetXml("Item", item, w);
                }
            }
            w.WriteEndElement();

            if (builder != null)
            {
                w.Close();
                return builder.ToString();
            }

            return "";
        }
    }
}
