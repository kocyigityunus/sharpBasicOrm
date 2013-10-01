using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace MyMVC
{

    public class DataCore : IDisposable
    {

        private SQLiteConnection connection;
        private SQLiteCommand command;
        private SQLiteDataAdapter dataAdapter;
        private SQLiteTransaction transaction;
        private string connectionString;

        public DataCore()
        {
            try
            {
                string path = System.Environment.CurrentDirectory;
                connectionString = path + @"\db\db.sqlite";
                // burada db nin adı olacak.
            }
            catch (Exception myExp)
            {
                throw myExp;
            }
        }

        public void connectAndInitialize()
        {
            try
            {
                connection = new SQLiteConnection();
                connection.ConnectionString = "Data Source = " + connectionString + " ; Version = 3; ";
                connection.Open();
                command = connection.CreateCommand();
            }
            catch (Exception myExp)
            {
                throw myExp;
            }
        }

        public void disconnectAndDispose()
        {
            if (connection != null && connection.State == ConnectionState.Open) { connection.Close(); }
            if (connection != null) { connection.Dispose(); }
            if (command != null) { command.Dispose(); }
            if (dataAdapter != null) { dataAdapter.Dispose(); }
            if (transaction != null) { transaction.Dispose(); }

            this.Dispose();
        }

        public void beginTransaction()
        {
            try
            {
                transaction = connection.BeginTransaction();
                command.Transaction = transaction;
            }
            catch (Exception myExp)
            {
                throw myExp;
            }
        }

        public void rollBackTransaction()
        {
            try
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
            }
            catch (Exception myExp)
            {
                throw myExp;
            }
        }

        public void commitTransaction()
        {

            try
            {
                if (transaction != null)
                {
                    transaction.Commit();
                }
            }
            catch (Exception myExp)
            {
                throw myExp;
            }
        }

        public void addCommandParameter(string name, object value)
        {
            if (value != null)
            {
                if (value.GetType() == typeof(DateTime))
                {
                    if (value.ToString() == "1.1.0001 00:00:00") value = DBNull.Value;
                    else if (value.ToString() == "01.01.0001 00:00:00") value = DBNull.Value;
                }
                else if (value.GetType() == typeof(string))
                {
                    if (string.IsNullOrWhiteSpace((string)value))
                    {
                        value = DBNull.Value;
                    }
                }
            }
            else value = DBNull.Value;
            command.Parameters.Add(new SQLiteParameter(name, value));
        }

        public DataTable fillDataTableText(string sqlText)
        {
            try
            {
                DataTable dt = new DataTable();
                dataAdapter = new SQLiteDataAdapter();
                command.CommandType = CommandType.Text;
                command.CommandText = sqlText;
                dataAdapter.SelectCommand = command;
                dataAdapter.Fill(dt);
                return dt;
            }
            catch (Exception myExp)
            {
                throw myExp;
            }
            finally
            {
                if (command != null) { command.Parameters.Clear(); }
            }
        }

        public int executeScalar(string sqlText)
        {
            try
            {
                command.CommandType = CommandType.Text;
                command.CommandText = sqlText;
                command.ExecuteScalar();
                return Convert.ToInt32(connection.LastInsertRowId);
            }
            catch (Exception myExp)
            {
                throw myExp;
            }
            finally
            {
                if (command != null) { command.Parameters.Clear(); }
            }

        }

        public void executeNonQuery(string sqlText)
        {
            try
            {
                command.CommandType = CommandType.Text;
                command.CommandText = sqlText;
                command.ExecuteNonQuery();
            }
            catch (Exception myExp)
            {
                throw myExp;
            }
            finally
            {
                if (command != null) { command.Parameters.Clear(); }
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
