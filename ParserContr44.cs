using System;
using System.IO;
using System.Net;
using System.Web;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ParserContracts44
{
    public class ParserContr44 : Parser
    {
        protected DataTable DtRegion;
        public readonly string[] years = new[] {"2016", "2017"};
        public readonly string[] except_file = new[] {"Failure", "contractProcedure", "contractCancel"};

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
                    GetListFileArch(v, PathParse, (string) row["conf"]);
                }
            }
        }

        public override void GetListFileArch(string Arch, string PathParse, string region)
        {
            string filea = "";
            string path_unzip = "";
            filea = GetArch(Arch, PathParse);
            if (!String.IsNullOrEmpty(filea))
            {
                path_unzip = Unzipped.Unzip(filea);
                if (path_unzip != "")
                {
                    if (Directory.Exists(path_unzip))
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(path_unzip);
                        FileInfo[] filelist = dirInfo.GetFiles();
                        foreach (var f in filelist)
                        {
                            try
                            {
                                Bolter(f.ToString(), region);
                            }
                            catch (Exception e)
                            {
                                Log.Logger("Не удалось обработать файл", f, filea);
                            }
                        }
                        dirInfo.Delete(true);
                    }
                }
            }
        }

        public void Bolter(string f, string region)
        {
            string file_lower = f.ToLower();
            if (!file_lower.EndsWith(".xml", StringComparison.Ordinal))
            {
                return;
            }

            if (except_file.Any(ex => file_lower.IndexOf(ex.ToLower(), StringComparison.Ordinal) != -1))
            {
                return;
            }

            try
            {
                ParsingXML(f, region);
            }
            catch (Exception e)
            {
                Log.Logger("Ошибка при парсинге xml", e, f);
            }
        }

        public void ParsingXML(string f, string region)
        {
            FileInfo fileInf = new FileInfo(f);
            if (fileInf.Exists)
            {
                using (StreamReader sr = new StreamReader(f, System.Text.Encoding.Default))
                {
                    string ftext;
                    ftext = sr.ReadToEnd();
                    ftext = ClearText.ClearString(ftext);
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(ftext);
                    string jsons = JsonConvert.SerializeXmlNode(doc);
                    JObject json = JObject.Parse(jsons);
                    /*WorkWithContract44 c = new WorkWithContract44(json, f, region);
                    c.Work44();*/
                    /*WorkWithContract44Parralel p = new WorkWithContract44Parralel(json, f, region);
                    p.Work44();*/
                    WorkWithContract44Async a = new WorkWithContract44Async(json, f, region);
                    a.Work44();
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
            foreach (var a in archtemp.Where(a => years.Any(t => a.IndexOf(t, StringComparison.Ordinal) != -1)))
            {
                using (MySqlConnection connect = ConnectToDb.GetDBConnection())
                {
                    connect.Open();
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
                            $"INSERT INTO {Program.Prefix}arhiv_contract SET arhiv = @archive, region =  @region";
                        MySqlCommand cmd1 = new MySqlCommand(add_arch, connect);
                        cmd1.Prepare();
                        cmd1.Parameters.AddWithValue("@archive", a);
                        cmd1.Parameters.AddWithValue("@region", RegionPath);
                        cmd1.ExecuteNonQuery();
                        arch.Add(a);
                    }
                }
            }

            return arch;
        }

        public override List<String> GetListArchPrev(string PathParse, string RegionPath)
        {
            List<String> arch = new List<string>();
            WorkWithFtp ftp = ClientFtp44();
            ftp.ChangeWorkingDirectory(PathParse);
            List<String> archtemp = ftp.ListDirectory();
            string serachd = $"{Program.LocalDate:yyyyMMdd}";
            foreach (var a in archtemp.Where(a => a.IndexOf(serachd, StringComparison.Ordinal) != -1))
            {
                string prev_a = $"prev_{a}";

                using (MySqlConnection connect = ConnectToDb.GetDBConnection())
                {
                    connect.Open();
                    string select_arch =
                        $"SELECT id FROM {Program.Prefix}arhiv_contract WHERE arhiv = @archive AND region =  @region";
                    MySqlCommand cmd = new MySqlCommand(select_arch, connect);
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@archive", prev_a);
                    cmd.Parameters.AddWithValue("@region", RegionPath);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    bool res_read = reader.HasRows;
                    reader.Close();
                    if (!res_read)
                    {
                        string add_arch =
                            $"INSERT INTO {Program.Prefix}arhiv_contract SET arhiv = @archive, region =  @region";
                        MySqlCommand cmd1 = new MySqlCommand(add_arch, connect);
                        cmd1.Prepare();
                        cmd1.Parameters.AddWithValue("@archive", prev_a);
                        cmd1.Parameters.AddWithValue("@region", RegionPath);
                        cmd1.ExecuteNonQuery();
                        arch.Add(a);
                    }
                }
            }

            return arch;
        }
    }
}