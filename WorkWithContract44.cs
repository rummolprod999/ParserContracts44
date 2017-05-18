using System;
using MySql.Data.MySqlClient;
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
            string placing =
                ((string) j44.SelectToken("export.contract.foundation.fcsOrder.order.placing") ?? "").Trim();
            if (placing == "")
                placing = ((string) j44.SelectToken("export.contract.foundation.oosOrder.order.placing") ?? "").Trim();
            string url = ((string) j44.SelectToken("export.contract.href") ?? "").Trim();
            string sign_date = ((string) j44.SelectToken("export.contract.signDate") ?? "").Trim();
            string single_customer_reason_code =
                ((string) j44.SelectToken("export.contract.foundation.fcsOrder.order.singleCustomer.reason.code") ?? "")
                .Trim();
            if (single_customer_reason_code == "")
                single_customer_reason_code =
                ((string) j44.SelectToken("export.contract.foundation.oosOrder.order.singleCustomer.reason.code") ??
                 "").Trim();
            string single_customer_reason_name =
                ((string) j44.SelectToken("export.contract.foundation.fcsOrder.order.singleCustomer.reason.name") ?? "")
                .Trim();
            if (single_customer_reason_name == "")
                single_customer_reason_name =
                ((string) j44.SelectToken("export.contract.foundation.oosOrder.order.singleCustomer.reason.name") ??
                 "").Trim();
            string fz = "44";
            string notification_number = ((string) j44.SelectToken("export.contract.foundation.fcsOrder.order.notificationNumber") ?? "").Trim();
            if (notification_number == "")
                notification_number = ((string) j44.SelectToken("export.contract.foundation.oosOrder.order.notificationNumber") ?? "").Trim();
            if (notification_number == "")
                notification_number = "Нет номера";
            int lot_number = (int?) j44.SelectToken("export.contract.foundation.fcsOrder.order.lotNumber") ?? 0;
            if (lot_number == 0)
                lot_number = (int?) j44.SelectToken("export.contract.foundation.oosOrder.order.lotNumber") ?? 0;
            if (lot_number == 0)
                lot_number = 1;
            decimal contract_price = (decimal?) j44.SelectToken("export.contract.priceInfo.price") ?? 0.0m;

            string currency = ((string) j44.SelectToken("export.contract.priceInfo.currency.name") ?? "").Trim();
            int version_number = (int?) j44.SelectToken("export.contract.versionNumber") ?? 0;
            int cancel = 0;
            string execution_start_date = ((string) j44.SelectToken("export.contract.executionPeriod.currency.startDate") ?? "").Trim();
            string execution_end_date = ((string) j44.SelectToken("export.contract.executionPeriod.currency.endDate") ?? "").Trim();
            using (MySqlConnection connect = ConnectToDb.GetDBConnection())
            {
                connect.Open();
                if (regnum != "" && version_number != 0)
                {
                    string select_get_max =
                        $"SELECT MAX(version_number) as m FROM {Program.Prefix}od_contract WHERE regnum = @regnum";
                    MySqlCommand cmd = new MySqlCommand(select_get_max, connect);
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@regnum", regnum);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    bool res_read = reader.HasRows;

                    if (res_read)
                    {
                        int? max_number = (int?)reader["m"];
                        Console.WriteLine(max_number);
                        reader.Close();
                    }
                }
            }

        }
    }
}