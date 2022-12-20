using System;
using System.Collections.Generic;
using System.Text;

namespace OrionDAL.OAL.Metadata
{
    public abstract class Validation
    {
        public abstract void Validate(string field, object value);
    }
}
