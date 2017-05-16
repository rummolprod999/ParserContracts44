using System;
using MySql.Data.MySqlClient;

namespace ParserContracts44
{
    public class ConnectToDb
    {
        public static MySqlConnection GetDBConnection()
        {
            // Connection String.
            String connString = "Server=" + "localhost" + ";Database=" + Program.Database
                                + ";User Id=" + Program.User + ";password=" + Program.Pass + ";CharSet=utf8";

            MySqlConnection conn = new MySqlConnection(connString);

            return conn;
        }
    }
}