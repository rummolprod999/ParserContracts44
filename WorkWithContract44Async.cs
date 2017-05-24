using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace ParserContracts44
{
    public class WorkWithContract44Async : WorkWithContract44
    {
        public event AddData AddSupplierEvent;
        public event AddData AddCustomerEvent;
        public event AddData UpdateContractEvent;
        public event AddData AddContractEvent;
        public event AddData AddProductEvent;

        public WorkWithContract44Async(JObject json, string f, string r) : base(json, f, r)
        {
            AddCustomerEvent += AddCustomer;
            AddSupplierEvent += AddSupplier;
            UpdateContractEvent += UpdateContract;
            AddContractEvent += AddContract;
            AddProductEvent += AddProduct;
        }

        public new async void Work44()
        {
            string xml = GetXml(file);
            int id_customer = 0;
            int id_supplier = 0;
            string id_contract = ((string) j44.SelectToken("export.contract.id") ?? "").Trim();
            if (String.IsNullOrEmpty(id_contract))
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
            if (String.IsNullOrEmpty(placing))
                placing = ((string) j44.SelectToken("export.contract.foundation.oosOrder.order.placing") ?? "").Trim();
            string url = ((string) j44.SelectToken("export.contract.href") ?? "").Trim();
            string sign_date = ((string) j44.SelectToken("export.contract.signDate") ?? "").Trim();
            string single_customer_reason_code =
                ((string) j44.SelectToken("export.contract.foundation.fcsOrder.order.singleCustomer.reason.code") ?? "")
                .Trim();
            if (String.IsNullOrEmpty(single_customer_reason_code))
                single_customer_reason_code =
                ((string) j44.SelectToken("export.contract.foundation.oosOrder.order.singleCustomer.reason.code") ??
                 "").Trim();
            string single_customer_reason_name =
                ((string) j44.SelectToken("export.contract.foundation.fcsOrder.order.singleCustomer.reason.name") ?? "")
                .Trim();
            if (String.IsNullOrEmpty(single_customer_reason_name))
                single_customer_reason_name =
                ((string) j44.SelectToken("export.contract.foundation.oosOrder.order.singleCustomer.reason.name") ??
                 "").Trim();
            string fz = "44";
            string notification_number =
                ((string) j44.SelectToken("export.contract.foundation.fcsOrder.order.notificationNumber") ?? "").Trim();
            if (String.IsNullOrEmpty(notification_number))
                notification_number =
                    ((string) j44.SelectToken("export.contract.foundation.oosOrder.order.notificationNumber") ?? "")
                    .Trim();
            if (String.IsNullOrEmpty(notification_number))
                notification_number = "Нет номера";
            int lot_number = (int?) j44.SelectToken("export.contract.foundation.fcsOrder.order.lotNumber") ?? 0;
            if (lot_number == 0)
                lot_number = (int?) j44.SelectToken("export.contract.foundation.oosOrder.order.lotNumber") ?? 0;
            if (lot_number == 0)
                lot_number = 1;
            string contract_price = ((string) j44.SelectToken("export.contract.priceInfo.price") ?? "").Trim();
            /*decimal contract_price = decimal.Parse(contract_price_s, NumberStyles.Any, CultureInfo.InvariantCulture);*/
            string currency = ((string) j44.SelectToken("export.contract.priceInfo.currency.name") ?? "").Trim();
            int version_number = (int?) j44.SelectToken("export.contract.versionNumber") ?? 0;
            int cancel = 0;
            string execution_start_date =
                ((string) j44.SelectToken("export.contract.executionPeriod.startDate") ?? "").Trim();
            string execution_end_date = ((string) j44.SelectToken("export.contract.executionPeriod.endDate") ??
                                         "").Trim();
            using (MySqlConnection connect = ConnectToDb.GetDBConnection())
            {
                connect.Open();
                if (!String.IsNullOrEmpty(regnum) && version_number != 0)
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
                if (!String.IsNullOrEmpty(customer_regnumber))
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
                        string contracts_sum_customer = contract_price;
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
                        id_customer = (int) cmd4.LastInsertedId;

                        AddCustomerEvent?.Invoke(add_c);
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
                        if (sup.Type == JTokenType.Property)
                        {
                            sup = enumerable.First().Parent;
                        }
                        string supplier_inn = ((string) sup.SelectToken("legalEntityRF.INN") ?? "").Trim();
                        if (String.IsNullOrEmpty(supplier_inn))
                        {
                            supplier_inn = ((string) sup.SelectToken("individualPersonRF.INN") ?? "").Trim();
                        }
                        if (String.IsNullOrEmpty(supplier_inn))
                        {
                            supplier_inn = ((string) sup.SelectToken("legalEntityForeignState.INN") ?? "").Trim();
                        }
                        if (String.IsNullOrEmpty(supplier_inn))
                        {
                            supplier_inn =
                                ((string) sup.SelectToken("legalEntityForeignState.taxPayerCode") ?? "").Trim();
                        }
                        if (String.IsNullOrEmpty(supplier_inn))
                        {
                            supplier_inn =
                            ((string) sup.SelectToken("individualPersonForeignState.registerInRFTaxBodies.INN") ??
                             "").Trim();
                        }
                        if (String.IsNullOrEmpty(supplier_inn))
                        {
                            supplier_inn = ((string) sup.SelectToken("individualPersonForeignState.taxPayerCode") ?? "")
                                .Trim();
                        }
                        if (String.IsNullOrEmpty(supplier_inn))
                        {
                            supplier_inn = ((string) sup.SelectToken("individualPersonForeignState.INN") ?? "").Trim();
                        }
                        if (!String.IsNullOrEmpty(supplier_inn))
                        {
                            string kpp_supplier = ((string) sup.SelectToken("legalEntityRF.KPP") ?? "").Trim();
                            if (String.IsNullOrEmpty(kpp_supplier))
                            {
                                kpp_supplier = ((string) sup.SelectToken("individualPersonRF.KPP") ?? "").Trim();
                            }
                            if (String.IsNullOrEmpty(kpp_supplier))
                            {
                                kpp_supplier =
                                ((string) sup.SelectToken(
                                     "individualPersonForeignState.registerInRFTaxBodies.KPP") ?? "").Trim();
                            }
                            string select_supplier =
                                $"SELECT id FROM {Program.Prefix}od_supplier WHERE inn = @supplier_inn AND kpp = @kpp_supplier";
                            MySqlCommand cmd5 = new MySqlCommand(select_supplier, connect);
                            cmd5.Prepare();
                            cmd5.Parameters.AddWithValue("@supplier_inn", supplier_inn);
                            cmd5.Parameters.AddWithValue("@kpp_supplier", kpp_supplier);
                            MySqlDataReader reader = cmd5.ExecuteReader();
                            bool res_read = reader.HasRows;
                            if (res_read)
                            {
                                reader.Read();
                                id_supplier = reader.GetInt32("id");
                                reader.Close();
                            }
                            else
                            {
                                reader.Close();
                                string contactphone_supplier =
                                    ((string) sup.SelectToken("legalEntityRF.contactPhone") ?? "").Trim();
                                if (String.IsNullOrEmpty(contactphone_supplier))
                                {
                                    contactphone_supplier =
                                        ((string) sup.SelectToken("individualPersonRF.contactPhone") ?? "").Trim();
                                }
                                if (String.IsNullOrEmpty(contactphone_supplier))
                                {
                                    contactphone_supplier =
                                    ((string) sup.SelectToken(
                                         "legalEntityForeignState.placeOfStayInRegCountry.contactPhone") ?? "").Trim();
                                }
                                if (String.IsNullOrEmpty(contactphone_supplier))
                                {
                                    contactphone_supplier =
                                        ((string) sup.SelectToken(
                                             "individualPersonForeignState.placeOfStayInRegCountry.contactPhone") ?? "")
                                        .Trim();
                                }
                                string contactemail_supplier =
                                    ((string) sup.SelectToken("legalEntityRF.contactEMail") ?? "").Trim();
                                if (String.IsNullOrEmpty(contactemail_supplier))
                                {
                                    contactemail_supplier =
                                        ((string) sup.SelectToken("individualPersonRF.contactEMail") ?? "").Trim();
                                }
                                if (String.IsNullOrEmpty(contactemail_supplier))
                                {
                                    contactemail_supplier =
                                    ((string) sup.SelectToken(
                                         "legalEntityForeignState.placeOfStayInRegCountry.contactEMail") ?? "").Trim();
                                }
                                if (String.IsNullOrEmpty(contactemail_supplier))
                                {
                                    contactemail_supplier =
                                        ((string) sup.SelectToken(
                                             "individualPersonForeignState.placeOfStayInRegCountry.contactEMail") ?? "")
                                        .Trim();
                                }
                                string organizationname_supplier =
                                    ((string) sup.SelectToken("legalEntityRF.fullName") ?? "").Trim();
                                if (String.IsNullOrEmpty(organizationname_supplier))
                                {
                                    organizationname_supplier =
                                        ((string) sup.SelectToken("legalEntityForeignState.fullName") ?? "").Trim();
                                }
                                if (String.IsNullOrEmpty(organizationname_supplier))
                                {
                                    string lastname = ((string) sup.SelectToken("individualPersonRF.lastName") ?? "")
                                        .Trim();
                                    if (String.IsNullOrEmpty(lastname))
                                    {
                                        lastname = ((string) sup.SelectToken("individualPersonForeignState.lastName") ??
                                                    "").Trim();
                                    }
                                    string firsname = ((string) sup.SelectToken("individualPersonRF.firstName") ?? "")
                                        .Trim();
                                    if (String.IsNullOrEmpty(firsname))
                                    {
                                        firsname =
                                            ((string) sup.SelectToken("individualPersonForeignState.firstName") ?? "")
                                            .Trim();
                                    }
                                    string middlename =
                                        ((string) sup.SelectToken("individualPersonRF.middleName") ?? "").Trim();
                                    if (String.IsNullOrEmpty(middlename))
                                    {
                                        middlename =
                                            ((string) sup.SelectToken("individualPersonForeignState.middleName") ?? "")
                                            .Trim();
                                    }
                                    if (!String.IsNullOrEmpty(lastname) || !String.IsNullOrEmpty(firsname) ||
                                        !String.IsNullOrEmpty(middlename))
                                    {
                                        organizationname_supplier = $"{lastname} {firsname} {middlename}".Trim();
                                    }
                                }
                                int contracts_count_supplier = 1;
                                string contracts_sum_supplier = contract_price;
                                int contracts223_count_supplier = 0;
                                decimal contracts223_sum_supplier = 0.0m;
                                string ogrn_supplier = "";
                                string region_code_supplier = "";
                                string postal_address_supplier = "";
                                string contactfax_supplier = "";
                                string contact_name_supplier = "";
                                string add_supplier =
                                    $"INSERT INTO {Program.Prefix}od_supplier SET inn = @supplier_inn, kpp = @kpp_supplier, " +
                                    $"contracts_count = @contracts_count, " +
                                    $"contracts223_count = @contracts223_count, contracts_sum = @contracts_sum, " +
                                    $"contracts223_sum = @contracts223_sum, ogrn = @ogrn,region_code = @region_code, " +
                                    $"organizationName = @organizationName,postal_address = @postal_address, " +
                                    $"contactPhone = @contactPhone,contactFax = @contactFax, " +
                                    $"contactEMail = @contactEMail,contact_name = @contact_name";
                                MySqlCommand cmd6 = new MySqlCommand(add_supplier, connect);
                                cmd6.Prepare();
                                cmd6.Parameters.AddWithValue("@supplier_inn", supplier_inn);
                                cmd6.Parameters.AddWithValue("@kpp_supplier", kpp_supplier);
                                cmd6.Parameters.AddWithValue("@contracts_count", contracts_count_supplier);
                                cmd6.Parameters.AddWithValue("@contracts223_count", contracts223_count_supplier);
                                cmd6.Parameters.AddWithValue("@contracts_sum", contracts_sum_supplier);
                                cmd6.Parameters.AddWithValue("@contracts223_sum", contracts223_sum_supplier);
                                cmd6.Parameters.AddWithValue("@ogrn", ogrn_supplier);
                                cmd6.Parameters.AddWithValue("@region_code", region_code_supplier);
                                cmd6.Parameters.AddWithValue("@organizationName", organizationname_supplier);
                                cmd6.Parameters.AddWithValue("@postal_address", postal_address_supplier);
                                cmd6.Parameters.AddWithValue("@contactPhone", contactphone_supplier);
                                cmd6.Parameters.AddWithValue("@contactFax", contactfax_supplier);
                                cmd6.Parameters.AddWithValue("@contactEMail", contactemail_supplier);
                                cmd6.Parameters.AddWithValue("@contact_name", contact_name_supplier);
                                int add_s = cmd6.ExecuteNonQuery();
                                id_supplier = (int) cmd6.LastInsertedId;
                                AddSupplierEvent?.Invoke(add_s);
                            }
                        }
                    }
                }
                int id_od_contract = 0;
                string select_contract =
                    $"SELECT id FROM {Program.Prefix}od_contract WHERE id_contract = @id_contract AND region_code = @region_code";
                MySqlCommand cmd7 = new MySqlCommand(select_contract, connect);
                cmd7.Prepare();
                cmd7.Parameters.AddWithValue("@id_contract", id_contract);
                cmd7.Parameters.AddWithValue("@region_code", region);
                MySqlDataReader reader_c = cmd7.ExecuteReader();
                if (reader_c.HasRows)
                {
                    reader_c.Read();
                    id_od_contract = reader_c.GetInt32("id");
                    reader_c.Close();
                    string delete_products =
                        $"DELETE FROM {Program.Prefix}od_contract_product WHERE id_od_contract = @id_od_contract";
                    MySqlCommand cmd8 = new MySqlCommand(delete_products, connect);
                    cmd8.Prepare();
                    cmd8.Parameters.AddWithValue("@id_od_contract", id_od_contract);
                    cmd8.ExecuteNonQuery();
                    string update_contract =
                        $"UPDATE {Program.Prefix}od_contract SET p_number = @p_number, regnum = @regnum, " +
                        $"current_contract_stage = @current_contract_stage, placing = @placing, " +
                        $"region_code = @region_code, url = @url, sign_date = @sign_date, " +
                        $"single_customer_reason_code = @single_customer_reason_code, single_customer_reason_name = @single_customer_reason_name, " +
                        $"fz = @fz, notification_number = @notification_number, lot_number = @lot_number, " +
                        $"contract_price = @contract_price, currency = @currency, version_number = @version_number, " +
                        $"execution_start_date = @execution_start_date, execution_end_date = @execution_end_date, " +
                        $"id_customer = @id_customer, id_supplier = @id_supplier, xml = @xml WHERE id = @id_od_contract";
                    MySqlCommand cmd9 = new MySqlCommand(update_contract, connect);
                    cmd9.Prepare();
                    cmd9.Parameters.AddWithValue("@p_number", p_number);
                    cmd9.Parameters.AddWithValue("@regnum", regnum);
                    cmd9.Parameters.AddWithValue("@current_contract_stage", current_contract_stage);
                    cmd9.Parameters.AddWithValue("@placing", placing);
                    cmd9.Parameters.AddWithValue("@region_code", region);
                    cmd9.Parameters.AddWithValue("@url", url);
                    cmd9.Parameters.AddWithValue("@sign_date", sign_date);
                    cmd9.Parameters.AddWithValue("@single_customer_reason_code", single_customer_reason_code);
                    cmd9.Parameters.AddWithValue("@single_customer_reason_name", single_customer_reason_name);
                    cmd9.Parameters.AddWithValue("@fz", fz);
                    cmd9.Parameters.AddWithValue("@notification_number", notification_number);
                    cmd9.Parameters.AddWithValue("@lot_number", lot_number);
                    cmd9.Parameters.AddWithValue("@contract_price", contract_price);
                    cmd9.Parameters.AddWithValue("@currency", currency);
                    cmd9.Parameters.AddWithValue("@version_number", version_number);
                    cmd9.Parameters.AddWithValue("@execution_start_date", execution_start_date);
                    cmd9.Parameters.AddWithValue("@execution_end_date", execution_end_date);
                    cmd9.Parameters.AddWithValue("@id_customer", id_customer);
                    cmd9.Parameters.AddWithValue("@id_supplier", id_supplier);
                    cmd9.Parameters.AddWithValue("@xml", xml);
                    cmd9.Parameters.AddWithValue("@id_od_contract", id_od_contract);
                    int upd_c = cmd9.ExecuteNonQuery();
                    UpdateContractEvent?.Invoke(upd_c);
                }
                else
                {
                    reader_c.Close();
                    string insert_contract =
                        $"INSERT INTO {Program.Prefix}od_contract SET id_contract = @id_contract, p_number = @p_number, regnum = @regnum, " +
                        $"current_contract_stage = @current_contract_stage, placing = @placing, " +
                        $"region_code = @region_code, url = @url, sign_date = @sign_date, " +
                        $"single_customer_reason_code = @single_customer_reason_code, single_customer_reason_name = @single_customer_reason_name, " +
                        $"fz = @fz, notification_number = @notification_number, lot_number = @lot_number, " +
                        $"contract_price = @contract_price, currency = @currency, version_number = @version_number, " +
                        $"execution_start_date = @execution_start_date, execution_end_date = @execution_end_date, " +
                        $"id_customer = @id_customer, id_supplier = @id_supplier, cancel = @cancel, xml = @xml";
                    MySqlCommand cmd10 = new MySqlCommand(insert_contract, connect);
                    cmd10.Prepare();
                    cmd10.Parameters.AddWithValue("@id_contract", id_contract);
                    cmd10.Parameters.AddWithValue("@p_number", p_number);
                    cmd10.Parameters.AddWithValue("@regnum", regnum);
                    cmd10.Parameters.AddWithValue("@current_contract_stage", current_contract_stage);
                    cmd10.Parameters.AddWithValue("@placing", placing);
                    cmd10.Parameters.AddWithValue("@region_code", region);
                    cmd10.Parameters.AddWithValue("@url", url);
                    cmd10.Parameters.AddWithValue("@sign_date", sign_date);
                    cmd10.Parameters.AddWithValue("@single_customer_reason_code", single_customer_reason_code);
                    cmd10.Parameters.AddWithValue("@single_customer_reason_name", single_customer_reason_name);
                    cmd10.Parameters.AddWithValue("@fz", fz);
                    cmd10.Parameters.AddWithValue("@notification_number", notification_number);
                    cmd10.Parameters.AddWithValue("@lot_number", lot_number);
                    cmd10.Parameters.AddWithValue("@contract_price", contract_price);
                    cmd10.Parameters.AddWithValue("@currency", currency);
                    cmd10.Parameters.AddWithValue("@version_number", version_number);
                    cmd10.Parameters.AddWithValue("@execution_start_date", execution_start_date);
                    cmd10.Parameters.AddWithValue("@execution_end_date", execution_end_date);
                    cmd10.Parameters.AddWithValue("@id_customer", id_customer);
                    cmd10.Parameters.AddWithValue("@id_supplier", id_supplier);
                    cmd10.Parameters.AddWithValue("@cancel", cancel);
                    cmd10.Parameters.AddWithValue("@xml", xml);
                    int add_contr = cmd10.ExecuteNonQuery();
                    id_od_contract = (int) cmd10.LastInsertedId;
                    AddContractEvent?.Invoke(add_contr);
                }
                var test_prod = j44.SelectToken("export.contract.products");
                if (test_prod != null && test_prod.Type != JTokenType.Null)
                {
                    var products = test_prod.SelectToken("product");
                    if (products.Any())
                    {
                        List<JToken> list_p = new List<JToken>();
                        if (products.Type == JTokenType.Array)
                        {
                            foreach (var p in products)
                            {
                                list_p.Add(p);
                            }
                        }
                        else
                        {
                            list_p.Add(products);
                        }

                        if (list_p.Count == 0)
                        {
                            Log.Logger("У контракта нет продуктов", file);
                            return;
                        }

                        foreach (var prod in list_p)
                        {
                            int okpd2_group_code = 0;
                            string okpd2_group_level1_code = "";
                            int okpd_group_code = 0;
                            string okpd_group_level1_code = "";
                            string name_p = ((string) prod.SelectToken("name") ?? "").Trim();
                            name_p = Regex.Replace(name_p, @"\t|\n|\r", "");
                            if (String.IsNullOrEmpty(name_p))
                                name_p = "Нет названия";
                            string okpd2_code = ((string) prod.SelectToken("OKPD2.code") ?? "").Trim();
                            if (!String.IsNullOrEmpty(okpd2_code))
                            {
                                GetOKPD(okpd2_code, out okpd2_group_code, out okpd2_group_level1_code);
                            }
                            string okpd2_name = ((string) prod.SelectToken("OKPD2.name") ?? "").Trim();
                            string okpd_code = ((string) prod.SelectToken("OKPD.code") ?? "").Trim();
                            if (!String.IsNullOrEmpty(okpd_code))
                            {
                                GetOKPD(okpd_code, out okpd_group_code, out okpd_group_level1_code);
                            }
                            string okpd_name = ((string) prod.SelectToken("OKPD.name") ?? "").Trim();
                            string price = ((string) prod.SelectToken("price") ?? "").Trim();
                            /*decimal price = decimal.Parse(price_s, NumberStyles.Any, CultureInfo.InvariantCulture);*/
                            string quantity = ((string) prod.SelectToken("quantity") ?? "").Trim();
                            /*decimal quantity =
                                decimal.Parse(quantity_s, NumberStyles.Any, CultureInfo.InvariantCulture);*/
                            string sum_p = ((string) prod.SelectToken("sum") ?? "").Trim();
                            /*decimal sum_p = decimal.Parse(sum_p_s, NumberStyles.Any, CultureInfo.InvariantCulture);*/
                            string sid = ((string) prod.SelectToken("sid") ?? "").Trim();
                            string okei = ((string) prod.SelectToken("OKEI.nationalCode") ?? "").Trim();
                            string insert_prod =
                                $"INSERT INTO {Program.Prefix}od_contract_product SET id_od_contract = @id_od_contract, " +
                                $"name = @name_p, okpd2_code = @okpd2_code, okpd_code = @okpd_code, okpd2_group_code = @okpd2_group_code, " +
                                $"okpd_group_code = @okpd_group_code, okpd2_group_level1_code = @okpd2_group_level1_code, " +
                                $"okpd_group_level1_code = @okpd_group_level1_code, price = @price, okpd2_name = @okpd2_name, " +
                                $"okpd_name = @okpd_name, quantity = @quantity, okei = @okei, sum = @sum, sid = @sid";
                            MySqlCommand cmd11 = new MySqlCommand(insert_prod, connect);
                            cmd11.Prepare();
                            cmd11.Parameters.AddWithValue("@id_od_contract", id_od_contract);
                            cmd11.Parameters.AddWithValue("@name_p", name_p);
                            cmd11.Parameters.AddWithValue("@okpd2_code", okpd2_code);
                            cmd11.Parameters.AddWithValue("@okpd_code", okpd_code);
                            cmd11.Parameters.AddWithValue("@okpd2_group_code", okpd2_group_code);
                            cmd11.Parameters.AddWithValue("@okpd_group_code", okpd_group_code);
                            cmd11.Parameters.AddWithValue("@okpd2_group_level1_code", okpd2_group_level1_code);
                            cmd11.Parameters.AddWithValue("@okpd_group_level1_code", okpd_group_level1_code);
                            cmd11.Parameters.AddWithValue("@price", price);
                            cmd11.Parameters.AddWithValue("@okpd2_name", okpd2_name);
                            cmd11.Parameters.AddWithValue("@okpd_name", okpd_name);
                            cmd11.Parameters.AddWithValue("@quantity", quantity);
                            cmd11.Parameters.AddWithValue("@okei", okei);
                            cmd11.Parameters.AddWithValue("@sum", sum_p);
                            cmd11.Parameters.AddWithValue("@sid", sid);
                            int add_p =  await cmd11.ExecuteNonQueryAsync();
                            AddProductEvent?.Invoke(add_p);
                        }
                    }
                }
            }
        }

        public void AddProductAsync()
        {
            
        }
    }
}