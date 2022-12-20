using OrionDAL.OAL;
using OrionDAL.Web.Entities.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace OrionDAL.Web
{
    public class CustomPersistence : BasicPersistence2
    {
        public override void Fill(object entity, DataRow dataRow)
        {
            base.Fill(entity, dataRow);

            //Burda projemizde BaseEntity diye bir class olduğunu ve bu class'ın
            //tüm entitylerin atası olduğunu varsayıyoruz.
            BaseEntity e = entity as BaseEntity;
            if (e != null)
            {
                //e.AfterFill(dataRow);
            }
        }
    }
}
