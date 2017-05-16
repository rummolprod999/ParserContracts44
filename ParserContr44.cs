using System;
using System.IO;
using System.Net;
using System.Web;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ParserContracts44
{
    public class ParserContr44 : Parser
    {
        protected DataTable DtRegion;

        public ParserContr44(string arg) : base(arg)
        {
        }

        public override void Parsing()
        {
            DtRegion = GetRegions();
            foreach (DataRow row in DtRegion.Rows)
            {
                List<String> arch = new List<string>();

                if (Program.Periodparsing == TypeArguments.Last)
                {
                    arch = GetListArchLast((string) row["path"]);
                }
                else if (Program.Periodparsing == TypeArguments.Curr)
                {
                    arch = GetListArchCurr((string) row["path"]);
                }
                else if (Program.Periodparsing == TypeArguments.Prev)
                {
                    arch = GetListArchPrev((string) row["path"]);
                }
            }
        }

        public override List<String> GetListArchLast(string RegionPath)
        {
            List<String> arch = new List<string>();
            string PathParse = $"fcs_regions/{RegionPath}/contracts/";
            return arch;
        }

        public override List<String> GetListArchCurr(string RegionPath)
        {
            List<String> arch = new List<string>();
            string PathParse = $"fcs_regions/{RegionPath}/contracts/currMonth/";
            return arch;
        }
        public override List<String> GetListArchPrev(string RegionPath)
        {
            List<String> arch = new List<string>();
            string PathParse = $"fcs_regions/{RegionPath}/contracts/prevMonth/";
            return arch;
        }
    }
}