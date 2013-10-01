using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;

namespace MyMVC
{
 
    public class ModelBase : IDisposable
    {

        public int IDFIELD;
        SqlTextBuilder sqlBuilder;
        string sqlText;
        DataTable dt;

        public ModelBase()
        {
            sqlBuilder = new SqlTextBuilder(this);
        }

        private void _createTableOnDatabase(DataCore dc)
        {
            try
            {
                sqlText = sqlBuilder.getTableCreateScript();
                dc.executeNonQuery(sqlText);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Insert(DataCore dc)
        {
            try
            {
                _createTableOnDatabase(dc);
                sqlText = sqlBuilder.getInsertScript();
                _addParams(dc);
                this.IDFIELD = dc.executeScalar(sqlText);

            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Update(DataCore dc,params string[] whereFieldArray)
        {
            try
            {
                _createTableOnDatabase(dc);
                sqlText = sqlBuilder.getUpdateScript();

                if (whereFieldArray.Length == 0)
                {
                    whereFieldArray = new string[1] { "IDFIELD" };
                }

                foreach (string value in whereFieldArray)
                {
                    sqlText += " AND " + value + "= @" + value;
                }

                _addParams(dc);
                dc.executeNonQuery(sqlText);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Delete(DataCore dc, params string[] whereFieldArray)
        {
            try
            {
                _createTableOnDatabase(dc);
                sqlText = "DELETE FROM " + this.GetType().Name + " WHERE 1=1 ";
                if (whereFieldArray.Length == 0)
                {
                    whereFieldArray = new string[1] { "IDFIELD" };
                }

                foreach (string value in whereFieldArray)
                {
                    sqlText += " AND " + value + "= @" + value;
                }

                _addParams(dc);
                dc.executeNonQuery(sqlText);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void _setAttribute( string attributeName, object value )
        {
            try
            {
                Type type = this.GetType();
                FieldInfo fieldInfo = type.GetField(attributeName);

                if (fieldInfo != null) 
                {
                    switch (fieldInfo.FieldType.Name)
                    { 
                        case "Int32":
                            fieldInfo.SetValue(this, Convert.ToInt32(value));
                            break;
                        case "String":
                            fieldInfo.SetValue(this, Convert.ToString(value));
                            break;
                        case "Double":
                            fieldInfo.SetValue(this, Convert.ToDouble(value));
                            break;
                        case "Datetime":
                            fieldInfo.SetValue(this, Convert.ToDateTime(value));
                            break;
                        case "Boolean":
                            fieldInfo.SetValue(this, Convert.ToBoolean(value));
                            break;
                    }
                }
            }
            catch (Exception myExp)
            {
                throw myExp;
            }  
        }

        public void validate()
        {
            try
            {
                Type type = this.GetType();
                FieldInfo[] fieldInfos = type.GetFields();

                foreach (FieldInfo fInfo in fieldInfos)
                {
                    if (fInfo.Name.EndsWith("Field"))
                    {
                        // validate edilecek attributelar var mı diye bak
                        if ( fInfo.GetCustomAttributes(false).Length != 0)
                        { 
                            //validate edilecek attributelar varsa
                            //hem attribute u hem property info yu validate attribute methoduna yolla
                            foreach (Attribute attribute in fInfo.GetCustomAttributes(false))
                            {
                                this.validateAttribute(attribute, fInfo);
                            }
                        }
                    
                    }

                }
 
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void validateAttribute(Attribute attr, FieldInfo fInfo)
        {
            try
            {
                object obj = fInfo.GetValue(this);

                if (attr is RequiredAttribute)
                {
                    if (obj == null)
                        throw new Exception("RequiredAttribute exception. Required Fields must not equal to null");

                    if (obj.ToString().Length == 0 )
                        throw new Exception("RequiredAttribute exception. string lengths  must not equal to zero");

                }

                if (attr is MaxLengthAttribute)
                {
                    var attributeData = fInfo.GetCustomAttributesData();
                    CustomAttributeData cd = attributeData[0];
                    int uzunluk = (int)cd.ConstructorArguments[0].Value;

                    string objString = (string)obj;

                    if (objString.Length > uzunluk)
                        throw new Exception("MaxLengthAttribute exception.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        
        }

        private void _addParams(DataCore dc)
        {
            try
            {
                foreach (FieldInfo fInfo in this.GetType().GetFields())
                {
                    if (fInfo.Name.EndsWith("FIELD"))
                    {
                        dc.addCommandParameter( "@"+fInfo.Name, fInfo.GetValue(this) );    
                    }
                
                }
            }
            catch (Exception)
            {
                throw;
            }
        
        }

        ~ModelBase()
        { 
            
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

    }


}
