using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MyMVC
{
    class SqlTextBuilder : IDisposable
    {
        StringBuilder sqlBuilder;
        Object obj;
        Type t;
        FieldInfo[] fInfos;

        public SqlTextBuilder(object obj)
        {
            this.obj = obj;
            this.t = obj.GetType();
            this.fInfos = t.GetFields();
            this.sqlBuilder = new StringBuilder();
        }

        public string getTableCreateScript() 
        {
            try
            {
                int count = 0;
                int maxCount = this._getFieldNumber();

                sqlBuilder.Clear();
                sqlBuilder.Append(@"CREATE TABLE IF NOT EXISTS 'main'.'"+  this.t.Name  +"' ('IDFIELD' INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,  ");

                foreach( FieldInfo f in fInfos )
                {
                    if (f.Name.EndsWith("FIELD") && f.Name != "IDFIELD" )
                    {
                        count++;

                        sqlBuilder.Append(" '" + f.Name + "' ");

                        switch (f.FieldType.Name)
                        {
                            case "Int32":
                                sqlBuilder.Append(" INTEGER ");
                                break;
                            case "String":
                                if (this._getStringLength(f) == 0)
                                {
                                    throw new MaxLengthAttributeNotFoundException();
                                }
                                break;
                            case "Double":
                                sqlBuilder.Append(" DOUBLE ");
                                break;
                            case "Datetime":
                                sqlBuilder.Append(" DATETIME ");
                                break;
                            case "Boolean":
                                sqlBuilder.Append(" BOOL ");
                                break;
                        }

                        foreach (Attribute attr in f.GetCustomAttributes(false) )
                        {
                            if (attr is MaxLengthAttribute && f.FieldType.Name == "String")
                            {
                                var attributeData = f.GetCustomAttributesData();
                                CustomAttributeData cd = attributeData[0];
                                int length = (int)cd.ConstructorArguments[0].Value;
                                sqlBuilder.Append(" VARCHAR( "+length+" ) ");
                            }

                            else if (attr is RequiredAttribute)
                            {
                                sqlBuilder.Append(" NOT NULL ");
                            }
                        }

                        if (count == maxCount-1)
                        {
                            sqlBuilder.Append(" ) ");
                        }
                        else
                        {
                            sqlBuilder.Append(" , ");
                        }
                    }

                    else { }

                    //CREATE  TABLE  IF NOT EXISTS "main"."tableName" ("c1" INTEGER PRIMARY KEY  AUTOINCREMENT  NOT NULL , "c2" VARCHAR(200) NOT NULL , "c3" DOUBLE, "c4" DATETIME, "c5 " BOOL)

                
                }

                return sqlBuilder.ToString();
            }
            catch (Exception)
            {
                throw;
            }

        
        }

        public string getInsertScript()
        {
            try
            {
                int count = 0;
                int maxCount = _getFieldNumber();

                sqlBuilder.Clear();
                sqlBuilder.Append(" INSERT INTO "+ t.Name +" ( ");

                foreach (FieldInfo fInfo in fInfos)
                {
                    if ( fInfo.Name.EndsWith("FIELD") && fInfo.Name != "IDFIELD" )
                    {
                        count++;
                        sqlBuilder.Append(fInfo.Name);

                        if (count == maxCount-1)
                        {
                            sqlBuilder.Append(" ) ");
                        }
                        else
                        {
                            sqlBuilder.Append(" , ");
                        }

                    }
                }

                sqlBuilder.Append(" VALUES ( ");
                count = 0;

                foreach (FieldInfo fInfo in fInfos)
                {
                    if (fInfo.Name.EndsWith("FIELD") && fInfo.Name != "IDFIELD" )
                    {
                        count++;
                        sqlBuilder.Append( "@" + fInfo.Name);

                        if (count == maxCount-1)
                        {
                            sqlBuilder.Append(" ) ");
                        }
                        else
                        {
                            sqlBuilder.Append(" , ");
                        }

                    }
                }

                return sqlBuilder.ToString();

            }
            catch (Exception)
            {
                throw;
            }
        
        }

        public string getUpdateScript()
        {
            try
            {
                int count = 0;
                int maxCount = this._getFieldNumber();

                sqlBuilder.Clear();
                sqlBuilder.Append(" UPDATE " + t.Name + " SET " );

                foreach (FieldInfo f in fInfos)
                {
                    if (f.Name.EndsWith("FIELD") && f.Name != "IDFIELD")
                    {
                        count++;
                        sqlBuilder.Append( f.Name +" = @" + f.Name + "  "  );

                        if (count != maxCount - 1)
                        {
                            sqlBuilder.Append(",");
                        }
                        else { }
                    
                    }
                
                }

                sqlBuilder.Append(" WHERE 1=1 ");

                return sqlBuilder.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private int _getFieldNumber()
        {
            try
            {
                int sayi = 0;
                foreach (FieldInfo f in fInfos) {
                    if (f.Name.EndsWith("FIELD") )
                    {
                        sayi++;
                    }
                }
                return sayi;
            }
            catch (Exception)
            {
                throw;
            }
        
        }

        private int _getStringLength(FieldInfo fieldInfo)
        {
            try
            {
                int length = 0;
                foreach (Attribute attr in fieldInfo.GetCustomAttributes(false))
                {
                    if (attr is MaxLengthAttribute)
                    {

                        var attributeData = fieldInfo.GetCustomAttributesData();
                        CustomAttributeData cd = attributeData[0];
                        length = (int)cd.ConstructorArguments[0].Value;
                    }
                
                }
                return length;
            }
            catch (Exception)
            { 
                throw;
            }
        
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
