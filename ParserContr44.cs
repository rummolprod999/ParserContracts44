using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using FluentFTP;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ParserContracts44
{
    public class ParserContr44 : Parser
    {
        protected DataTable DtRegion;
        public readonly string[] ExceptFile = new[] {"Failure", "contractProcedure", "contractCancel", "contractAvailableForElAct"};

        public ParserContr44(string arg) : base(arg)
        {
        }

        public override void Parsing()
        {
            DtRegion = GetRegions();
            foreach (DataRow row in DtRegion.Rows)
            {
                List<String> arch = new List<string>();
                string pathParse = "";
                string regionPath = (string) row["path"];

                switch (Program.Periodparsing)
                {
                    case TypeArguments.Last44:
                        pathParse = $"/fcs_regions/{regionPath}/contracts/";
                        arch = GetListArchLast(pathParse, regionPath);
                        break;
                    case TypeArguments.Curr44:
                        pathParse = $"/fcs_regions/{regionPath}/contracts/currMonth/";
                        arch = GetListArchCurr(pathParse, regionPath);
                        break;
                    case TypeArguments.Prev44:
                        pathParse = $"/fcs_regions/{regionPath}/contracts/prevMonth/";
                        arch = GetListArchPrev(pathParse, regionPath);
                        break;
                }

                if (arch.Count == 0)
                {
                    Log.Logger("Не получили список архивов по региону", row["path"]);
                    continue;
                }

                foreach (var v in arch)
                {
                    GetListFileArch(v, pathParse, (string) row["conf"]);
                }
            }
        }

        public override void GetListFileArch(string arch, string pathParse, string region)
        {
            string filea = "";
            string pathUnzip = "";
            filea = GetArch44(arch, pathParse);
            if (!String.IsNullOrEmpty(filea))
            {
                pathUnzip = Unzipped.Unzip(filea);
                if (pathUnzip != "")
                {
                    if (Directory.Exists(pathUnzip))
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(pathUnzip);
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
            string fileLower = f.ToLower();
            if (!fileLower.EndsWith(".xml", StringComparison.Ordinal))
            {
                return;
            }

            if (ExceptFile.Any(ex => fileLower.IndexOf(ex.ToLower(), StringComparison.Ordinal) != -1))
            {
                return;
            }

            try
            {
                ParsingXml(f, region);
            }
            catch (Exception e)
            {
                Log.Logger("Ошибка при парсинге xml", e, f);
            }
        }

        public void ParsingXml(string f, string region)
        {
            FileInfo fileInf = new FileInfo(f);
            if (fileInf.Exists)
            {
                using (StreamReader sr = new StreamReader(f, Encoding.Default))
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
                    WorkWithContract44Parralel p = new WorkWithContract44Parralel(json, f, region);
                    p.Work44();
                    /*WorkWithContract44Async a = new WorkWithContract44Async(json, f, region);
                    a.Work44();*/
                }
            }
        }


        public override List<String> GetListArchLast(string pathParse, string regionPath)
        {
            List<string> archtemp = GetListFtp(pathParse, Wftp44);
            return archtemp.Where(a => Program.Years.Any(t => a.IndexOf(t, StringComparison.Ordinal) != -1)).ToList();
        }

        public override List<String> GetListArchCurr(string pathParse, string regionPath)
        {
            List<String> arch = new List<string>();
            List<string> archtemp = GetListFtp(pathParse, Wftp44);
            foreach (var a in archtemp
                .Where(a => Program.Years.Any(t => a.IndexOf(t, StringComparison.Ordinal) != -1)))
            {
                using (MySqlConnection connect = ConnectToDb.GetDbConnection())
                {
                    connect.Open();
                    string selectArch =
                        $"SELECT id FROM {Program.Prefix}arhiv_contract WHERE arhiv = @archive AND region =  @region";
                    MySqlCommand cmd = new MySqlCommand(selectArch, connect);
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@archive", a);
                    cmd.Parameters.AddWithValue("@region", regionPath);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    bool resRead = reader.HasRows;
                    reader.Close();
                    if (!resRead)
                    {
                        string addArch =
                            $"INSERT INTO {Program.Prefix}arhiv_contract SET arhiv = @archive, region =  @region";
                        MySqlCommand cmd1 = new MySqlCommand(addArch, connect);
                        cmd1.Prepare();
                        cmd1.Parameters.AddWithValue("@archive", a);
                        cmd1.Parameters.AddWithValue("@region", regionPath);
                        cmd1.ExecuteNonQuery();
                        arch.Add(a);
                    }
                }
            }

            return arch;
        }

        public override List<String> GetListArchPrev(string pathParse, string regionPath)
        {
            List<String> arch = new List<string>();
            List<string> archtemp = GetListFtp(pathParse, Wftp44);
            /*FtpClient ftp = ClientFtp44();*/
            //string serachd = $"{Program.LocalDate:yyyyMMdd}";
            foreach (var a in archtemp)
            {
                string prevA = $"prev_{a}";

                using (MySqlConnection connect = ConnectToDb.GetDbConnection())
                {
                    connect.Open();
                    string selectArch =
                        $"SELECT id FROM {Program.Prefix}arhiv_contract WHERE arhiv = @archive AND region =  @region";
                    MySqlCommand cmd = new MySqlCommand(selectArch, connect);
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@archive", prevA);
                    cmd.Parameters.AddWithValue("@region", regionPath);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    bool resRead = reader.HasRows;
                    reader.Close();
                    if (!resRead)
                    {
                        string addArch =
                            $"INSERT INTO {Program.Prefix}arhiv_contract SET arhiv = @archive, region =  @region";
                        MySqlCommand cmd1 = new MySqlCommand(addArch, connect);
                        cmd1.Prepare();
                        cmd1.Parameters.AddWithValue("@archive", prevA);
                        cmd1.Parameters.AddWithValue("@region", regionPath);
                        cmd1.ExecuteNonQuery();
                        arch.Add(a);
                    }
                }
            }

            return arch;
        }

        
    }
}