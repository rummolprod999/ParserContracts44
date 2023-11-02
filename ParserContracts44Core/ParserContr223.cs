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
    public class ParserContr223 : Parser
    {
        protected DataTable DtRegion;
        public readonly string[] NeedFile = new[] {"contractcompleting"};

        public ParserContr223(string arg) : base(arg)
        {
        }

        public override void Parsing()
        {
            DtRegion = GetRegions();
            foreach (DataRow row in DtRegion.Rows)
            {
                var arch = new List<string>();
                var pathParse = "";
                var regionPath = (string) row["path223"];
                switch (Program.Periodparsing)
                {
                    case TypeArguments.Last223:
                        pathParse = $"/out/published/{regionPath}/contractCompleting/";
                        arch = GetListArchLast(pathParse, regionPath);
                        break;
                    case TypeArguments.Daily223:
                        pathParse = $"/out/published/{regionPath}/contractCompleting/daily/";
                        arch = GetListArchCurr(pathParse, regionPath);
                        break;
                }

                if (arch.Count == 0)
                {
                    Log.Logger("Не получили список архивов по региону", pathParse);
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
            var filea = "";
            var pathUnzip = "";
            filea = GetArch223(arch, pathParse);
            if (!String.IsNullOrEmpty(filea))
            {
                pathUnzip = Unzipped.Unzip(filea);
                if (pathUnzip != "")
                {
                    if (Directory.Exists(pathUnzip))
                    {
                        var dirInfo = new DirectoryInfo(pathUnzip);
                        var filelist = dirInfo.GetFiles();
                        var array_complaint = filelist
                            .Where(a => NeedFile.Any(
                                            t => a.Name.ToLower().IndexOf(t, StringComparison.Ordinal) != -1) &&
                                        a.Length != 0)
                            .ToList();
                        foreach (var f in array_complaint)
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
                using (var sr = new StreamReader(f, Encoding.Default))
                {
                    string ftext;
                    ftext = sr.ReadToEnd();
                    ftext = ClearText.ClearString(ftext);
                    var doc = new XmlDocument();
                    doc.LoadXml(ftext);
                    var jsons = JsonConvert.SerializeXmlNode(doc);
                    var json = JObject.Parse(jsons);
                    var p = new WorkWithContract223(json, f, region);
                    p.Work223();
                }
            }
        }

        public override List<String> GetListArchLast(string pathParse, string regionPath)
        {
            var archtemp = GetListFtp(pathParse, Wftp223);
            return archtemp.Where(a => Program.Years.Any(t => a.IndexOf(t, StringComparison.Ordinal) != -1)).ToList();
        }

        public override List<String> GetListArchCurr(string pathParse, string regionPath)
        {
            var arch = new List<string>();
            var archtemp = GetListFtp(pathParse, Wftp223);
            foreach (var a in archtemp
                .Where(a => Program.Years.Any(t => a.IndexOf(t, StringComparison.Ordinal) != -1)))
            {
                using (var db = new Archive223Context())
                {
                    var Archives = db.ArchiveContracts223Results.Where(p => p.Archive == a).ToList();

                    if (Archives.Count == 0)
                    {
                        var ar = new ArchiveContracts223 {Archive = a, Region = regionPath};
                        db.ArchiveContracts223Results.Add(ar);
                        arch.Add(a);
                        db.SaveChanges();
                    }
                }
            }
            return arch;
        }
    }
}