using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;

namespace UCA
{
    public class DBReader
    {
        readonly OleDbConnection connection;
        private DBReader (OleDbConnection connection)
        {
            this.connection = connection;
            this.connection.Open();
        }

        public DBReader(string connectionString) : this(new OleDbConnection(connectionString))
        {

        }

        private List<string> GetTableNamesList()
        {
            var tableNamesList = new List<string>();
            var dataTable = connection.GetSchema("Tables");
            foreach (DataRow row in dataTable.Rows)
            {
                string tableName = (string)row[2];
                if (!tableName.Contains("MSys") && !tableName.Contains("~"))
                    tableNamesList.Add(tableName);
            }
            return tableNamesList;
        }

        public DataSet GetDataSet()
        {
            DataSet dataSet = new DataSet();
            foreach (var tableName in GetTableNamesList())
            {
                var selectCommandText = $"SELECT * FROM [{tableName}]";
                OleDbDataAdapter dataAdapter = new OleDbDataAdapter(selectCommandText, connection);
                dataAdapter.Fill(dataSet, tableName.Replace("$", ""));
            }
            return dataSet;
        }

        ~DBReader() // деструктор
        {
            //this.connection.Close();
        }
    }
}
