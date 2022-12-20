using System;
using System.Collections.Generic;
using OrionDAL.OAL;
using OrionDAL.OAL.Metadata;
using System.Reflection;
using System.ComponentModel;
using Newtonsoft.Json;

namespace OrionDAL.ActiveRecord
{
    public class EntitySavedEventArgs : EventArgs
    {
        public bool Insert { get; set; }
        public string EntityType { get; set; }
        public int Id { get; set; }
        public string  TransactionId { get; set; }
    }

    public class SharpPointer<T> where T: new()
    {
        private int id = 0;
        [JsonDisplay]
        public int Id {
            get {
                if (value!=null)
                {
                    return (value as ActiveRecordBase).Id;
                }
                return id;
            }
            set {
                if (this.value != null)
                {
                    (this.value as ActiveRecordBase).Id = value;
                }
                else
                {
                    id = value;
                }
            }
        }
        public string Aciklama { get; set; }
        [JsonDisplay]
        public string Kodu { get; set; } // lookuplarýn çalýþmasý için eklendi
        [JsonDisplay]
        public string Adi { get; set; } // lookuplarýn çalýþmasý için eklendi

        private T value;

        [JsonIgnoreAttribute]
        public T Value
        {
            get
            {
                if (value == null)
                {
                    value = new T();
                    (value as ActiveRecordBase).Id = this.id;
                }
                return value;
            }
            set { this.value = value; }
        }

        [JsonIgnoreAttribute]
        public Type PointerType { get { return typeof(T); } }




        public virtual bool Exist()
        {
            return Id != 0;
        }

        public virtual bool NotExist()
        {
            return !Exist();
        }
    }

    [FieldDefinition(TypeName = "Int32", IsFiltered = false)] //For 'Id' 
    [EntityDefinition(IdMethod = IdMethod.Identity, OptimisticLockField="RowVersion")]
    public class ActiveRecordBase : INotifyPropertyChanged
    {
        public static EventHandler<EntitySavedEventArgs> EntitySaved;

        protected bool isRead = false;

        private Int32 id;

        [JsonDisplay]
        public Int32 Id
        {
            get { return id; }
            set
            {
                //--if (value == System.DBNull.Value) value = null;
                id = value;
                isRead = false;
                OnPropertyChanged("Id");
            }
        }

        private byte rowVersion;
        public byte RowVersion
        {
            get { return rowVersion; }
            set { rowVersion = value; }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            //Keep track of changed properties.
            // Fire an event for binding, with propertyName included in event parameter.
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        //public event PropertyChangedEventHandler OnPropertyChanged;

        public virtual void Validate()
        {
            //Loop each field
            //  if field has definition and validations in it
            //  then call validate method
            Type type = GetType();
            PropertyInfo[] properties = PersistenceStrategyProvider.FindStrategyFor(type).GetPersistentProperties(type);

            if (properties != null)
            {
                foreach (PropertyInfo property in properties)
                {
                    FieldDefinitionAttribute definition = DataDictionary.Instance.GetFieldDefinition(type.Name + "." + property.Name);
                    if(definition==null)
                        throw new Exception("Definition okunamadý: " + property.Name);
                    object value = GetValue(property.Name);

                    //Length validation for string fields
                    if (definition.TypeName == typeof(string).Name)
                    {
                        if (definition.Length > 0 // sýfýr ise sýnýrsýz yazýlabilir
                            && value != null && definition.Length < value.ToString().Length) //! value can be enumeration so call toString
                        {
                            throw new Exception(string.Format(
                                "({2})\n '{0}' Bu alana en fazla {1} adet harf yazýlabilir.",
                                property.Name, definition.Length, type.Name));
                        }
                    }

                    //Required validation
                    if (definition.IsRequired)
                        ValidateRequiredField(value, definition.Text);
                }
            }
        }

        protected void ValidateRequiredField(object value, string fieldName)
        {
            if (value==null)
                throw new Exception("Lütfen '" + fieldName + "' Alanýný Boþ Býrakmayýnýz.");
            
            Type type = value.GetType();
            if (type.IsSubclassOf(typeof(ActiveRecordBase))
                && !((ActiveRecordBase)value).Exist())
            {
                throw new Exception("Lütfen '" + fieldName + "' Alanýný Boþ Býrakmayýnýz.");
            }
            else if (type == typeof(string) && string.IsNullOrEmpty((string)value))
            {
                throw new Exception("Lütfen '" + fieldName + "' Alanýný Boþ Býrakmayýnýz.");
            }
            else if (string.IsNullOrEmpty(value.ToString()))
            {
                throw new Exception("Lütfen '" + fieldName + "' Alanýný Boþ Býrakmayýnýz.");
            }
        }

        public virtual void Insert()
        {
            //Validate();
            Transaction.Instance.Join(delegate()
            {
                Id = Convert.ToInt32(Persistence.Insert(this));
                InsertAllChilds();
            }, delegate()
            {
                this.Id = 0;
            },
            delegate()
            {
                if (EntitySaved != null)
                    EntitySaved(null, new EntitySavedEventArgs() { Insert = true, EntityType = this.GetType().Name, Id = this.Id });
            },
            false, -1);
        }

        public virtual int Update()
        {
            Validate();
            int i = -1;
            Transaction.Instance.Join(delegate()
            {
                i = Persistence.Update(this);
                if (i == 1)
                {
                    DeleteAllChilds();
                    InsertAllChilds();
                }
                else
                    throw new ApplicationException("Bu kayýt sizden önce deðiþtirilmiþ veya silinmiþ. $001");

            },
            null,
            delegate()
            {
                RowVersion = (byte)(RowVersion == 255 ? 0 : RowVersion + 1);

                if (EntitySaved != null)
                    EntitySaved(null, new EntitySavedEventArgs() { Insert = false, EntityType = this.GetType().Name, Id = this.Id });
            },
            false,
            -1);
            return i;
        }

        public virtual void Save()
        {
            Validate();
            if (Exist())
                Update();
            else
                Insert();
        }

        public virtual void Delete()
        {
            Type typeOfInstance = this.GetType();
            object keyValue = Id;
            bool throwException = true;

            Transaction.Instance.Join(delegate()
            {
                DeleteAllChilds();
                Persistence.DeleteByKey(typeOfInstance, keyValue, throwException);
            });
        }

        public virtual Type GetChildType()
        {
            return null;
        }

        public virtual string GetFKName()
        {
            return null;
        }

        public virtual void DeleteAllChilds()
        {
            if (GetChildType() == null || string.IsNullOrEmpty(GetFKName())) return;
            string sql = "delete from " + GetChildType().Name + " where " + GetFKName() + "='" + Id + "'";

            Transaction.Instance.ExecuteNonQuery(sql);
        }

        public virtual void InsertAllChilds()
        {
            if (GetChildType() == null || string.IsNullOrEmpty(GetFKName())) return;

            string parentProperty = GetFKName().Substring(0, GetFKName().IndexOf("_"));
            foreach (ActiveRecordBase child in Details)
            {
                child.SetValue(parentProperty, this);
                child.Insert();
            }
        }

        public string GetName()
        {
            return this.GetType().Name;
        }

        public object GetValue(string fieldName)
        {
            return ReflectionHelper.GetValue(this, fieldName);
        }

        public object GetValueString(string fieldName)
        {
            object value;

            value = ReflectionHelper.GetValue(this, fieldName);
            if (value is DateTime)
                value = ((DateTime)value).ToString("dd.MM.yyyy");
            
            return value;
        }

        public void SetValue(string fieldName, object value)
        {
            ReflectionHelper.SetValue(this, fieldName, value);
        }

        public virtual bool Exist()
        {
            return id != 0;

            /*if (Id == null) return false;

            if (Id is string && string.IsNullOrEmpty((string)Id)) return false;
            if (Id is Int32 && ((Int32)Id) <= 0) return false;
            if (Id is Int64 && ((Int64)Id) <= 0) return false;
            
            return true;*/
        }

        public virtual bool NotExist()
        {
            return !Exist();
        }

        [JsonIgnore]
        private List<ActiveRecordBase> Details = new List<ActiveRecordBase>();

        public void SetChilds(List<ActiveRecordBase> Details)
        {
            this.Details = Details;
        }

        public virtual void Read()
        {
            Read(false);
        }

        public virtual void Read(bool forceRead)
        {
            if (!forceRead)
            {
                if (isRead) return;
                isRead = true;
            }
            Persistence.Read(this, Id);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}