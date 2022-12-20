using Microsoft.Extensions.Configuration;
using OrionDAL.OAL;
using OrionDAL.OAL.Metadata;
using OrionDAL.Web.Entities.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Configuration = OrionDAL.OAL.ConfigurationOrion;

namespace OrionDAL.Web
{
    public class OrionWebApplication
    {
        private string _configurationPrefix = "";

        /// <summary>
        /// Dev ortamlarında true, prod'da false olabilir
        /// </summary>
        public bool DbSynchronization { get; set; }

        private readonly IConfiguration Configuration;
        static OrionWebApplication()
        {
           
        }

        public OrionWebApplication(IConfiguration configuration)
        {
            Configuration = configuration;
            var dev = GetDevKeyword();

            string dbType = GetSetting("DbType", dev);
            string connectionString = GetSetting("Connection", dev);
            var emailSettings = GetSetting("EmailSettings", dev);
            var tempStr = GetSetting("Dbsynchronization", dev);


            if (string.IsNullOrEmpty(dbType))
            {
                throw new ApplicationException("Konfigürasyon eksik, " + this.ConfigurationPrefix() + "-DbType bulunamadı.");
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ApplicationException("Konfigürasyon eksik, " + this.ConfigurationPrefix() + "-Connection bulunamadı.");
            }

            if (!string.IsNullOrEmpty(tempStr))
            {
                DbSynchronization = bool.Parse(tempStr);
            }

            this.InitializeOrm(dbType, connectionString, emailSettings);
        }

        public string GetDevKeyword()
        {
            var dev = "";
            var path = Environment.CurrentDirectory+"/DeveloperProfile.txt";
            if (File.Exists(path))
            {
                var devFile = (File.ReadAllText(path) ?? "").Trim();
                if (!string.IsNullOrEmpty(devFile)) dev = devFile;
            }
            return dev;
        }

        public string GetSetting(string setting, string dev)
        {
            string devKey = ConfigurationPrefix() + "-" + dev + "-" + setting;

            if (!string.IsNullOrEmpty(dev)
                && !string.IsNullOrEmpty(Configuration[devKey]))
            {
                return Configuration[devKey];
            }

            string key = ConfigurationPrefix() + "-" + setting;
            return Configuration[key];
        }

        public virtual void AfterLoad()
        {
        }

        public virtual string ConfigurationPrefix()
        {
            if (string.IsNullOrEmpty(_configurationPrefix))
            {
                /*** Recursive olmaması için burada getsetting çağrılmamalı! ***/
                var dev = GetDevKeyword();
                
                // ilk parametrenin hardcoded olması mecburiyetten
                if (!string.IsNullOrEmpty(Configuration["WebApplication-" + dev + "-Prefix"]))
                    _configurationPrefix = Configuration["WebApplication-" + dev + "-Prefix"];
                else
                    _configurationPrefix = Configuration["WebApplication-Prefix"];
            }

            return _configurationPrefix;
        }



       
        
        public virtual void Initialize()
        {
        }





        protected virtual void InitializeOrm(string dbType, string connectionString, string emailSettings)
        {
           ConfigurationOrion.SetValue("EmailSettings", emailSettings);
            ConfigurationOrion.SetValue("DbType", dbType);
            ConfigurationOrion.SetValue("Connection", connectionString);
            PersistenceStrategyProvider.SetDefault(new CustomPersistence());

            var ormPrefix = "Intranet-orm-api";
            var config = Configuration[ormPrefix].Split(",");
            foreach (var item in config)
            {
                var assembly = Assembly.Load(item);
                var entityTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(ActiveRecord.ActiveRecordBase))).ToArray();
                DataDictionary.Instance.AddEntities(entityTypes);

                if (this.DbSynchronization)
                {
                    try
                    {
                        OrionDAL.OAL.Schema.SchemaHelper.Syncronize(typeof(BaseEntity), true, Transaction.Instance, assembly);
                    }
                    catch (Exception)
                    {

                    }
                    
                }
            }
            //foreach (var configKey in  ConfigurationManager.AppSettings.AllKeys)
            //{
            //    if (configKey.StartsWith(ormPrefix))
            //    {

            //    }
            //}
        }
    }
}
