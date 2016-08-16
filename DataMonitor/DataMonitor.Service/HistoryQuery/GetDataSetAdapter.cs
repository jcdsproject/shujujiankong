using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMonitor.Service
{
    public class GetDataSetAdapter
    {
        public static DataSet GetdataSet(string connectionString, string sqlString)
        {
            DataSet dataSet = new DataSet();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                connection.Open();
                SqlCommand command = new SqlCommand(sqlString, connection);
                command.CommandType = CommandType.Text;
                adapter.SelectCommand = command;
                adapter.Fill(dataSet);
                connection.Close();
            }
            return dataSet;
        }
    }
}
