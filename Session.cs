using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MyMVC
{
    public class Session
    {
        public static ClassType FindObject<ClassType>(DataCore dc, int primaryKey)
        {
            try
            {
                object instance = null;
                Type t = typeof(ClassType);
                ConstructorInfo cTor = t.GetConstructor(System.Type.EmptyTypes);
                string sqlText;
                System.Data.DataTable dt;

                if (cTor != null)
                {
                    instance = cTor.Invoke(null);
                }

                sqlText = "SELECT * FROM " + t.Name + " WHERE IDFIELD = " + primaryKey.ToString();
                dt = dc.fillDataTableText(sqlText);

                if (dt.Rows.Count == 0)
                {
                    throw new ObjectNotFoundException();
                }

                foreach (System.Data.DataColumn dataColumn in dt.Columns)
                {
                    _setAttribute(instance, dataColumn.ColumnName, dataColumn.DataType, dt.Rows[0][dataColumn.ColumnName].ToString());
                }

                return (ClassType)instance;

            }
            catch (Exception)
            {
                throw;
            }

        }

        public static ClassType[] FindObjects<ClassType>(DataCore dc, string sqlText)
        {
            try
            {
                int count = 0;

                ClassType[] dizi = null;
                Type t = typeof(ClassType);
                ConstructorInfo cTor = t.GetConstructor(System.Type.EmptyTypes);
                System.Data.DataTable dt;

                if (cTor == null)
                {
                    throw new ConstrucutorNullException();
                }

                dt = dc.fillDataTableText(sqlText);

                if (dt.Rows.Count == 0)
                {
                    throw new ObjectNotFoundException();
                }

                dizi = new ClassType[dt.Rows.Count];

                foreach (System.Data.DataRow dataRow in dt.Rows)
                {
                    object instance = cTor.Invoke(null);

                    foreach (System.Data.DataColumn dataColumn in dt.Columns)
                    {
                        _setAttribute(instance, dataColumn.ColumnName, dataColumn.DataType, dt.Rows[count][dataColumn.ColumnName].ToString());
                    }

                    dizi[count] = (ClassType)instance;
                    count++;
                }

                return dizi;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static void _setAttribute(object instance, string attributeName, Type dataType, string data)
        {
            try
            {
                Type type = instance.GetType();
                FieldInfo fieldInfo = type.GetField(attributeName);

                if (fieldInfo != null)
                {
                    switch (dataType.Name)
                    {
                        case "Int64":
                            fieldInfo.SetValue(instance, Convert.ToInt32(data));
                            break;
                        case "Int32":
                            fieldInfo.SetValue(instance, Convert.ToInt32(data));
                            break;
                        case "String":

                            if (_tarihMi(fieldInfo))
                            {
                                // dogru ise tarih demektir.
                                fieldInfo.SetValue(instance, Convert.ToDateTime(data));
                            }
                            else
                            {
                                // yanlış ise tarih değil demektir.
                                fieldInfo.SetValue(instance, Convert.ToString(data));
                            }
                            break;
                        case "Double":
                            fieldInfo.SetValue(instance, Convert.ToDouble(data));
                            break;
                        case "Datetime":
                            fieldInfo.SetValue(instance, Convert.ToDateTime(data));
                            break;
                        case "Boolean":

                            if (data == "True")
                            {
                                fieldInfo.SetValue(instance, true);
                            }
                            else
                            {
                                fieldInfo.SetValue(instance, false);
                            }

                            break;
                    }
                }
            }
            catch (Exception myExp)
            {
                throw myExp;
            }
        }

        private static bool _tarihMi(FieldInfo fInfo)
        {
            try
            {
                // max length attribute une göre denetle

                foreach (Attribute attr in fInfo.GetCustomAttributes(false))
                {
                    if (attr is MaxLengthAttribute)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }



}
