using System;
using MySql.Data.MySqlClient;

namespace ParserContracts44
{
    public static class ConnectToDb
    {
        public static string ConnectString { get; set; }

        static ConnectToDb()
        {
            ConnectString =
                $"Server={Program.Server};port={Program.Port};Database={Program.Database};User Id={Program.User};password={Program.Pass};CharSet=utf8;Convert Zero Datetime=True;default command timeout=3600;Connection Timeout=3600";
        }

        public static MySqlConnection GetDbConnection()
        {
            // Connection String.
            MySqlConnection conn = new MySqlConnection(ConnectString);

            return conn;
        }
    }
}