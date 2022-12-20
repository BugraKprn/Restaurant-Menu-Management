using System;
using System.Collections.Generic;
using System.Text;

namespace OrionDAL.OAL.Metadata
{
    public interface IInsertInfo
    {
        int InsertUser { get; set; }
        DateTime InsertDate { get; set; }
    }
}
