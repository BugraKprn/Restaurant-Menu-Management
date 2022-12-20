using OrionDAL.OAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrionDAL
{
    public class OrionRule
    {
        public virtual bool JoinTransaction { get { return true; } }
        protected virtual void Execute()
        {
            throw new NotImplementedException("Rule Execute metodu oluşturulmamış");
        }

        public virtual void Parameters(string jsonPrms)
        {
            throw new NotImplementedException("Rule Parameters metodu oluşturulmamış");
        }

        public OrionRule RunRule()
        {
            if(false)//if (OrionServis.Instance.Remote)
            {
                return RunRemote();
            }
            else
            {
                return RunLocal();
            }
        }

        private OrionRule RunRemote()
        {
            return null;
        }

        private OrionRule RunLocal()
        {
            if (JoinTransaction)
            {
                Transaction.Instance.Join(() =>
                {
                    Execute();
                });
            }
            else
            {
                Execute();
            }

            return this;
        }
    }
}
