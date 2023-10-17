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

        public new void Work44()
        {
            var xml = GetXml(File);
            var idCustomer = 0;
            var idSupplier = 0;
            var idContract = ((string) J44.SelectToken("export.contract.id") ?? "").Trim();
            if (String.IsNullOrEmpty(idContract))
            {
                Log.Logger("У контракта нет id", File);
                return;
            }
            var pNumber = idContract;
            var regnum = ((string) J44.SelectToken("export.contract.regNum") ?? "").Trim();
            var currentContractStage = ((string) J44.SelectToken("export.contract.currentContractStage") ?? "")
                .Trim();
            var placing =
                ((string) J44.SelectToken("export.contract.foundation.fcsOrder.order.placing") ?? "").Trim();
            if (String.IsNullOrEmpty(placing))
                placing = ((string) J44.SelectToken("export.contract.foundation.oosOrder.order.placing") ?? "").Trim();
            var url = ((string) J44.SelectToken("export.contract.href") ?? "").Trim();
            var signDate = ((string) J44.SelectToken("export.contract.signDate") ?? "").Trim();
            var singleCustomerReasonCode =
                ((string) J44.SelectToken("export.contract.foundation.fcsOrder.order.singleCustomer.reason.code") ?? "")
                .Trim();
            if (String.IsNullOrEmpty(singleCustomerReasonCode))
                singleCustomerReasonCode =
                ((string) J44.SelectToken("export.contract.foundation.oosOrder.order.singleCustomer.reason.code") ??
                 "").Trim();
            var singleCustomerReasonName =
                ((string) J44.SelectToken("export.contract.foundation.fcsOrder.order.singleCustomer.reason.name") ?? "")
                .Trim();
            if (String.IsNullOrEmpty(singleCustomerReasonName))
                singleCustomerReasonName =
                ((string) J44.SelectToken("export.contract.foundation.oosOrder.order.singleCustomer.reason.name") ??
                 "").Trim();
            var fz = "44";
            var notificationNumber =
                ((string) J44.SelectToken("export.contract.foundation.fcsOrder.order.notificationNumber") ?? "").Trim();
            if (String.IsNullOrEmpty(notificationNumber))
                notificationNumber =
                    ((string) J44.SelectToken("export.contract.foundation.oosOrder.order.notificationNumber") ?? "")
                    .Trim();
            if (String.IsNullOrEmpty(notificationNumber))
                notificationNumber = "Нет номера";
            var lotNumber = (int?) J44.SelectToken("export.contract.foundation.fcsOrder.order.lotNumber") ?? 0;
            if (lotNumber == 0)
                lotNumber = (int?) J44.SelectToken("export.contract.foundation.oosOrder.order.lotNumber") ?? 0;
            if (lotNumber == 0)
                lotNumber = 1;
            var contractPrice = ((string) J44.SelectToken("export.contract.priceInfo.price") ?? "").Trim();
            /*decimal contract_price = decimal.Parse(contract_price_s, NumberStyles.Any, CultureInfo.InvariantCulture);*/
            var currency = ((string) J44.SelectToken("export.contract.priceInfo.currency.name") ?? "").Trim();
            var versionNumber = (int?) J44.SelectToken("export.contract.versionNumber") ?? 0;
            var cancel = 0;
            var executionStartDate =
                ((string) J44.SelectToken("export.contract.executionPeriod.startDate") ?? "").Trim();
            var executionEndDate = ((string) J44.SelectToken("export.contract.executionPeriod.endDate") ??
                                    "").Trim();
            using (var connect = ConnectToDb.GetDbConnection())
            {
                connect.Open();
                if (!String.IsNullOrEmpty(regnum) && versionNumber != 0)
                {
                    var selectGetMax =
                        $"SELECT MAX(version_number) as m FROM {Program.Prefix}od_contract WHERE regnum = @regnum";
                    var cmd = new MySqlCommand(selectGetMax, connect);
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@regnum", regnum);
                    var resultm = cmd.ExecuteScalar();
                    var maxNumber = (int?) (!Convert.IsDBNull(resultm) ? resultm : null);
                    if (maxNumber != null)
                    {
                        if (versionNumber > maxNumber)
                        {
                            var updateC = $"UPDATE {Program.Prefix}od_contract SET cancel=1 WHERE regnum= @regnum";
                            var cmd2 = new MySqlCommand(updateC, connect);
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
                var customerRegnumber = ((string) J44.SelectToken("export.contract.customer.regNum") ?? "").Trim();
                if (!String.IsNullOrEmpty(customerRegnumber))
                {
                    var selectCustomer =
                        $"SELECT id FROM {Program.Prefix}od_customer WHERE regNumber = @customer_regnumber";
                    var cmd3 = new MySqlCommand(selectCustomer, connect);
                    cmd3.Prepare();
                    cmd3.Parameters.AddWithValue("@customer_regnumber", customerRegnumber);
                    var reader = cmd3.ExecuteReader();
                    var resRead = reader.HasRows;
                    if (resRead)
                    {
                        reader.Read();
                        idCustomer = reader.GetInt32("id");
                        reader.Close();
                    }
                    else
                    {
                        reader.Close();
                        var kppCustomer = ((string) J44.SelectToken("export.contract.customer.kpp") ?? "").Trim();
                        var innCustomer = ((string) J44.SelectToken("export.contract.customer.inn") ?? "").Trim();
                        var fullNameCustomer =
                            ((string) J44.SelectToken("export.contract.customer.fullName") ?? "").Trim();
                        var postalAddressCustomer = "";
                        var contractsCountCustomer = 1;
                        var contractsSumCustomer = contractPrice;
                        var contracts223CountCustomer = 0;
                        var contracts223SumCustomer = 0.0m;
                        var ogrnCustomer = "";
                        var regionCodeCustomer = "";
                        var phoneCustomer = "";
                        var faxCustomer = "";
                        var emailCustomer = "";
                        var contactNameCustomer = "";
                        var addCustomer =
                            $"INSERT INTO {Program.Prefix}od_customer SET regNumber = @customer_regnumber, inn = @inn_customer, " +
                            $"kpp = @kpp_customer, contracts_count = @contracts_count_customer, contracts223_count = @contracts223_count_customer," +
                            $"contracts_sum = @contracts_sum_customer, contracts223_sum = @contracts223_sum_customer," +
                            $"ogrn = @ogrn_customer, region_code = @region_code_customer, full_name = @full_name_customer," +
                            $"postal_address = @postal_address_customer, phone = @phone_customer, fax = @fax_customer," +
                            $"email = @email_customer, contact_name = @contact_name_customer";
                        var cmd4 = new MySqlCommand(addCustomer, connect);
                        cmd4.Prepare();
                        cmd4.Parameters.AddWithValue("@customer_regnumber", customerRegnumber);
                        cmd4.Parameters.AddWithValue("@inn_customer", innCustomer);
                        cmd4.Parameters.AddWithValue("@kpp_customer", kppCustomer);
                        cmd4.Parameters.AddWithValue("@contracts_count_customer", contractsCountCustomer);
                        cmd4.Parameters.AddWithValue("@contracts223_count_customer", contracts223CountCustomer);
                        cmd4.Parameters.AddWithValue("@contracts_sum_customer", contractsSumCustomer);
                        cmd4.Parameters.AddWithValue("@contracts223_sum_customer", contracts223SumCustomer);
                        cmd4.Parameters.AddWithValue("@ogrn_customer", ogrnCustomer);
                        cmd4.Parameters.AddWithValue("@region_code_customer", regionCodeCustomer);
                        cmd4.Parameters.AddWithValue("@full_name_customer", fullNameCustomer);
                        cmd4.Parameters.AddWithValue("@postal_address_customer", postalAddressCustomer);
                        cmd4.Parameters.AddWithValue("@phone_customer", phoneCustomer);
                        cmd4.Parameters.AddWithValue("@fax_customer", faxCustomer);
                        cmd4.Parameters.AddWithValue("@email_customer", emailCustomer);
                        cmd4.Parameters.AddWithValue("@contact_name_customer", contactNameCustomer);
                        var addC = cmd4.ExecuteNonQuery();
                        idCustomer = (int) cmd4.LastInsertedId;

                        AddCustomerEvent?.Invoke(addC);
                    }
                }
                var testSup = J44.SelectToken("export.contract.suppliers");
                if (testSup != null && testSup.Type != JTokenType.Null)
                {
                    var suppliers = testSup.SelectToken("supplier") ?? new JArray();
                    var enumerable = suppliers as IList<JToken> ?? suppliers.ToList();
                    if (enumerable.Any())
                    {
                        var sup = enumerable.First();
                        if (sup.Type == JTokenType.Property)
                        {
                            sup = enumerable.First().Parent;
                        }
                        var supplierInn = ((string) sup.SelectToken("legalEntityRF.INN") ?? "").Trim();
                        if (String.IsNullOrEmpty(supplierInn))
                        {
                            supplierInn = ((string) sup.SelectToken("individualPersonRF.INN") ?? "").Trim();
                        }
                        if (String.IsNullOrEmpty(supplierInn))
                        {
                            supplierInn = ((string) sup.SelectToken("legalEntityForeignState.INN") ?? "").Trim();
                        }
                        if (String.IsNullOrEmpty(supplierInn))
                        {
                            supplierInn =
                                ((string) sup.SelectToken("legalEntityForeignState.taxPayerCode") ?? "").Trim();
                        }
                        if (String.IsNullOrEmpty(supplierInn))
                        {
                            supplierInn =
                            ((string) sup.SelectToken("individualPersonForeignState.registerInRFTaxBodies.INN") ??
                             "").Trim();
                        }
                        if (String.IsNullOrEmpty(supplierInn))
                        {
                            supplierInn = ((string) sup.SelectToken("individualPersonForeignState.taxPayerCode") ?? "")
                                .Trim();
                        }
                        if (String.IsNullOrEmpty(supplierInn))
                        {
                            supplierInn = ((string) sup.SelectToken("individualPersonForeignState.INN") ?? "").Trim();
                        }
                        if (!String.IsNullOrEmpty(supplierInn))
                        {
                            var kppSupplier = ((string) sup.SelectToken("legalEntityRF.KPP") ?? "").Trim();
                            if (String.IsNullOrEmpty(kppSupplier))
                            {
                                kppSupplier = ((string) sup.SelectToken("individualPersonRF.KPP") ?? "").Trim();
                            }
                            if (String.IsNullOrEmpty(kppSupplier))
                            {
                                kppSupplier =
                                ((string) sup.SelectToken(
                                     "individualPersonForeignState.registerInRFTaxBodies.KPP") ?? "").Trim();
                            }
                            var selectSupplier =
                                $"SELECT id FROM {Program.Prefix}od_supplier WHERE inn = @supplier_inn AND kpp = @kpp_supplier";
                            var cmd5 = new MySqlCommand(selectSupplier, connect);
                            cmd5.Prepare();
                            cmd5.Parameters.AddWithValue("@supplier_inn", supplierInn);
                            cmd5.Parameters.AddWithValue("@kpp_supplier", kppSupplier);
                            var reader = cmd5.ExecuteReader();
                            var resRead = reader.HasRows;
                            if (resRead)
                            {
                                reader.Read();
                                idSupplier = reader.GetInt32("id");
                                reader.Close();
                            }
                            else
                            {
                                reader.Close();
                                var contactphoneSupplier =
                                    ((string) sup.SelectToken("legalEntityRF.contactPhone") ?? "").Trim();
                                if (String.IsNullOrEmpty(contactphoneSupplier))
                                {
                                    contactphoneSupplier =
                                        ((string) sup.SelectToken("individualPersonRF.contactPhone") ?? "").Trim();
                                }
                                if (String.IsNullOrEmpty(contactphoneSupplier))
                                {
                                    contactphoneSupplier =
                                    ((string) sup.SelectToken(
                                         "legalEntityForeignState.placeOfStayInRegCountry.contactPhone") ?? "").Trim();
                                }
                                if (String.IsNullOrEmpty(contactphoneSupplier))
                                {
                                    contactphoneSupplier =
                                        ((string) sup.SelectToken(
                                             "individualPersonForeignState.placeOfStayInRegCountry.contactPhone") ?? "")
                                        .Trim();
                                }
                                var contactemailSupplier =
                                    ((string) sup.SelectToken("legalEntityRF.contactEMail") ?? "").Trim();
                                if (String.IsNullOrEmpty(contactemailSupplier))
                                {
                                    contactemailSupplier =
                                        ((string) sup.SelectToken("individualPersonRF.contactEMail") ?? "").Trim();
                                }
                                if (String.IsNullOrEmpty(contactemailSupplier))
                                {
                                    contactemailSupplier =
                                    ((string) sup.SelectToken(
                                         "legalEntityForeignState.placeOfStayInRegCountry.contactEMail") ?? "").Trim();
                                }
                                if (String.IsNullOrEmpty(contactemailSupplier))
                                {
                                    contactemailSupplier =
                                        ((string) sup.SelectToken(
                                             "individualPersonForeignState.placeOfStayInRegCountry.contactEMail") ?? "")
                                        .Trim();
                                }
                                var organizationnameSupplier =
                                    ((string) sup.SelectToken("legalEntityRF.fullName") ?? "").Trim();
                                if (String.IsNullOrEmpty(organizationnameSupplier))
                                {
                                    organizationnameSupplier =
                                        ((string) sup.SelectToken("legalEntityForeignState.fullName") ?? "").Trim();
                                }
                                if (String.IsNullOrEmpty(organizationnameSupplier))
                                {
                                    var lastname = ((string) sup.SelectToken("individualPersonRF.lastName") ?? "")
                                        .Trim();
                                    if (String.IsNullOrEmpty(lastname))
                                    {
                                        lastname = ((string) sup.SelectToken("individualPersonForeignState.lastName") ??
                                                    "").Trim();
                                    }
                                    var firsname = ((string) sup.SelectToken("individualPersonRF.firstName") ?? "")
                                        .Trim();
                                    if (String.IsNullOrEmpty(firsname))
                                    {
                                        firsname =
                                            ((string) sup.SelectToken("individualPersonForeignState.firstName") ?? "")
                                            .Trim();
                                    }
                                    var middlename =
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
                                        organizationnameSupplier = $"{lastname} {firsname} {middlename}".Trim();
                                    }
                                }
                                var contractsCountSupplier = 1;
                                var contractsSumSupplier = contractPrice;
                                var contracts223CountSupplier = 0;
                                var contracts223SumSupplier = 0.0m;
                                var ogrnSupplier = "";
                                var regionCodeSupplier = "";
                                var postalAddressSupplier = "";
                                var contactfaxSupplier = "";
                                var contactNameSupplier = "";
                                var addSupplier =
                                    $"INSERT INTO {Program.Prefix}od_supplier SET inn = @supplier_inn, kpp = @kpp_supplier, " +
                                    $"contracts_count = @contracts_count, " +
                                    $"contracts223_count = @contracts223_count, contracts_sum = @contracts_sum, " +
                                    $"contracts223_sum = @contracts223_sum, ogrn = @ogrn,region_code = @region_code, " +
                                    $"organizationName = @organizationName,postal_address = @postal_address, " +
                                    $"contactPhone = @contactPhone,contactFax = @contactFax, " +
                                    $"contactEMail = @contactEMail,contact_name = @contact_name";
                                var cmd6 = new MySqlCommand(addSupplier, connect);
                                cmd6.Prepare();
                                cmd6.Parameters.AddWithValue("@supplier_inn", supplierInn);
                                cmd6.Parameters.AddWithValue("@kpp_supplier", kppSupplier);
                                cmd6.Parameters.AddWithValue("@contracts_count", contractsCountSupplier);
                                cmd6.Parameters.AddWithValue("@contracts223_count", contracts223CountSupplier);
                                cmd6.Parameters.AddWithValue("@contracts_sum", contractsSumSupplier);
                                cmd6.Parameters.AddWithValue("@contracts223_sum", contracts223SumSupplier);
                                cmd6.Parameters.AddWithValue("@ogrn", ogrnSupplier);
                                cmd6.Parameters.AddWithValue("@region_code", regionCodeSupplier);
                                cmd6.Parameters.AddWithValue("@organizationName", organizationnameSupplier);
                                cmd6.Parameters.AddWithValue("@postal_address", postalAddressSupplier);
                                cmd6.Parameters.AddWithValue("@contactPhone", contactphoneSupplier);
                                cmd6.Parameters.AddWithValue("@contactFax", contactfaxSupplier);
                                cmd6.Parameters.AddWithValue("@contactEMail", contactemailSupplier);
                                cmd6.Parameters.AddWithValue("@contact_name", contactNameSupplier);
                                var addS = cmd6.ExecuteNonQuery();
                                idSupplier = (int) cmd6.LastInsertedId;
                                AddSupplierEvent?.Invoke(addS);
                            }
                        }
                    }
                }
                var idOdContract = 0;
                var selectContract =
                    $"SELECT id FROM {Program.Prefix}od_contract WHERE id_contract = @id_contract AND region_code = @region_code";
                var cmd7 = new MySqlCommand(selectContract, connect);
                cmd7.Prepare();
                cmd7.Parameters.AddWithValue("@id_contract", idContract);
                cmd7.Parameters.AddWithValue("@region_code", Region);
                var readerC = cmd7.ExecuteReader();
                if (readerC.HasRows)
                {
                    readerC.Read();
                    idOdContract = readerC.GetInt32("id");
                    readerC.Close();
                    var deleteProducts =
                        $"DELETE FROM {Program.Prefix}od_contract_product WHERE id_od_contract = @id_od_contract";
                    var cmd8 = new MySqlCommand(deleteProducts, connect);
                    cmd8.Prepare();
                    cmd8.Parameters.AddWithValue("@id_od_contract", idOdContract);
                    cmd8.ExecuteNonQuery();
                    var updateContract =
                        $"UPDATE {Program.Prefix}od_contract SET p_number = @p_number, regnum = @regnum, " +
                        $"current_contract_stage = @current_contract_stage, placing = @placing, " +
                        $"region_code = @region_code, url = @url, sign_date = @sign_date, " +
                        $"single_customer_reason_code = @single_customer_reason_code, single_customer_reason_name = @single_customer_reason_name, " +
                        $"fz = @fz, notification_number = @notification_number, lot_number = @lot_number, " +
                        $"contract_price = @contract_price, currency = @currency, version_number = @version_number, " +
                        $"execution_start_date = @execution_start_date, execution_end_date = @execution_end_date, " +
                        $"id_customer = @id_customer, id_supplier = @id_supplier, xml = @xml WHERE id = @id_od_contract";
                    var cmd9 = new MySqlCommand(updateContract, connect);
                    cmd9.Prepare();
                    cmd9.Parameters.AddWithValue("@p_number", pNumber);
                    cmd9.Parameters.AddWithValue("@regnum", regnum);
                    cmd9.Parameters.AddWithValue("@current_contract_stage", currentContractStage);
                    cmd9.Parameters.AddWithValue("@placing", placing);
                    cmd9.Parameters.AddWithValue("@region_code", Region);
                    cmd9.Parameters.AddWithValue("@url", url);
                    cmd9.Parameters.AddWithValue("@sign_date", signDate);
                    cmd9.Parameters.AddWithValue("@single_customer_reason_code", singleCustomerReasonCode);
                    cmd9.Parameters.AddWithValue("@single_customer_reason_name", singleCustomerReasonName);
                    cmd9.Parameters.AddWithValue("@fz", fz);
                    cmd9.Parameters.AddWithValue("@notification_number", notificationNumber);
                    cmd9.Parameters.AddWithValue("@lot_number", lotNumber);
                    cmd9.Parameters.AddWithValue("@contract_price", contractPrice);
                    cmd9.Parameters.AddWithValue("@currency", currency);
                    cmd9.Parameters.AddWithValue("@version_number", versionNumber);
                    cmd9.Parameters.AddWithValue("@execution_start_date", executionStartDate);
                    cmd9.Parameters.AddWithValue("@execution_end_date", executionEndDate);
                    cmd9.Parameters.AddWithValue("@id_customer", idCustomer);
                    cmd9.Parameters.AddWithValue("@id_supplier", idSupplier);
                    cmd9.Parameters.AddWithValue("@xml", xml);
                    cmd9.Parameters.AddWithValue("@id_od_contract", idOdContract);
                    var updC = cmd9.ExecuteNonQuery();
                    UpdateContractEvent?.Invoke(updC);
                }
                else
                {
                    readerC.Close();
                    var insertContract =
                        $"INSERT INTO {Program.Prefix}od_contract SET id_contract = @id_contract, p_number = @p_number, regnum = @regnum, " +
                        $"current_contract_stage = @current_contract_stage, placing = @placing, " +
                        $"region_code = @region_code, url = @url, sign_date = @sign_date, " +
                        $"single_customer_reason_code = @single_customer_reason_code, single_customer_reason_name = @single_customer_reason_name, " +
                        $"fz = @fz, notification_number = @notification_number, lot_number = @lot_number, " +
                        $"contract_price = @contract_price, currency = @currency, version_number = @version_number, " +
                        $"execution_start_date = @execution_start_date, execution_end_date = @execution_end_date, " +
                        $"id_customer = @id_customer, id_supplier = @id_supplier, cancel = @cancel, xml = @xml";
                    var cmd10 = new MySqlCommand(insertContract, connect);
                    cmd10.Prepare();
                    cmd10.Parameters.AddWithValue("@id_contract", idContract);
                    cmd10.Parameters.AddWithValue("@p_number", pNumber);
                    cmd10.Parameters.AddWithValue("@regnum", regnum);
                    cmd10.Parameters.AddWithValue("@current_contract_stage", currentContractStage);
                    cmd10.Parameters.AddWithValue("@placing", placing);
                    cmd10.Parameters.AddWithValue("@region_code", Region);
                    cmd10.Parameters.AddWithValue("@url", url);
                    cmd10.Parameters.AddWithValue("@sign_date", signDate);
                    cmd10.Parameters.AddWithValue("@single_customer_reason_code", singleCustomerReasonCode);
                    cmd10.Parameters.AddWithValue("@single_customer_reason_name", singleCustomerReasonName);
                    cmd10.Parameters.AddWithValue("@fz", fz);
                    cmd10.Parameters.AddWithValue("@notification_number", notificationNumber);
                    cmd10.Parameters.AddWithValue("@lot_number", lotNumber);
                    cmd10.Parameters.AddWithValue("@contract_price", contractPrice);
                    cmd10.Parameters.AddWithValue("@currency", currency);
                    cmd10.Parameters.AddWithValue("@version_number", versionNumber);
                    cmd10.Parameters.AddWithValue("@execution_start_date", executionStartDate);
                    cmd10.Parameters.AddWithValue("@execution_end_date", executionEndDate);
                    cmd10.Parameters.AddWithValue("@id_customer", idCustomer);
                    cmd10.Parameters.AddWithValue("@id_supplier", idSupplier);
                    cmd10.Parameters.AddWithValue("@cancel", cancel);
                    cmd10.Parameters.AddWithValue("@xml", xml);
                    var addContr = cmd10.ExecuteNonQuery();
                    idOdContract = (int) cmd10.LastInsertedId;
                    AddContractEvent?.Invoke(addContr);
                }
                var testProd = J44.SelectToken("export.contract.products");
                if (testProd != null && testProd.Type != JTokenType.Null)
                {
                    var products = testProd.SelectToken("product");
                    if (products.Any())
                    {
                        var listP = new List<JToken>();
                        if (products.Type == JTokenType.Array)
                        {
                            foreach (var p in products)
                            {
                                listP.Add(p);
                            }
                        }
                        else
                        {
                            listP.Add(products);
                        }

                        if (listP.Count == 0)
                        {
                            Log.Logger("У контракта нет продуктов", File);
                            return;
                        }
                        var transaction = connect.BeginTransaction();

                        foreach (var prod in listP)
                        {
                            var okpd2GroupCode = 0;
                            var okpd2GroupLevel1Code = "";
                            var okpdGroupCode = 0;
                            var okpdGroupLevel1Code = "";
                            var nameP = ((string) prod.SelectToken("name") ?? "").Trim();
                            nameP = Regex.Replace(nameP, @"\t|\n|\r", "");
                            if (String.IsNullOrEmpty(nameP))
                                nameP = "Нет названия";
                            var okpd2Code = ((string) prod.SelectToken("OKPD2.code") ?? "").Trim();
                            if (!String.IsNullOrEmpty(okpd2Code))
                            {
                                GetOkpd(okpd2Code, out okpd2GroupCode, out okpd2GroupLevel1Code);
                            }
                            var okpd2Name = ((string) prod.SelectToken("OKPD2.name") ?? "").Trim();
                            var okpdCode = ((string) prod.SelectToken("OKPD.code") ?? "").Trim();
                            if (!String.IsNullOrEmpty(okpdCode))
                            {
                                GetOkpd(okpdCode, out okpdGroupCode, out okpdGroupLevel1Code);
                            }
                            var okpdName = ((string) prod.SelectToken("OKPD.name") ?? "").Trim();
                            var price = ((string) prod.SelectToken("price") ?? "").Trim();
                            /*decimal price = decimal.Parse(price_s, NumberStyles.Any, CultureInfo.InvariantCulture);*/
                            var quantity = ((string) prod.SelectToken("quantity") ?? "").Trim();
                            /*decimal quantity =
                                decimal.Parse(quantity_s, NumberStyles.Any, CultureInfo.InvariantCulture);*/
                            var sumP = ((string) prod.SelectToken("sum") ?? "").Trim();
                            /*decimal sum_p = decimal.Parse(sum_p_s, NumberStyles.Any, CultureInfo.InvariantCulture);*/
                            var sid = ((string) prod.SelectToken("sid") ?? "").Trim();
                            var okei = ((string) prod.SelectToken("OKEI.nationalCode") ?? "").Trim();
                            var insertProd =
                                $"INSERT INTO {Program.Prefix}od_contract_product SET id_od_contract = @id_od_contract, " +
                                $"name = @name_p, okpd2_code = @okpd2_code, okpd_code = @okpd_code, okpd2_group_code = @okpd2_group_code, " +
                                $"okpd_group_code = @okpd_group_code, okpd2_group_level1_code = @okpd2_group_level1_code, " +
                                $"okpd_group_level1_code = @okpd_group_level1_code, price = @price, okpd2_name = @okpd2_name, " +
                                $"okpd_name = @okpd_name, quantity = @quantity, okei = @okei, sum = @sum, sid = @sid";
                            var cmd11 = new MySqlCommand(insertProd, connect);
                            cmd11.Transaction = transaction;
                            cmd11.Prepare();
                            cmd11.Parameters.AddWithValue("@id_od_contract", idOdContract);
                            cmd11.Parameters.AddWithValue("@name_p", nameP);
                            cmd11.Parameters.AddWithValue("@okpd2_code", okpd2Code);
                            cmd11.Parameters.AddWithValue("@okpd_code", okpdCode);
                            cmd11.Parameters.AddWithValue("@okpd2_group_code", okpd2GroupCode);
                            cmd11.Parameters.AddWithValue("@okpd_group_code", okpdGroupCode);
                            cmd11.Parameters.AddWithValue("@okpd2_group_level1_code", okpd2GroupLevel1Code);
                            cmd11.Parameters.AddWithValue("@okpd_group_level1_code", okpdGroupLevel1Code);
                            cmd11.Parameters.AddWithValue("@price", price);
                            cmd11.Parameters.AddWithValue("@okpd2_name", okpd2Name);
                            cmd11.Parameters.AddWithValue("@okpd_name", okpdName);
                            cmd11.Parameters.AddWithValue("@quantity", quantity);
                            cmd11.Parameters.AddWithValue("@okei", okei);
                            cmd11.Parameters.AddWithValue("@sum", sumP);
                            cmd11.Parameters.AddWithValue("@sid", sid);
                            var addP = cmd11.ExecuteNonQuery();
                            AddProductEvent?.Invoke(addP);
                        }

                        transaction.Commit();
                    }
                }
            }
        }

        public void AddProductAsync()
        {
        }
    }
}