using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrionDAL.Remote
{
    public class RemoteReadParameter
    {
        public string EntityType { get; set; }

        public List<Condition> Conditions { get; set; }

        public RemoteReadParameter()
        {
            Conditions = new List<Condition>();
        }
    }
}
