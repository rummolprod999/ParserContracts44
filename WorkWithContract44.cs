using System;
using Newtonsoft.Json.Linq;

namespace ParserContracts44
{
    public class WorkWithContract44
    {
        private readonly JObject j44;
        private readonly string file;
        private readonly string region;

        public WorkWithContract44(JObject json, string f, string r)
        {
            j44 = json;
            file = f;
            region = r;
        }

        public void Work44()
        {
            int id_customer = 0;
            int id_supplier = 0;
            string id_contract = ((string) j44.SelectToken("export.contract.id") ?? "").Trim();
            if (id_contract == "")
            {
                Log.Logger("У контракта нет id", file);
                return;
            }
            string p_number = id_contract;
            string regnum = ((string) j44.SelectToken("export.contract.regNum") ?? "").Trim();
            string current_contract_stage = ((string) j44.SelectToken("export.contract.currentContractStage") ?? "")
                .Trim();
            string placing = ((string) j44.SelectToken("export.contract.foundation.fcsOrder.order.placing") ?? "");
            if (placing == "")
            {
                placing = ((string) j44.SelectToken("export.contract.foundation.oosOrder.order.placing") ?? "");
            }
        }
    }
}