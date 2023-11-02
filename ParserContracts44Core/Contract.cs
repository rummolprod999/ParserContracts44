using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ParserContracts44
{
    public class Contract
    {
        protected readonly string File;

        public Contract(string f)
        {
            File = f;
        }

        public string GetXml(string xml)
        {
            var xmlt = xml.Split('/');
            var t = xmlt.Length;
            var txml = "";
            if (t > 1)
            {
                txml = xmlt[t - 2] + "/" + xmlt[t - 1];
            }
            return txml;
        }

        protected void AddCustomer(int d)
        {
            if (d > 0)
            {
                Program.AddCustomer++;
            }
            else
            {
                Log.Logger("Не удалось добавить customer", File);
            }
        }

        protected void AddSupplier(int d)
        {
            if (d > 0)
            {
                Program.AddSupplier++;
            }
            else
            {
                Log.Logger("Не удалось добавить supplier", File);
            }
        }

        protected void AddContract(int d)
        {
            if (d > 0)
            {
                Program.AddContract++;
            }
            else
            {
                Log.Logger("Не удалось добавить contract", File);
            }
        }

        protected virtual void AddProduct(int d)
        {
            if (d > 0)
            {
                Program.AddProduct++;
            }
            else
            {
                Log.Logger("Не удалось добавить product", File);
            }
        }

        protected void UpdateContract(int d)
        {
            if (d > 0)
            {
                Program.UpdateContract++;
            }
            else
            {
                Log.Logger("Не удалось обновить contact", File);
            }
        }

        public void GetOkpd(string okpd2Code, out int okpd2GroupCode, out string okpd2GroupLevel1Code)
        {
            if (okpd2Code.Length > 1)
            {
                var dot = okpd2Code.IndexOf(".");
                if (dot != -1)
                {
                    var okpd2GroupCodeTemp = okpd2Code.Substring(0, dot);
                    okpd2GroupCodeTemp = okpd2GroupCodeTemp.Substring(0, 2);
                    int tempOkpd2GroupCode;
                    if (!Int32.TryParse(okpd2GroupCodeTemp, out tempOkpd2GroupCode))
                    {
                        tempOkpd2GroupCode = 0;
                    }
                    okpd2GroupCode = tempOkpd2GroupCode;
                }
                else
                {
                    okpd2GroupCode = 0;
                }
            }
            else
            {
                okpd2GroupCode = 0;
            }
            if (okpd2Code.Length > 3)
            {
                var dot = okpd2Code.IndexOf(".");
                if (dot != -1)
                {
                    okpd2GroupLevel1Code = okpd2Code.Substring(dot + 1, 1);
                }
                else
                {
                    okpd2GroupLevel1Code = "";
                }
            }
            else
            {
                okpd2GroupLevel1Code = "";
            }
        }
        
        public List<JToken> GetElements(JToken j, string s)
        {
            var els = new List<JToken>();
            var els_obj = j.SelectToken(s);
            if (els_obj != null && els_obj.Type != JTokenType.Null)
            {
                switch (els_obj.Type)
                {
                    case JTokenType.Object:
                        els.Add(els_obj);
                        break;
                    case JTokenType.Array:
                        els.AddRange(els_obj);
                        break;
                }
            }

            return els;
        }
    }
}