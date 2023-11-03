using System;
using System.Runtime.InteropServices;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ParserContracts44
{
    public class Eacts44 : Contract
    {
        public delegate void AddData(int d);

        protected readonly JObject J44;
        protected readonly string Region;

        public Eacts44(JObject json, string f, string r) : base(f)
        {
            J44 = json;
            Region = r;
            UpdateEacts44Event += UpdateContract;
            AddEacts44Event += AddContract;
        }

        public event AddData UpdateEacts44Event;
        public event AddData AddEacts44Event;

        public void Work44()
        {
            var xml = GetXmlNew(File);
            var root = ((JObject)J44.SelectToken("ФайлПакет"));
            var IdTrPaket = ((string)root.SelectToken("@ИдТрПакет") ?? "").Trim();
            if (String.IsNullOrEmpty(IdTrPaket))
            {
                Log.Logger("У контракта нет IdTrPaket", File);
                return;
            }

            var type = "";
            if (xml.Contains("ON_NSCHFDOPPR"))
            {
                type = "ON_NSCHFDOPPR";
            }
            else if (xml.Contains("ON_NSCHFDOPPOK"))
            {
                type = "ON_NSCHFDOPPOK";
            }
            else if (xml.Contains("ON_KORSCHFDOPPR"))
            {
                type = "ON_KORSCHFDOPPR";
            }
            else if (xml.Contains("ON_KORSCHFDOPPOK"))
            {
                type = "ON_KORSCHFDOPPOK";
            }
            else if (xml.Contains("ON_NKORSCHFDOPPR"))
            {
                type = "ON_NKORSCHFDOPPR";
            }
            else if (xml.Contains("ON_NKORSCHFDOPPOK"))
            {
                type = "ON_NKORSCHFDOPPOK";
            }
            else if (xml.Contains("DP_IZVUCH"))
            {
                type = "DP_IZVUCH";
            }
            else if (xml.Contains("DP_UVUTOCH"))
            {
                type = "DP_UVUTOCH";
            }
            else if (xml.Contains("DP_UVOBZH"))
            {
                type = "DP_UVOBZH";
            }
            else if (xml.Contains("DP_PROTZ"))
            {
                type = "DP_PROTZ";
            }
            else if (xml.Contains("DP_PDOTPR"))
            {
                type = "DP_PDOTPR";
            }
            else if (xml.Contains("DP_PDPOL"))
            {
                type = "DP_PDPOL";
            }
            else if (xml.Contains("DP_IZVPOL"))
            {
                type = "DP_IZVPOL";
            }
            else if (xml.Contains("DP_KVITIZMSTATUS"))
            {
                type = "DP_KVITIZMSTATUS";
            }
            else if (xml.Contains("elActUnstructuredSupplierTitle"))
            {
                type = "elActUnstructuredSupplierTitle";
            }
            else if (xml.Contains("elActUnstructuredCustomerTitle"))
            {
                type = "elActUnstructuredCustomerTitle";
            }
            else if (xml.Contains("ON_AKTREZRABP"))
            {
                type = "ON_AKTREZRABP";
            }
            else if (xml.Contains("ON_AKTREZRABZ"))
            {
                type = "ON_AKTREZRABZ";
            }

            var SistOtpr = ((string)root.SelectToken("@СистОтпр") ?? "").Trim();
            var SystPol = ((string)root.SelectToken("@СистПол") ?? "").Trim();
            var IdObyekt = ((string)root.SelectToken("@ИдОбъект") ?? "").Trim();
            var VneshId = ((string)root.SelectToken("@ВнешИд") ?? "").Trim();
            var IdFayl = ((string)root.SelectToken("@ИдФайл") ?? "").Trim();
            var IdPrilozh = ((string)root.SelectToken("@ИдПрилож") ?? "").Trim();
            var ReestrNomKont = ((string)root.SelectToken("@РеестрНомКонт") ?? "").Trim();
            if (ReestrNomKont == "")
            {
                var tmpR = xml.Replace(type + "_", "");
                var q = tmpR.Split('_');
                ReestrNomKont = q[0];
            }
            var DataVrFormir = (JsonConvert.SerializeObject(root.SelectToken("@ДатаВрФормир") ?? "") ??
                                "").Trim('"');
            var TipPrilozh = ((string)root.SelectToken("@ТипПрилож") ?? "").Trim();
            var VersForm = ((string)root.SelectToken("@ВерсФорм") ?? "").Trim();
            var IdOtpr = ((string)root.SelectToken("@ИдОтпр") ?? "").Trim();
            var IdPol = ((string)root.SelectToken("@ИдПол") ?? "").Trim();
            var KontentDokument = ((string)root.SelectToken("Документ.Контент") ?? "").Trim();
            var KontentPrilozh = ((string)root.SelectToken("Прилож.Контент") ?? "").Trim();
            var Ssylka = ((string)root.SelectToken("ПечатнФорм.Ссылка") ?? "").Trim();
            var TipObyekt = ((string)root.SelectToken("..ТипОбъект") ?? "").Trim();
            var TipFKh = ((string)root.SelectToken("..ТипФХ") ?? "").Trim();
            using (var connect = ConnectToDb.GetDbConnection())
            {
                connect.Open();
                if (!String.IsNullOrEmpty(IdTrPaket))
                {
                    var selectTender =
                        "SELECT id FROM eacts44 WHERE  IdTrPaket = @IdTrPaket";
                    var cmd = new MySqlCommand(selectTender, connect);
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@IdTrPaket", IdTrPaket);
                    var reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Close();
                        return;
                    }

                    reader.Close();
                }

                var updateContract =
                    $"INSERT INTO eacts44 (id, type, IdTrPaket, SistOtpr, SystPol, IdObyekt, VneshId, IdFayl, IdPrilozh, ReestrNomKont, DataVrFormir, TipPrilozh, VersForm, IdOtpr, IdPol, KontentDokument, KontentPrilozh, Ssylka, TipObyekt, TipFKh, xml) VALUES (null,@type,@idtrpaket,@sistotpr,@systpol,@idobyekt,@vneshid,@idfayl,@idprilozh,@reestrnomkont,@datavrformir,@tipprilozh,@versform,@idotpr,@idpol,@kontentdokument,@kontentprilozh,@ssylka,@tipobyekt,@tipfkh, @xml)";
                var cmd9 = new MySqlCommand(updateContract, connect);
                cmd9.Prepare();
                cmd9.Parameters.AddWithValue("@type", type);
                cmd9.Parameters.AddWithValue("@idtrpaket", IdTrPaket);
                cmd9.Parameters.AddWithValue("@sistotpr", SistOtpr);
                cmd9.Parameters.AddWithValue("@systpol", SystPol);
                cmd9.Parameters.AddWithValue("@idobyekt", IdObyekt);
                cmd9.Parameters.AddWithValue("@vneshid", VneshId);
                cmd9.Parameters.AddWithValue("@idfayl", IdFayl);
                cmd9.Parameters.AddWithValue("@idprilozh", IdPrilozh);
                cmd9.Parameters.AddWithValue("@reestrnomkont", ReestrNomKont);
                cmd9.Parameters.AddWithValue("@datavrformir", DataVrFormir);
                cmd9.Parameters.AddWithValue("@tipprilozh", TipPrilozh);
                cmd9.Parameters.AddWithValue("@versform", VersForm);
                cmd9.Parameters.AddWithValue("@idotpr", IdOtpr);
                cmd9.Parameters.AddWithValue("@idpol", IdPol);
                cmd9.Parameters.AddWithValue("@kontentdokument", KontentDokument);
                cmd9.Parameters.AddWithValue("@kontentprilozh", KontentPrilozh);
                cmd9.Parameters.AddWithValue("@ssylka", Ssylka);
                cmd9.Parameters.AddWithValue("@tipobyekt", TipObyekt);
                cmd9.Parameters.AddWithValue("@tipfkh", TipFKh);
                cmd9.Parameters.AddWithValue("@xml", xml);
                var updC = cmd9.ExecuteNonQuery();
                var idOdContract = (int)cmd9.LastInsertedId;
                AddEacts44Event?.Invoke(updC);
                var attach =
                    GetElements(root, "Вложен");
                foreach (var att in attach)
                {
                    var fileName = ((string)att.SelectToken("@ИмяФайл") ?? "").Trim();
                    var publishedContentId = ((string)att.SelectToken("@КонтентИд") ?? "").Trim();
                    var docDescription = ((string)att.SelectToken("docDescription") ?? "").Trim();
                    var fileSize = ((string)att.SelectToken("@РазмерФайл") ?? "").Trim();
                    var urlA = ((string)att.SelectToken("@Ссылка") ?? "").Trim();
                    var insertAtt =
                        $"INSERT INTO `eacts44_attach`(`id_attach`, `id_eacts44`, `publishedContentId`, `fileName`, `url`, `description`, `attach_text`, `attach_add`, `fileSize`) VALUES (null,@id_eacts44,@publishedContentId,@fileName,@url,@description,@attach_text,@attach_add,@fileSize)";
                    var cmd1 = new MySqlCommand(insertAtt, connect);
                    cmd1.Prepare();
                    cmd1.Parameters.AddWithValue("@id_eacts44", idOdContract);
                    cmd1.Parameters.AddWithValue("@publishedContentId", publishedContentId);
                    cmd1.Parameters.AddWithValue("@fileName", fileName);
                    cmd1.Parameters.AddWithValue("@url", urlA);
                    cmd1.Parameters.AddWithValue("@description", docDescription);
                    cmd1.Parameters.AddWithValue("@attach_text", "");
                    cmd1.Parameters.AddWithValue("@attach_add", 0);
                    cmd1.Parameters.AddWithValue("@fileSize", fileSize);
                    cmd1.ExecuteNonQuery();
                }

                var dataUndersigned =
                    GetElements(root, "Прилож");
                foreach (var d in dataUndersigned)
                {
                    var OblPoln = ((string)d.SelectToken("ПодписьПрилож.@ОблПолн") ?? "").Trim();
                    var Status = ((string)d.SelectToken("ПодписьПрилож.@Статус") ?? "").Trim();
                    var OsnPoln = ((string)d.SelectToken("ПодписьПрилож.@ОснПолн") ?? "").Trim();
                    var OsnPolnOrg = ((string)d.SelectToken("ПодписьПрилож.@ОснПолнОрг") ?? "").Trim();
                    var DataPodpisan =
                        (string)d.SelectToken("ПодписьПрилож.@ДатаПодписан") ?? "";
                    if (DataPodpisan != "")
                    {
                        DataPodpisan = DateTime.ParseExact(DataPodpisan, "dd.MM.yyyy",
                            System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    }
                    var VremPodpisan =
                        (string)d.SelectToken("ПодписьПрилож.@ВремПодписан") ?? "";
                    if (VremPodpisan != "")
                    {
                        VremPodpisan = VremPodpisan.Replace(".", ":");
                    }
                    var INNFL = ((string)d.SelectToken("ПодписьПрилож.ИП.@ИННФЛ") ?? "").Trim();
                    var Familiya = ((string)d.SelectToken("ПодписьПрилож.ЮЛ.ФИО.Фамилия") ?? "").Trim();
                    if (Familiya == "")
                    {
                        Familiya = ((string)d.SelectToken("ПодписьПрилож.ИП.ФИО.Фамилия") ?? "").Trim();
                    }

                    var Imya = ((string)d.SelectToken("ПодписьПрилож.ЮЛ.ФИО.Имя") ?? "").Trim();
                    if (Imya == "")
                    {
                        Imya = ((string)d.SelectToken("ПодписьПрилож.ИП.ФИО.Имя") ?? "").Trim();
                    }

                    var Otchestvo = ((string)d.SelectToken("ПодписьПрилож.ЮЛ.ФИО.Отчество") ?? "").Trim();
                    if (Otchestvo == "")
                    {
                        Otchestvo = ((string)d.SelectToken("ПодписьПрилож.ИП.ФИО.Отчество") ?? "").Trim();
                    }

                    var SvGosRegIP = ((string)d.SelectToken("ПодписьПрилож.ИП.@СвГосРегИП") ?? "").Trim();
                    var InyeSved = ((string)d.SelectToken("ПодписьПрилож.ИП.@ИныеСвед") ?? "").Trim();
                    if (InyeSved == "")
                    {
                        InyeSved = ((string)d.SelectToken("ПодписьПрилож.ЮЛ.@ИныеСвед") ?? "").Trim();
                    }

                    var INNYuL = ((string)d.SelectToken("ПодписьПрилож.ЮЛ.@ИННЮЛ") ?? "").Trim();
                    var NaimOrg = ((string)d.SelectToken("ПодписьПрилож.ЮЛ.@НаимОрг") ?? "").Trim();
                    var Dolzhn = ((string)d.SelectToken("ПодписьПрилож.ЮЛ.@Должн") ?? "").Trim();
                    var NomDover = ((string)d.SelectToken("ПодписьПрилож.ЮЛ.@НомДовер") ?? "").Trim();
                    if (NomDover == "")
                    {
                        NomDover = ((string)d.SelectToken("ПодписьПрилож.ИП.@НомДовер") ?? "").Trim();
                    }

                    var VnNomDover = ((string)d.SelectToken("ПодписьПрилож.ЮЛ.@ВнНомДовер") ?? "").Trim();
                    if (VnNomDover == "")
                    {
                        VnNomDover = ((string)d.SelectToken("ПодписьПрилож.ИП.@ВнНомДовер") ?? "").Trim();
                    }

                    var DataVydDover =
                        (JsonConvert.SerializeObject(d.SelectToken("ПодписьПрилож.ИП.@ДатаВыдДовер") ?? "") ??
                         "").Trim('"');
                    if (DataVydDover == "")
                    {
                        DataVydDover =
                            (JsonConvert.SerializeObject(d.SelectToken("ПодписьПрилож.ЮЛ.@ДатаВыдДовер") ?? "") ??
                             "").Trim('"');
                    }
                    if (DataVydDover != "")
                    {
                        DataVydDover = DateTime.ParseExact(DataVydDover, "dd.MM.yyyy",
                            System.Globalization.CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                    }
                    var insertProd =
                        $"INSERT INTO eacts44_dataUndersigned (id, id_eacts44, OblPoln, Status, OsnPoln, OsnPolnOrg, DataPodpisan, VremPodpisan, INNFL, Familiya, Imya, Otchestvo, SvGosRegIP, InyeSved, INNYuL, NaimOrg, Dolzhn, NomDover, VnNomDover, DataVydDover) VALUES (null,@idEacts44,@oblpoln,@status,@osnpoln,@osnpolnorg,@datapodpisan,@vrempodpisan,@innfl,@familiya,@imya,@otchestvo,@svgosregip,@inyesved,@innyul,@naimorg,@dolzhn,@nomdover,@vnnomdover,@datavyddover)";
                    var cmd11 = new MySqlCommand(insertProd, connect);
                    cmd11.Prepare();
                    cmd11.Parameters.AddWithValue("@idEacts44", idOdContract);
                    cmd11.Parameters.AddWithValue("@oblpoln", OblPoln);
                    cmd11.Parameters.AddWithValue("@status", Status);
                    cmd11.Parameters.AddWithValue("@osnpoln", OsnPoln);
                    cmd11.Parameters.AddWithValue("@osnpolnorg", OsnPolnOrg);
                    cmd11.Parameters.AddWithValue("@datapodpisan", DataPodpisan);
                    cmd11.Parameters.AddWithValue("@vrempodpisan", VremPodpisan);
                    cmd11.Parameters.AddWithValue("@innfl", INNFL);
                    cmd11.Parameters.AddWithValue("@familiya", Familiya);
                    cmd11.Parameters.AddWithValue("@imya", Imya);
                    cmd11.Parameters.AddWithValue("@otchestvo", Otchestvo);
                    cmd11.Parameters.AddWithValue("@svgosregip", SvGosRegIP);
                    cmd11.Parameters.AddWithValue("@inyesved", InyeSved);
                    cmd11.Parameters.AddWithValue("@innyul", INNYuL);
                    cmd11.Parameters.AddWithValue("@naimorg", NaimOrg);
                    cmd11.Parameters.AddWithValue("@dolzhn", Dolzhn);
                    cmd11.Parameters.AddWithValue("@nomdover", NomDover);
                    cmd11.Parameters.AddWithValue("@vnnomdover", VnNomDover);
                    cmd11.Parameters.AddWithValue("@datavyddover", DataVydDover);
                    var addP = cmd11.ExecuteNonQuery();
                }
            }
        }
    }
}