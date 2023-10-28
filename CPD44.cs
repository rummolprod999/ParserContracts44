using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ParserContracts44
{
    public class CPD44 : Contract
    {
        public delegate void AddData(int d);

        protected readonly JObject J44;
        protected readonly string Region;

        public CPD44(JObject json, string f, string r) : base(f)
        {
            J44 = json;
            Region = r;
            UpdateContractEvent += UpdateContract;
            AddContractEvent += AddContract;
        }

        public event AddData UpdateContractEvent;
        public event AddData AddContractEvent;

        public void Work44()
        {
            var xml = GetXml(File);
            var root = ((JObject)J44.SelectToken("export")).Last.First;
            var uuid = ((string)root.SelectToken("uuid") ?? "").Trim();
            var lkpUuid = ((string)root.SelectToken("lkpUuid") ?? "").Trim();
            if (String.IsNullOrEmpty(uuid) && String.IsNullOrEmpty(lkpUuid))
            {
                Log.Logger("У контракта нет uuid или lkpUuid", File);
                return;
            }

            using (var connect = ConnectToDb.GetDbConnection())
            {
                connect.Open();
                if (!String.IsNullOrEmpty(uuid))
                {
                    var selectTender =
                        "SELECT id FROM contractProcedureDocs44 WHERE uuid = @uuid";
                    var cmd = new MySqlCommand(selectTender, connect);
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@uuid", uuid);
                    var reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Close();
                        return;
                    }

                    reader.Close();
                }

                if (!String.IsNullOrEmpty(lkpUuid))
                {
                    var selectTender =
                        "SELECT id FROM contractProcedureDocs44 WHERE lkpUuid = @lkpUuid";
                    var cmd = new MySqlCommand(selectTender, connect);
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@lkpUuid", lkpUuid);
                    var reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Close();
                        return;
                    }

                    reader.Close();
                }

                var type = "";
                if (((string)root.SelectToken("commonInfo.docType.code") ?? "").Trim() == "REF")
                {
                    type = "REF_contractProcedureUnilateralRefusal";
                }
                else if (((string)root.SelectToken("commonInfo.docType.code") ?? "").Trim() == "NOTIF")
                {
                    type = "NOTIF_claimsCorrespondenceNotice";
                }
                else if (((string)root.SelectToken("commonInfo.docType.code") ?? "").Trim() == "CREF")
                {
                    type = "CREF_contractProcedureUnilateralRefusalCancel";
                }

                var refusalUuid = ((string)root.SelectToken("refusalUuid") ?? "").Trim();
                var parentNoticeUuid = ((string)root.SelectToken("parentNoticeUuid") ?? "").Trim();
                var refusalLkpUuid = ((string)root.SelectToken("refusalLkpUuid") ?? "").Trim();
                var parentNoticeLkpUuid = ((string)root.SelectToken("parentNoticeLkpUuid") ?? "").Trim();
                var regNumber = ((string)root.SelectToken("commonInfo.regNumber") ?? "").Trim();
                var noticeNum = ((string)root.SelectToken("commonInfo.noticeNum") ?? "").Trim();
                var estimatedEffectiveDate =
                    (JsonConvert.SerializeObject(root.SelectToken("commonInfo.estimatedEffectiveDate") ?? "") ??
                     "").Trim('"');
                var repeatedViolation = ((string)root.SelectToken("commonInfo.repeatedViolation") ?? "").Trim();
                var terminationReason = ((string)root.SelectToken("commonInfo.terminationReason") ?? "").Trim();
                var addInfo = "";
                var sup_fullName = ((string)root.SelectToken("..fullName") ?? "").Trim();
                var sup_shortName = ((string)root.SelectToken("..shortName") ?? "").Trim();
                var sup_firmName = ((string)root.SelectToken("..firmName") ?? "").Trim();
                var sup_INN = ((string)root.SelectToken("..INN") ?? "").Trim();
                var sup_KPP = ((string)root.SelectToken("..KPP") ?? "").Trim();
                var sup_registrationDate = (JsonConvert.SerializeObject(root.SelectToken("..registrationDate") ?? "") ??
                                            "").Trim('"');
                var sup_contactEMail = ((string)root.SelectToken("..contactEMail") ?? "").Trim();
                var sup_contactPhone = ((string)root.SelectToken("..contactPhone") ?? "").Trim();
                var sup_lastName = ((string)root.SelectToken("..lastName") ?? "").Trim();
                var sup_firstName = ((string)root.SelectToken("..firstName") ?? "").Trim();
                var sup_middleName = ((string)root.SelectToken("..middleName") ?? "").Trim();
                var sup_countryCode = ((string)root.SelectToken("..countryCode") ?? "").Trim();
                var sup_isIP = (bool?)root.SelectToken("isIP") ?? false;
                var code = ((string)root.SelectToken("commonInfo.docType.code") ?? "").Trim();
                var name = ((string)root.SelectToken("commonInfo.docType.name") ?? "").Trim();
                var url = ((string)root.SelectToken("printFormInfo.url") ?? "").Trim();
                var signDT = (JsonConvert.SerializeObject(root.SelectToken("commonInfo.signDT") ?? "") ??
                              "").Trim('"');
                var cancelDate = (JsonConvert.SerializeObject(root.SelectToken("..cancelDate") ?? "") ??
                                  "").Trim('"');
                var updateContract =
                    $"INSERT INTO `contractProcedureDocs44`(`id`, `type`, `uuid`, `refusalUuid`, `parentNoticeUuid`, `lkpUuid`, `refusalLkpUuid`, `parentNoticeLkpUuid`, `regNumber`, `noticeNum`, `estimatedEffectiveDate`, `repeatedViolation`, `terminationReason`, `addInfo`, `sup_fullName`, `sup_shortName`, `sup_firmName`, `sup_INN`, `sup_KPP`, `sup_registrationDate`, `sup_contactEMail`, `sup_contactPhone`, `sup_lastName`, `sup_firstName`, `sup_middleName`, `sup_countryCode`, `sup_isIP`, `code`, `name`, `url`, `signDT`, `cancelDate`) VALUES (null,@type,@uuid,@refusalUuid,@parentNoticeUuid,@lkpUuid,@refusalLkpUuid,@parentNoticeLkpUuid,@regNumber,@noticeNum,@estimatedEffectiveDate,@repeatedViolation,@terminationReason,@addInfo,@sup_fullName,@sup_shortName,@sup_firmName,@sup_INN,@sup_KPP,@sup_registrationDate,@sup_contactEMail,@sup_contactPhone,@sup_lastName,@sup_firstName,@sup_middleName,@sup_countryCode,sup_isIP,@code,@name,@url,@signDT,@cancelDate)";
                var cmd9 = new MySqlCommand(updateContract, connect);
                cmd9.Prepare();
                cmd9.Parameters.AddWithValue("@type", type);
                cmd9.Parameters.AddWithValue("@uuid", uuid);
                cmd9.Parameters.AddWithValue("@refusalUuid", refusalUuid);
                cmd9.Parameters.AddWithValue("@parentNoticeUuid", parentNoticeUuid);
                cmd9.Parameters.AddWithValue("@lkpUuid", lkpUuid);
                cmd9.Parameters.AddWithValue("@refusalLkpUuid", refusalLkpUuid);
                cmd9.Parameters.AddWithValue("@parentNoticeLkpUuid", parentNoticeLkpUuid);
                cmd9.Parameters.AddWithValue("@regNumber", regNumber);
                cmd9.Parameters.AddWithValue("@noticeNum", noticeNum);
                cmd9.Parameters.AddWithValue("@estimatedEffectiveDate", estimatedEffectiveDate);
                cmd9.Parameters.AddWithValue("@repeatedViolation", repeatedViolation);
                cmd9.Parameters.AddWithValue("@terminationReason", terminationReason);
                cmd9.Parameters.AddWithValue("@addInfo", addInfo);
                cmd9.Parameters.AddWithValue("@sup_fullName", sup_fullName);
                cmd9.Parameters.AddWithValue("@sup_shortName", sup_shortName);
                cmd9.Parameters.AddWithValue("@sup_firmName", sup_firmName);
                cmd9.Parameters.AddWithValue("@sup_INN", sup_INN);
                cmd9.Parameters.AddWithValue("@sup_KPP", sup_KPP);
                cmd9.Parameters.AddWithValue("@sup_registrationDate", sup_registrationDate);
                cmd9.Parameters.AddWithValue("@sup_contactEMail", sup_contactEMail);
                cmd9.Parameters.AddWithValue("@sup_contactPhone", sup_contactPhone);
                cmd9.Parameters.AddWithValue("@sup_lastName", sup_lastName);
                cmd9.Parameters.AddWithValue("@sup_firstName", sup_firstName);
                cmd9.Parameters.AddWithValue("@sup_middleName", sup_middleName);
                cmd9.Parameters.AddWithValue("@sup_countryCode", sup_countryCode);
                cmd9.Parameters.AddWithValue("@sup_isIP", sup_isIP);
                cmd9.Parameters.AddWithValue("@code", code);
                cmd9.Parameters.AddWithValue("@name", name);
                cmd9.Parameters.AddWithValue("@url", url);
                cmd9.Parameters.AddWithValue("@signDT", signDT);
                cmd9.Parameters.AddWithValue("@cancelDate", cancelDate);
                var updC = cmd9.ExecuteNonQuery();
                var idOdContract = (int)cmd9.LastInsertedId;
                AddContractEvent?.Invoke(updC);
                var attach =
                    GetElements(root, "attachmentsInfo.attachmentInfo");
                attach.AddRange(GetElements(root, "noticeAttachmentsInfo.attachmentInfo"));
                foreach (var att in attach)
                {
                    var fileName = ((string)att.SelectToken("fileName") ?? "").Trim();
                    var publishedContentId = ((string)att.SelectToken("publishedContentId") ?? "").Trim();
                    var docDescription = ((string)att.SelectToken("docDescription") ?? "").Trim();
                    var urlA = ((string)att.SelectToken("url") ?? "").Trim();
                    var insertAtt =
                        $"INSERT INTO `contractProcedureDocs44_attach`(`id_attach`, `id_contractProcedureDocs44`, `publishedContentId`, `fileName`, `url`, `description`, `attach_text`, `attach_add`) VALUES (null,@id_contractProcedureDocs44,@publishedContentId,@fileName,@url,@description,@attach_text,@attach_add)";
                    var cmd1 = new MySqlCommand(insertAtt, connect);
                    cmd1.Prepare();
                    cmd1.Parameters.AddWithValue("@id_contractProcedureDocs44", idOdContract);
                    cmd1.Parameters.AddWithValue("@publishedContentId", publishedContentId);
                    cmd1.Parameters.AddWithValue("@fileName", fileName);
                    cmd1.Parameters.AddWithValue("@url", urlA);
                    cmd1.Parameters.AddWithValue("@description", docDescription);
                    cmd1.Parameters.AddWithValue("@attach_text", "");
                    cmd1.Parameters.AddWithValue("@attach_add", 0);
                    cmd1.ExecuteNonQuery();
                }
            }
        }
    }
}