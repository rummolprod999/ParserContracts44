using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ParserContracts44
{
    public class ParserEacts44 : Parser
    {
        protected DataTable DtRegion;
        public readonly string[] ExceptFile = new String[0];

        public readonly string[] paths = new[] { "customerDocs", "supplierTitles" };

        public ParserEacts44(string a) : base(a)
        {
        }

        public override void Parsing()
        {
            DtRegion = GetRegions();
            foreach (var path in paths)
            {
                try
                {
                    parse(path);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Log.Logger("Не удалось обработать регион ", path);
                }
            }
        }

        private void parse(string path)
        {
            foreach (DataRow row in DtRegion.Rows)
            {
                var arch = new List<string>();
                var pathParse = "";
                var regionPath = (string)row["path"];

                switch (Program.Periodparsing)
                {
                    case TypeArguments.LastEacts44:
                        pathParse = $"/fcs_regions/{regionPath}/eacts/{path}/";
                        arch = GetListArchLast(pathParse, regionPath);
                        break;
                    case TypeArguments.CurrEacts44:
                        pathParse = $"/fcs_regions/{regionPath}/eacts/{path}/currMonth/";
                        arch = GetListArchCurr(pathParse, regionPath);
                        break;
                    case TypeArguments.PrevEacts44:
                        pathParse = $"/fcs_regions/{regionPath}/eacts/{path}/prevMonth/";
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
                    try
                    {
                        GetListFileArch(v, pathParse, (string)row["conf"]);
                    }
                    catch (Exception e)
                    {
                        Log.Logger(e);
                        Console.WriteLine(e);
                    }
                }
            }
        }

        public override void GetListFileArch(string arch, string pathParse, string region)
        {
            var filea = "";
            var pathUnzip = "";
            filea = GetArch44(arch, pathParse);
            if (!String.IsNullOrEmpty(filea))
            {
                pathUnzip = Unzipped.Unzip(filea);
                if (pathUnzip != "")
                {
                    if (Directory.Exists(pathUnzip))
                    {
                        var dirInfo = new DirectoryInfo(pathUnzip);
                        var filelist = dirInfo.GetFiles();
                        foreach (var f in filelist)
                        {
                            try
                            {
                                Bolter(f.ToString(), region);
                            }
                            catch (Exception e)
                            {
                                Log.Logger("Не удалось обработать файл", f, filea);
                                Console.WriteLine(e);
                            }
                        }

                        dirInfo.Delete(true);
                    }
                }
            }
        }

        public void Bolter(string f, string region)
        {
            var fileLower = f.ToLower();
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
            var fileInf = new FileInfo(f);
            if (fileInf.Exists)
            {
                var srcEncoding = Encoding.GetEncoding(1251);
                using (var sr = new StreamReader(f, srcEncoding))
                {
                    string ftext;
                    ftext = sr.ReadToEnd();
                    ftext = ClearText.ClearString(ftext);
                    var doc = new XmlDocument();
                    doc.LoadXml(ftext);
                    var jsons = JsonConvert.SerializeXmlNode(doc);
                    var json = JObject.Parse(jsons);
                    var p = new Eacts44(json, f, region);
                    p.Work44();
                }
            }
        }


        public override List<String> GetListArchLast(string pathParse, string regionPath)
        {
            var archtemp = GetListFtp(pathParse, Wftp44);
            return archtemp.Where(a => Program.Years.Any(t => a.IndexOf(t, StringComparison.Ordinal) != -1)).ToList();
        }

        public override List<String> GetListArchCurr(string pathParse, string regionPath)
        {
            var arch = new List<string>();
            var archtemp = GetListFtp(pathParse, Wftp44);
            foreach (var a in archtemp
                         .Where(a => Program.Years.Any(t => a.IndexOf(t, StringComparison.Ordinal) != -1)))
            {
                using (var connect = ConnectToDb.GetDbConnection())
                {
                    connect.Open();
                    var selectArch =
                        $"SELECT id FROM {Program.Prefix}arhiv_eacts44 WHERE arhiv = @archive AND region =  @region";
                    var cmd = new MySqlCommand(selectArch, connect);
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@archive", a);
                    cmd.Parameters.AddWithValue("@region", regionPath);
                    var reader = cmd.ExecuteReader();
                    var resRead = reader.HasRows;
                    reader.Close();
                    if (!resRead)
                    {
                        var addArch =
                            $"INSERT INTO {Program.Prefix}arhiv_eacts44 SET arhiv = @archive, region =  @region";
                        var cmd1 = new MySqlCommand(addArch, connect);
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
            var arch = new List<string>();
            var archtemp = GetListFtp(pathParse, Wftp44);
            /*FtpClient ftp = ClientFtp44();*/
            //string serachd = $"{Program.LocalDate:yyyyMMdd}";
            foreach (var a in archtemp)
            {
                var prevA = $"prev_{a}";

                using (var connect = ConnectToDb.GetDbConnection())
                {
                    connect.Open();
                    var selectArch =
                        $"SELECT id FROM {Program.Prefix}arhiv_eacts44 WHERE arhiv = @archive AND region =  @region";
                    var cmd = new MySqlCommand(selectArch, connect);
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@archive", prevA);
                    cmd.Parameters.AddWithValue("@region", regionPath);
                    var reader = cmd.ExecuteReader();
                    var resRead = reader.HasRows;
                    reader.Close();
                    if (!resRead)
                    {
                        var addArch =
                            $"INSERT INTO {Program.Prefix}arhiv_eacts44 SET arhiv = @archive, region =  @region";
                        var cmd1 = new MySqlCommand(addArch, connect);
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