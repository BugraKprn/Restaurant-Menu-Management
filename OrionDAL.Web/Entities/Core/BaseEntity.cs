using Newtonsoft.Json;
using OrionDAL.ActiveRecord;
using OrionDAL.OAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrionDAL.Web.Entities.Core
{
    public abstract class BaseEntity : ActiveRecordBase
    {
        public static DateTime MinDate = new DateTime(1753, 1, 1, 0, 0, 0, 0);

        /// <summary>
        /// Multi-tenant uygulamalar için organizasyon key değeri
        /// </summary>
        public int OrganizationId { get; set; }

        /// <summary>
        /// Soft delete
        /// </summary>
        public bool Deleted { get; set; }

        public DateTime Created { get; set; }
        public long CreatedUserId { get; set; }

        public DateTime Updated { get; set; }
        public long UpdatedUserId { get; set; }

        [NonPersistent]
        public string GUID { get { if (guid.IsNullOrEmpty()) { guid = Guid.NewGuid().ToString(); } return guid; } set { guid = value; } }
        private string guid;
      

     

        public static void Required(BaseEntity entity, string errorMessage)
        {
            if (entity.NotExist())
                throw new Exception(errorMessage);
        }
        public static T ReadById<T>(int id)
        {
            return Persistence.Read<T>(id);
        }
        public class BaseRule
        {
            public Transaction RuleTransaction;

            protected virtual void ExecuteMethod() // *** protected olması önemli yanlışlıkla direkt çağrılmamalı!
            {
                throw new NotImplementedException("Bu iş kuralı oluşturulmamış.");
            }

            public virtual bool SupportTransaction { get { return true; } }

            public virtual void SetParameters(string json) // *** protected olması önemli yanlışlıkla direkt çağrılmamalı!
            {
                throw new NotImplementedException("Bu özellik desteklenmiyor.");
            }

            public BaseRule Execute()
            {
                BaseRule result = null;

                result = ExecuteLocal();

                return result;
            }

            private BaseRule ExecuteLocal()
            {
                if (this.RuleTransaction == null)
                    this.RuleTransaction = Transaction.Instance;

                if (SupportTransaction)
                {
                    RuleTransaction.Join(() =>
                    {
                        ExecuteMethod();
                    });
                }
                else
                {
                    ExecuteMethod();
                }

                return this;
            }

            //private BaseRule ExecuteRemote()
            //{
            //    var t = this.GetType();
            //    var typeName = t.Namespace + "." + t.Name + ", " + t.Assembly.ManifestModule.Name;
            //    if (typeName.EndsWith(".dll")) typeName = typeName.Substring(0, typeName.Length - 4);

            //    var json = JsonConvert.SerializeObject(this);

            //    var url = string.Format(ServisMerkezi.Instance.BulutAdresi, "ExecuteRule") + "&ruleType=" + typeName;
            //    var result = WebHelper.PostData(url, json);
            //    var remoteRule = (BaseRule)JsonConvert.DeserializeObject(result, this.GetType());

            //    return remoteRule;
            //}

            /// <summary>
            /// Web apisi üzerinden çağırılacak
            /// </summary>
            public static string CreateAndExecute(string ruleTypeName, string ruleJson)
            {
                var ruleType = Type.GetType(ruleTypeName);
                var rule = (BaseRule)JsonConvert.DeserializeObject(ruleJson, ruleType);

                rule.ExecuteLocal(); // sunucudayız, dolayısıyla lokali çağırmalıyız

                var result = JsonConvert.SerializeObject(rule);
                return result;
            }
        }
    }
}
