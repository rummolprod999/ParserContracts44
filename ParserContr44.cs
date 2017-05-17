using System;
using System.IO;
using System.Net;
using System.Web;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;

namespace ParserContracts44
{
    public class ParserContr44 : Parser
    {
        protected DataTable DtRegion;
        public readonly string[] years = new[] {"2016", "2017"};

        public ParserContr44(string arg) : base(arg)
        {
        }

        public override void Parsing()
        {
            DtRegion = GetRegions();
            foreach (DataRow row in DtRegion.Rows)
            {
                List<String> arch = new List<string>();
                string PathParse = "";
                string RegionPath = (string) row["path"];

                switch (Program.Periodparsing)
                {
                    case TypeArguments.Last:
                        PathParse = $"/fcs_regions/{RegionPath}/contracts/";
                        arch = GetListArchLast(PathParse, RegionPath);
                        break;
                    case TypeArguments.Curr:
                        PathParse = $"/fcs_regions/{RegionPath}/contracts/currMonth/";
                        arch = GetListArchCurr(PathParse, RegionPath);
                        break;
                    case TypeArguments.Prev:
                        PathParse = $"/fcs_regions/{RegionPath}/contracts/prevMonth/";
                        arch = GetListArchPrev(PathParse, RegionPath);
                        break;
                }

                if (arch.Capacity == 0)
                {
                    Log.Logger("Не получили список архивов по региону", row["path"]);
                }
                foreach (var v in arch)
                {
                    Console.WriteLine(v);
                }
            }
        }

        public override List<String> GetListArchLast(string PathParse, string RegionPath)
        {
            List<String> arch = new List<string>();

            WorkWithFtp ftp = ClientFtp44();
            ftp.ChangeWorkingDirectory(PathParse);
            List<String> archtemp = ftp.ListDirectory();
            foreach (var a in archtemp)
            {
                if (years.Any(t => a.IndexOf(t, StringComparison.Ordinal) != -1))
                {
                    arch.Add(a);
                }
            }

            return arch;
        }

        public override List<String> GetListArchCurr(string PathParse, string RegionPath)
        {
            List<String> arch = new List<string>();

            WorkWithFtp ftp = ClientFtp44();
            ftp.ChangeWorkingDirectory(PathParse);
            List<String> archtemp = ftp.ListDirectory();
            foreach (var a in archtemp)
            {
                if (years.Any(t => a.IndexOf(t, StringComparison.Ordinal) != -1))
                {
                    using (MySqlConnection connect = ConnectToDb.GetDBConnection())
                    {
                        string select_arch =
                            $"SELECT id FROM {Program.Prefix}arhiv_contract WHERE arhiv = @archive AND region =  @region";
                        MySqlCommand cmd = new MySqlCommand(select_arch, connect);
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@archive", a);
                        cmd.Parameters.AddWithValue("@region", RegionPath);
                        MySqlDataReader reader = cmd.ExecuteReader();
                        bool res_read = reader.HasRows;
                        reader.Close();
                        if (!res_read)
                        {
                            string add_arch =
                                $"SELECT id FROM {Program.Prefix}arhiv_contract WHERE arhiv = @archive AND region =  @region";
                            MySqlCommand cmd1 = new MySqlCommand(add_arch, connect);
                            cmd1.Prepare();
                            cmd1.Parameters.AddWithValue("@archive", a);
                            cmd1.Parameters.AddWithValue("@region", RegionPath);
                            cmd1.ExecuteNonQuery();
                            arch.Add(a);
                        }
                    }
                }
            }

            return arch;
        }

        public override List<String> GetListArchPrev(string PathParse, string RegionPath)
        {
            List<String> arch = new List<string>();

            return arch;
        }
    }
}