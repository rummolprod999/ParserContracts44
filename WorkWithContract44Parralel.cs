using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ParserContracts44
{
    public class WorkWithContract44Parralel: WorkWithContract44
    {
        public event AddData AddSupplierEvent;
        public event AddData AddCustomerEvent;
        public event AddData UpdateContractEvent;
        public event AddData AddContractEvent;
        public event AddData AddProductEvent;
        public int IdOdContract;
        public List<JToken> ListP = new List<JToken>();
        private object _locker = new object();
        private object _locker2 = new object();


        public WorkWithContract44Parralel(JObject json, string f, string r):base(json, f, r)
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
            var dopInfo = J44.SelectToken("export.contract.enforcement")?.ToString() ?? "{}";
            var external_Id = ((string) J44.SelectToken("export.contract.externalId") ?? "").Trim();
            var type_eis = "contract";
            var type_Fz = "44";
            var placement_Date = (JsonConvert.SerializeObject(J44.SelectToken("export.contract.placementDate") ?? "") ??
                                  "").Trim('"');
            var publish_Date = (JsonConvert.SerializeObject(J44.SelectToken("export.contract.publishDate") ?? "") ??
                                "").Trim('"');
            var modification_description = ((string) J44.SelectToken("export.contract.modification.errorCorrection.description") ?? "").Trim();
            var purchase_Code = ((string) J44.SelectToken("export.contract.foundation.fcsOrder.order.purchaseCode") ?? "").Trim();
            var contractProject_Number = ((string) J44.SelectToken("export.contract.foundation.fcsOrder.order.contractProjectNumber") ?? "").Trim();
            var plan2020_Number = ((string) J44.SelectToken("export.contract.foundation.fcsOrder.order.tenderPlan2020Info.plan2020Number") ?? "").Trim();
            var position2020_Number = ((string) J44.SelectToken("export.contract.foundation.fcsOrder.order.tenderPlan2020Info.position2020Number") ?? "").Trim();
            var id_placer_org = 0;
            var protocol_Date = ((string) J44.SelectToken("export.contract.protocolDate") ?? "").Trim();
            var document_Base = ((string) J44.SelectToken("export.contract.documentBase") ?? "").Trim();
            var document_Code = ((string) J44.SelectToken("export.contract.documentCode") ?? "").Trim();
            var contract_Subject = ((string) J44.SelectToken("export.contract.contractSubject") ?? "").Trim();
            var price_Type = ((string) J44.SelectToken("export.contract.priceInfo.priceType") ?? "").Trim();
            var price_currency_code = ((string) J44.SelectToken("export.contract.priceInfo.currency.code") ?? "").Trim();
            var price_RUR = (decimal?) J44.SelectToken("export.contract.priceInfo.priceRUR") ?? 0.00m;
            var price_VAT = (decimal?) J44.SelectToken("export.contract.priceInfo.priceVAT") ?? 0.00m;
            var price_VATRUR = (decimal?) J44.SelectToken("export.contract.priceInfo.priceVATRUR") ?? 0.00m;
            var print_Form = ((string) J44.SelectToken("export.contract.printForm.url") ?? "").Trim();
            try
            {
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
                            $"SELECT id FROM od_customer WHERE regNumber = @customer_regnumber";
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
                            var shortNameCustomer =
                                ((string) J44.SelectToken("export.contract.customer.shortName") ?? "").Trim();
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
                            var customerDopInfo = J44.SelectToken("export.contract.customer.customerAccountsDetails")
                                ?.ToString() ?? "{}";
                            var addCustomer =
                                $"INSERT INTO od_customer SET regNumber = @customer_regnumber, inn = @inn_customer, kpp = @kpp_customer, contracts_count = @contracts_count_customer, contracts223_count = @contracts223_count_customer,contracts_sum = @contracts_sum_customer, contracts223_sum = @contracts223_sum_customer,ogrn = @ogrn_customer, region_code = @region_code_customer, full_name = @full_name_customer,postal_address = @postal_address_customer, phone = @phone_customer, fax = @fax_customer,email = @email_customer, contact_name = @contact_name_customer, short_name = @short_name, dop_info = @dop_info";
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
                            cmd4.Parameters.AddWithValue("@short_name", shortNameCustomer);
                            cmd4.Parameters.AddWithValue("@dop_info", customerDopInfo);
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
                                    $"SELECT id FROM od_supplier WHERE inn = @supplier_inn AND kpp = @kpp_supplier";
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
                                    var organizationshortnameSupplier =
                                        ((string) sup.SelectToken("legalEntityRF.shortName") ?? "").Trim();
                                    if (String.IsNullOrEmpty(organizationshortnameSupplier))
                                    {
                                        organizationshortnameSupplier =
                                            ((string) sup.SelectToken("legalEntityForeignState.shortName") ?? "").Trim();
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
                                    var supplierDopInfo = "";
                                    var addSupplier =
                                        $"INSERT INTO od_supplier SET inn = @supplier_inn, kpp = @kpp_supplier, contracts_count = @contracts_count, contracts223_count = @contracts223_count, contracts_sum = @contracts_sum, contracts223_sum = @contracts223_sum, ogrn = @ogrn,region_code = @region_code, organizationName = @organizationName,postal_address = @postal_address, contactPhone = @contactPhone, contactFax = @contactFax, contactEMail = @contactEMail, contact_name = @contact_name, organizationShortName = @organizationShortName";
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
                                    cmd6.Parameters.AddWithValue("@organizationShortName", organizationshortnameSupplier);
                                    var addS = cmd6.ExecuteNonQuery();
                                    idSupplier = (int) cmd6.LastInsertedId;
                                    AddSupplierEvent?.Invoke(addS);
                                }
                            }
                        }
                    }
                    var testSupNew = J44.SelectToken("export.contract.suppliersInfo");
                    if (testSupNew != null && testSupNew.Type != JTokenType.Null)
                    {
                        var suppliers = testSupNew.SelectToken("supplierInfo") ?? new JArray();
                        var enumerable = suppliers as IList<JToken> ?? suppliers.ToList();
                        if (enumerable.Any())
                        {
                            var sup = enumerable.First();
                            if (sup.Type == JTokenType.Property)
                            {
                                sup = enumerable.First().Parent;
                            }
                            var supplierInn = ((string) sup.SelectToken("legalEntityRF.EGRULInfo.INN") ?? "").Trim();
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
                                var kppSupplier = ((string) sup.SelectToken("legalEntityRF.EGRULInfo.KPP") ?? "").Trim();
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
                                    $"SELECT id FROM od_supplier WHERE inn = @supplier_inn AND kpp = @kpp_supplier";
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
                                        ((string) sup.SelectToken("legalEntityRF.EGRULInfo.otherInfo.contactPhone") ?? "").Trim();
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
                                        ((string) sup.SelectToken("legalEntityRF.EGRULInfo.otherInfo.contactEMail") ?? "").Trim();
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
                                        ((string) sup.SelectToken("legalEntityRF.EGRULInfo.fullName") ?? "").Trim();
                                    if (String.IsNullOrEmpty(organizationnameSupplier))
                                    {
                                        organizationnameSupplier =
                                            ((string) sup.SelectToken("legalEntityForeignState.fullName") ?? "").Trim();
                                    }
                                    var organizationshortnameSupplier =
                                        ((string) sup.SelectToken("legalEntityRF.EGRULInfo.shortName") ?? "").Trim();
                                    if (String.IsNullOrEmpty(organizationshortnameSupplier))
                                    {
                                        organizationshortnameSupplier =
                                            ((string) sup.SelectToken("legalEntityForeignState.shortName") ?? "").Trim();
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
                                    var supplierDopInfo = sup.SelectToken("supplierAccountsDetails")?.ToString() ?? "{}";
                                    var addSupplier =
                                        $"INSERT INTO od_supplier SET inn = @supplier_inn, kpp = @kpp_supplier, contracts_count = @contracts_count, contracts223_count = @contracts223_count, contracts_sum = @contracts_sum, contracts223_sum = @contracts223_sum, ogrn = @ogrn,region_code = @region_code, organizationName = @organizationName,postal_address = @postal_address, contactPhone = @contactPhone, contactFax = @contactFax, contactEMail = @contactEMail, contact_name = @contact_name, organizationShortName = @organizationShortName, dop_info = @dop_info";
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
                                    cmd6.Parameters.AddWithValue("@organizationShortName", organizationshortnameSupplier);
                                    cmd6.Parameters.AddWithValue("@dop_info", supplierDopInfo);
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
                        IdOdContract = idOdContract;
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
                            $"id_customer = @id_customer, id_supplier = @id_supplier, xml = @xml, dop_info = @dop_info, external_Id = @external_Id, type_eis = @type_eis, type_fz = @type_fz, placement_Date = @placement_Date, publish_Date = @publish_Date, modification_description = @modification_description,  purchase_Code = @purchase_Code, contractProject_Number = @contractProject_Number, plan2020_Number = @plan2020_Number, position2020_Number = @position2020_Number, id_placer_org = @id_placer_org, protocol_Date = @protocol_Date, document_Base = @document_Base,  document_Code = @document_Code, contract_Subject = @contract_Subject, price_Type = @price_Type, price_currency_code = @price_currency_code, price_RUR = @price_RUR, price_VAT = @price_VAT, price_VATRUR = @price_VATRUR, print_Form = @print_Form  WHERE id = @id_od_contract";
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
                        cmd9.Parameters.AddWithValue("@dop_info", dopInfo);
                        cmd9.Parameters.AddWithValue("@external_Id", external_Id);
                        cmd9.Parameters.AddWithValue("@type_eis", type_eis);
                        cmd9.Parameters.AddWithValue("@type_fz", type_Fz);
                        cmd9.Parameters.AddWithValue("@placement_Date", placement_Date);
                        cmd9.Parameters.AddWithValue("@publish_Date", publish_Date);
                        cmd9.Parameters.AddWithValue("@modification_description", modification_description);
                        cmd9.Parameters.AddWithValue("@purchase_Code", purchase_Code);
                        cmd9.Parameters.AddWithValue("@contractProject_Number", contractProject_Number);
                        cmd9.Parameters.AddWithValue("@plan2020_Number", plan2020_Number);
                        cmd9.Parameters.AddWithValue("@position2020_Number", position2020_Number);
                        cmd9.Parameters.AddWithValue("@id_placer_org", id_placer_org);
                        cmd9.Parameters.AddWithValue("@protocol_Date", protocol_Date);
                        cmd9.Parameters.AddWithValue("@document_Base", document_Base);
                        cmd9.Parameters.AddWithValue("@document_Code", document_Code);
                        cmd9.Parameters.AddWithValue("@contract_Subject", contract_Subject);
                        cmd9.Parameters.AddWithValue("@price_Type", price_Type);
                        cmd9.Parameters.AddWithValue("@price_currency_code", price_currency_code);
                        cmd9.Parameters.AddWithValue("@price_RUR", price_RUR);
                        cmd9.Parameters.AddWithValue("@price_VAT", price_VAT);
                        cmd9.Parameters.AddWithValue("@price_VATRUR", price_VATRUR);
                        cmd9.Parameters.AddWithValue("@print_Form", print_Form);
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
                            $"id_customer = @id_customer, id_supplier = @id_supplier, cancel = @cancel, xml = @xml, dop_info = @dop_info, external_Id = @external_Id, type_eis = @type_eis, type_fz = @type_fz, placement_Date = @placement_Date, publish_Date = @publish_Date, modification_description = @modification_description,  purchase_Code = @purchase_Code, contractProject_Number = @contractProject_Number, plan2020_Number = @plan2020_Number, position2020_Number = @position2020_Number, id_placer_org = @id_placer_org, protocol_Date = @protocol_Date, document_Base = @document_Base,  document_Code = @document_Code, contract_Subject = @contract_Subject, price_Type = @price_Type, price_currency_code = @price_currency_code, price_RUR = @price_RUR, price_VAT = @price_VAT, price_VATRUR = @price_VATRUR, print_Form = @print_Form";
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
                        cmd10.Parameters.AddWithValue("@dop_info", dopInfo);
                        cmd10.Parameters.AddWithValue("@external_Id", external_Id);
                        cmd10.Parameters.AddWithValue("@type_eis", type_eis);
                        cmd10.Parameters.AddWithValue("@type_fz", type_Fz);
                        cmd10.Parameters.AddWithValue("@placement_Date", placement_Date);
                        cmd10.Parameters.AddWithValue("@publish_Date", publish_Date);
                        cmd10.Parameters.AddWithValue("@modification_description", modification_description);
                        cmd10.Parameters.AddWithValue("@purchase_Code", purchase_Code);
                        cmd10.Parameters.AddWithValue("@contractProject_Number", contractProject_Number);
                        cmd10.Parameters.AddWithValue("@plan2020_Number", plan2020_Number);
                        cmd10.Parameters.AddWithValue("@position2020_Number", position2020_Number);
                        cmd10.Parameters.AddWithValue("@id_placer_org", id_placer_org);
                        cmd10.Parameters.AddWithValue("@protocol_Date", protocol_Date);
                        cmd10.Parameters.AddWithValue("@document_Base", document_Base);
                        cmd10.Parameters.AddWithValue("@document_Code", document_Code);
                        cmd10.Parameters.AddWithValue("@contract_Subject", contract_Subject);
                        cmd10.Parameters.AddWithValue("@price_Type", price_Type);
                        cmd10.Parameters.AddWithValue("@price_currency_code", price_currency_code);
                        cmd10.Parameters.AddWithValue("@price_RUR", price_RUR);
                        cmd10.Parameters.AddWithValue("@price_VAT", price_VAT);
                        cmd10.Parameters.AddWithValue("@price_VATRUR", price_VATRUR);
                        cmd10.Parameters.AddWithValue("@print_Form", print_Form);
                        var addContr = cmd10.ExecuteNonQuery();
                        idOdContract = (int) cmd10.LastInsertedId;
                        IdOdContract = idOdContract;
                        AddContractEvent?.Invoke(addContr);
                    }
                    var deliPlaces = GetElements(J44, "export.contract.deliveryPlaceInfo.byKLADRInfo.deliveryPlace");
                    deliPlaces.ForEach(d =>
                    {
                        var delPlace = ((string) d.ToString() ?? "").Trim();
                        if (!String.IsNullOrEmpty(delPlace))
                        {
                            var insertProd =
                                $"INSERT INTO od_contract_deliveryPlace (id, id_od_contract, delivery_Place) VALUES (NULL, @id_od_contract, @deliv)";
                            var cmd11 = new MySqlCommand(insertProd, connect);
                            cmd11.Prepare();
                            cmd11.Parameters.AddWithValue("@id_od_contract", idOdContract);
                            cmd11.Parameters.AddWithValue("@deliv", delPlace);
                            cmd11.ExecuteNonQuery();
                        }
                    });
                    var deliPlace = ((string) J44.SelectToken("export.contract.deliveryPlaceInfo.byKLADRInfo.deliveryPlace") ?? "").Trim();
                    if (!String.IsNullOrEmpty(deliPlace))
                    {
                        var insertProd =
                            $"INSERT INTO od_contract_deliveryPlace (id, id_od_contract, delivery_Place) VALUES (NULL, @id_od_contract, @deliv)";
                        var cmd11 = new MySqlCommand(insertProd, connect);
                        cmd11.Prepare();
                        cmd11.Parameters.AddWithValue("@id_od_contract", idOdContract);
                        cmd11.Parameters.AddWithValue("@deliv", deliPlace);
                        cmd11.ExecuteNonQuery();
                    }
                    var attach = GetElements(J44, "export.contract.attachments.attachment");
                    attach.AddRange(GetElements(J44, "export.contract.scanDocuments.attachment"));
                    attach.ForEach(a =>
                    {
                        var publishedContentId = ((string) a.SelectToken("publishedContentId") ?? "").Trim();
                        var fileName = ((string) a.SelectToken("fileName") ?? "").Trim();
                        var docDescription = ((string) a.SelectToken("docDescription") ?? "").Trim();
                        var docRegNumber = ((string) a.SelectToken("docRegNumber") ?? "").Trim();
                        var urlA = ((string) a.SelectToken("url") ?? "").Trim();
                        var att =
                            $"INSERT INTO od_contract_attach(id_attach, id_od_contract, publishedContentId, fileName, url, description) VALUES (NULL,@id_od_contract,@publishedContentId,@fileName,@url,@description)";
                    
                        var cmd11 = new MySqlCommand(att, connect);
                        cmd11.Prepare();
                        cmd11.Parameters.AddWithValue("@id_od_contract", idOdContract);
                        cmd11.Parameters.AddWithValue("@publishedContentId", publishedContentId);
                        cmd11.Parameters.AddWithValue("@fileName", fileName);
                        cmd11.Parameters.AddWithValue("@url", urlA);
                        cmd11.Parameters.AddWithValue("@description", docDescription);
                        cmd11.ExecuteNonQuery();
                    });
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
                            else
                            {
                                ListP = listP;
                            }
                        }
                    }
                }
                Parallel.ForEach<JToken>(ListP, new ParallelOptions { MaxDegreeOfParallelism = 6 }, AddProductP);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                throw;
            }
        }

        public void AddProductP(JToken prod)
        {
            using (var connect = ConnectToDb.GetDbConnection())
            {
                var idOdContract = IdOdContract;
                connect.Open();
                var okpd2GroupCode = 0;
                var okpd2GroupLevel1Code = "";
                var okpdGroupCode = 0;
                var okpdGroupLevel1Code = "";
                var nameP = ((string) prod.SelectToken("name") ?? "").Trim();
                nameP = Regex.Replace(nameP, @"\s+", " ");
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
                    $"INSERT INTO {Program.Prefix}od_contract_product SET id_od_contract = @id_od_contract, name = @name_p, okpd2_code = @okpd2_code, okpd_code = @okpd_code, okpd2_group_code = @okpd2_group_code, okpd_group_code = @okpd_group_code, okpd2_group_level1_code = @okpd2_group_level1_code, okpd_group_level1_code = @okpd_group_level1_code, price = @price, okpd2_name = @okpd2_name, okpd_name = @okpd_name, quantity = @quantity, okei = @okei, sum = @sum, sid = @sid, dop_info = @dop_info";
                var cmd11 = new MySqlCommand(insertProd, connect);
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
                cmd11.Parameters.AddWithValue("@dop_info", prod.ToString());
                var addP = 0;
                try
                {
                    addP = cmd11.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    lock (_locker2)
                    {
                        Log.Logger("Ошибка при добавлении продукта", File, "price", price, "sid", sid, e);
                    }
                }
                AddProductEvent?.Invoke(addP);

            }
        }

        protected override void AddProduct(int d)
        {
            if (d > 0)
            {
                lock (_locker)
                {
                    Program.AddProduct++;
                }
            }
        }
        
        public List<JToken> GetElements(JToken j, string s)
        {
            var els = new List<JToken>();
            try
            {
                var elsObj = j.SelectToken(s);
                if (elsObj != null && elsObj.Type != JTokenType.Null)
                {
                    switch (elsObj.Type)
                    {
                        case JTokenType.Object:
                            els.Add(elsObj);
                            break;
                        case JTokenType.Array:
                            els.AddRange(elsObj);
                            break;
                    }
                }

                return els;
            }
            catch (JsonException e)
            {
                return j.SelectTokens(s).ToList();
            }
        }
    }
}