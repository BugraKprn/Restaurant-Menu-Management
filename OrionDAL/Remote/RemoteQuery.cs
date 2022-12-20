using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrionDAL.Remote
{
    public class RemoteQuery
    {
        public string Query { get; set; }

        public List<RemoteQueryParameter> Parameters { get; set; }

        public RemoteQuery()
        {
            Parameters = new List<RemoteQueryParameter>();
        }

        public void Add(string name, object value, string type)
        {
            this.Parameters.Add(new RemoteQueryParameter()
            {
                Name = name,
                Value = value.ToString(),
                Type = type,
            });
        }

        public static RemoteQuery BuildQuery(string query, Dictionary<string, object> namedParameters, object[] parameterValues)
        {
            RemoteQuery q = new RemoteQuery();
            q.Query = query;
            if (namedParameters != null)
            {
                foreach (var item in namedParameters)
                {
                    if (!item.GetType().IsEnum)
                    {
                        q.Add(item.Key, item.Value, item.Value.GetType().FullName);
                    }
                    else
                    {
                        q.Add(item.Key, ((int)item.Value).ToString(), typeof(int).FullName);
                    }
                }
            }
            if (parameterValues != null)
            {
                int i = 0;
                foreach (var item in parameterValues)
                {
                    if (!item.GetType().IsEnum)
                    {
                        q.Add("prm" + i, item, item.GetType().FullName);
                    }
                    else
                    {
                        q.Add("prm" + i, ((int)item).ToString(), typeof(int).FullName);
                    }
                    i++;
                }
            }

            return q;
        }
    }
}