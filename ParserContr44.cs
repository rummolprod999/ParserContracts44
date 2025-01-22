using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
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

        public readonly string[] ExceptFile = new[]
            { "Failure", "contractProcedure", "contractCancel", "contractAvailableForElAct" };

        private readonly string[] types =
        {
            "contract"
        };

        public ParserContr44(string arg) : base(arg)
        {
        }

        public override void Parsing()
        {
            DtRegion = GetRegions();
            for (var i = Program._days; i >= 0; i--)
            {
                foreach (DataRow row in DtRegion.Rows)
                {
                    var regionKladr = (string)row["conf"];
                    foreach (var type in types)
                    {
                        try
                        {
                            var arch = new List<string>();
                            switch (Program.Periodparsing)
                            {
                                case TypeArguments.Curr44:

                                    arch = GetListArchCurr(regionKladr, type, i);

                                    break;
                            }

                            if (arch.Count == 0)
                            {
                                Log.Logger("Не получили список архивов по региону", row["path"]);
                                continue;
                            }

                            foreach (var v in arch)
                            {
                                GetListFileArch(v, (string)row["conf"]);
                            }
                        }
                        catch (Exception e)
                        {
                            Log.Logger("Ошибка при парсинге xml", e, type, regionKladr);
                        }
                    }
                }
            }
        }

        public void GetListFileArch(string arch, string region)
        {
            var filea = "";
            var pathUnzip = "";
            filea = downloadArchive(arch);
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

        private string downloadArchive(string url)
        {
            var count = 5;
            var sleep = 5000;
            var dest = $"{Program.TempPath}{Path.DirectorySeparatorChar}array.zip";
            while (true)
            {
                try
                {
                    using (var client = new TimedWebClient())
                    {
                        client.Headers.Add("individualPerson_token", Program._token);
                        client.DownloadFile(url, dest);
                    }

                    break;
                }
                catch (Exception e)
                {
                    if (count <= 0)
                    {
                        Log.Logger($"Не удалось скачать {url}");
                        break;
                    }

                    count--;
                    Thread.Sleep(sleep);
                    sleep *= 2;
                }
            }

            return dest;
        }

        public void ParsingXml(string f, string region)
        {
            var fileInf = new FileInfo(f);
            if (fileInf.Exists)
            {
                using (var sr = new StreamReader(f, Encoding.Default))
                {
                    string ftext;
                    ftext = sr.ReadToEnd();
                    ftext = ClearText.ClearString(ftext);
                    var doc = new XmlDocument();
                    doc.LoadXml(ftext);
                    var jsons = JsonConvert.SerializeXmlNode(doc);
                    var json = JObject.Parse(jsons);
                    /*WorkWithContract44 c = new WorkWithContract44(json, f, region);
                    c.Work44();*/
                    var p = new WorkWithContract44Parralel(json, f, region);
                    p.Work44();
                    /*WorkWithContract44Async a = new WorkWithContract44Async(json, f, region);
                    a.Work44();*/
                }
            }
        }


        public override List<String> GetListArchLast(string pathParse, string regionPath)
        {
            var archtemp = GetListFtp(pathParse, Wftp44);
            return archtemp.Where(a => Program.Years.Any(t => a.IndexOf(t, StringComparison.Ordinal) != -1)).ToList();
        }

        public List<String> GetListArchCurr(string regionKladr, string type, int i)
        {
            var arch = new List<string>();
            var resp = soap44(regionKladr, type, i);
            var xDoc = new XmlDocument();
            try
            {
                xDoc.LoadXml(resp);
            }
            catch (Exception e)
            {
                Log.Logger(e, resp);
                throw;
            }

            var nodeList = xDoc.SelectNodes("//dataInfo/archiveUrl");
            foreach (XmlNode node in nodeList)
            {
                var nodeValue = node.InnerText;
                arch.Add(nodeValue);
            }

            return arch;
        }

        public static string soap44(string regionKladr, string type, int i)
        {
            var count = 5;
            var sleep = 2000;
            while (true)
            {
                try
                {
                    var guid = Guid.NewGuid();
                    var currDate = DateTime.Now.ToString("s");
                    var prevday = DateTime.Now.AddDays(-1 * i).ToString("yyyy-MM-dd");
                    var request =
                        $"<soapenv:Envelope xmlns:soapenv=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:ws=\"http://zakupki.gov.ru/fz44/get-docs-ip/ws\">\n<soapenv:Header>\n<individualPerson_token>{Program._token}</individualPerson_token>\n</soapenv:Header>\n<soapenv:Body>\n<ws:getDocsByOrgRegionRequest>\n<index>\n<id>{guid}</id>\n<createDateTime>{currDate}</createDateTime>\n<mode>PROD</mode>\n</index>\n<selectionParams>\n<orgRegion>{regionKladr}</orgRegion>\n<subsystemType>RGK</subsystemType>\n<documentType44>{type}</documentType44>\n<periodInfo><exactDate>{prevday}</exactDate></periodInfo>\n</selectionParams>\n</ws:getDocsByOrgRegionRequest>\n</soapenv:Body>\n</soapenv:Envelope>";
                    var url = "https://int44.zakupki.gov.ru/eis-integration/services/getDocsIP";
                    var response = "";
                    using (WebClient wc = new TimedWebClient())
                    {
                        wc.Headers[HttpRequestHeader.ContentType] = "text/xml; charset=utf-8";
                        response = wc.UploadString(url,
                            request);
                    }

                    //Console.WriteLine(response);
                    return response;
                }
                catch (Exception e)
                {
                    if (count <= 0)
                    {
                        throw;
                    }

                    count--;
                    Thread.Sleep(sleep);
                    sleep *= 2;
                }
            }
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
                        $"SELECT id FROM {Program.Prefix}arhiv_contract WHERE arhiv = @archive AND region =  @region";
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
                            $"INSERT INTO {Program.Prefix}arhiv_contract SET arhiv = @archive, region =  @region";
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

    public class TimedWebClient : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            var wr = base.GetWebRequest(address);
            wr.Timeout = 300000;
            return wr;
        }
    }
}