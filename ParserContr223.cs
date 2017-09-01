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
        public readonly string[] NeedFile = new[] {"contractCompleting"};
        
        public ParserContr223(string arg) : base(arg)
        {
        }

        public override void Parsing()
        {
            DtRegion = GetRegions();
            foreach (DataRow row in DtRegion.Rows)
            {
                List<String> arch = new List<string>();
                string pathParse = "";
                string regionPath = (string) row["path223"];
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
            }
        }
    }
}