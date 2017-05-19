﻿using System;
using System.Collections.Generic;
using System.Linq;
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
            string xml = GetXml(file);
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
            string notification_number =
                ((string) j44.SelectToken("export.contract.foundation.fcsOrder.order.notificationNumber") ?? "").Trim();
            if (notification_number == "")
                notification_number =
                    ((string) j44.SelectToken("export.contract.foundation.oosOrder.order.notificationNumber") ?? "")
                    .Trim();
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
            string execution_start_date =
                ((string) j44.SelectToken("export.contract.executionPeriod.currency.startDate") ?? "").Trim();
            string execution_end_date = ((string) j44.SelectToken("export.contract.executionPeriod.currency.endDate") ??
                                         "").Trim();
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
                    object resultm = cmd.ExecuteScalar();
                    int? max_number = (int?) (!Convert.IsDBNull(resultm) ? resultm : null);
                    if (max_number != null)
                    {
                        if (version_number > max_number)
                        {
                            string update_c = $"UPDATE {Program.Prefix}od_contract SET cancel=1 WHERE regnum= @regnum";
                            MySqlCommand cmd2 = new MySqlCommand(update_c, connect);
                            cmd2.Prepare();
                            cmd2.Parameters.AddWithValue("@regnum", regnum);
                            cmd2.ExecuteNonQuery();
                        }
                        else
                        {
                            cancel = 1;
                        }
                    }
                }
                string customer_regnumber = ((string) j44.SelectToken("export.contract.customer.regNum") ?? "").Trim();
                if (customer_regnumber != "")
                {
                    string select_customer =
                        $"SELECT id FROM {Program.Prefix}od_customer WHERE regNumber = @customer_regnumber";
                    MySqlCommand cmd3 = new MySqlCommand(select_customer, connect);
                    cmd3.Prepare();
                    cmd3.Parameters.AddWithValue("@customer_regnumber", customer_regnumber);
                    MySqlDataReader reader = cmd3.ExecuteReader();
                    bool res_read = reader.HasRows;
                    if (res_read)
                    {
                        reader.Read();
                        id_customer = reader.GetInt32("id");
                        reader.Close();
                    }
                    else
                    {
                        reader.Close();
                        string kpp_customer = ((string) j44.SelectToken("export.contract.customer.kpp") ?? "").Trim();
                        string inn_customer = ((string) j44.SelectToken("export.contract.customer.inn") ?? "").Trim();
                        string full_name_customer =
                            ((string) j44.SelectToken("export.contract.customer.fullName") ?? "").Trim();
                        string postal_address_customer = "";
                        int contracts_count_customer = 1;
                        decimal contracts_sum_customer = contract_price;
                        int contracts223_count_customer = 0;
                        decimal contracts223_sum_customer = 0.0m;
                        string ogrn_customer = "";
                        string region_code_customer = "";
                        string phone_customer = "";
                        string fax_customer = "";
                        string email_customer = "";
                        string contact_name_customer = "";
                        string add_customer =
                            $"INSERT INTO {Program.Prefix}od_customer SET regNumber = @customer_regnumber, inn = @inn_customer, " +
                            $"kpp = @kpp_customer, contracts_count = @contracts_count_customer, contracts223_count = @contracts223_count_customer," +
                            $"contracts_sum = @contracts_sum_customer, contracts223_sum = @contracts223_sum_customer," +
                            $"ogrn = @ogrn_customer, region_code = @region_code_customer, full_name = @full_name_customer," +
                            $"postal_address = @postal_address_customer, phone = @phone_customer, fax = @fax_customer," +
                            $"email = @email_customer, contact_name = @contact_name_customer";
                        MySqlCommand cmd4 = new MySqlCommand(add_customer, connect);
                        cmd4.Prepare();
                        cmd4.Parameters.AddWithValue("@customer_regnumber", customer_regnumber);
                        cmd4.Parameters.AddWithValue("@inn_customer", inn_customer);
                        cmd4.Parameters.AddWithValue("@kpp_customer", kpp_customer);
                        cmd4.Parameters.AddWithValue("@contracts_count_customer", contracts_count_customer);
                        cmd4.Parameters.AddWithValue("@contracts223_count_customer", contracts223_count_customer);
                        cmd4.Parameters.AddWithValue("@contracts_sum_customer", contracts_sum_customer);
                        cmd4.Parameters.AddWithValue("@contracts223_sum_customer", contracts223_sum_customer);
                        cmd4.Parameters.AddWithValue("@ogrn_customer", ogrn_customer);
                        cmd4.Parameters.AddWithValue("@region_code_customer", region_code_customer);
                        cmd4.Parameters.AddWithValue("@full_name_customer", full_name_customer);
                        cmd4.Parameters.AddWithValue("@postal_address_customer", postal_address_customer);
                        cmd4.Parameters.AddWithValue("@phone_customer", phone_customer);
                        cmd4.Parameters.AddWithValue("@fax_customer", fax_customer);
                        cmd4.Parameters.AddWithValue("@email_customer", email_customer);
                        cmd4.Parameters.AddWithValue("@contact_name_customer", contact_name_customer);
                        int add_c = cmd4.ExecuteNonQuery();
                        id_customer = (int)cmd4.LastInsertedId;
                        Console.WriteLine(id_customer);
                        if (add_c > 0)
                        {
                            Program.AddCustomer++;
                        }
                        else
                        {
                            Log.Logger("Не удалось добавить customer", file);
                        }
                    }
                }
                var test_sup = j44.SelectToken("export.contract.suppliers");
                if (test_sup != null && test_sup.Type != JTokenType.Null)
                {
                    var suppliers = test_sup.SelectToken("supplier") ?? new JArray();
                    var enumerable = suppliers as IList<JToken> ?? suppliers.ToList();
                    if (enumerable.Any())
                    {
                        var sup = enumerable.First();
                        Console.WriteLine(sup);
                    }
                    
                    
                }
            }
        }

        public string GetXml(string xml)
        {
            string[] xmlt = xml.Split('/');
            int t = xmlt.Length;
            xml = xmlt[t - 2] + "/" + xmlt[t - 1];
            return xml;
        }
    }
}