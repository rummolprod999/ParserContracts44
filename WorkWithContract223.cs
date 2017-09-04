using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace ParserContracts44
{
    public class WorkWithContract223 : Contract
    {
        protected readonly JObject J223;
        protected readonly string Region;
        
        public event Action<int> AddSupplierEvent;
        public event Action<int> AddCustomerEvent;
        public event Action<int> UpdateContractEvent;
        public event Action<int> AddContractEvent;
        public event Action<int> AddProductEvent;
        
        public WorkWithContract223(JObject json, string f, string r) : base(f)
        {
            J223 = json;
            Region = r;
            AddCustomerEvent += AddCustomer;
            AddSupplierEvent += AddSupplier;
            UpdateContractEvent += UpdateContract;
            AddContractEvent += AddContract;
            AddProductEvent += AddProduct;
        }

        public void Work223()
        {
            string xml = GetXml(File);
            //Console.WriteLine(J223);
            int idCustomer = 0;
            int idSupplier = 0;
            JToken c = J223.SelectToken("performanceContract.body.item.performanceContractData");
            if (!c.IsNullOrEmpty())
            {
                string idContract = ((string) c.SelectToken("guid") ?? "").Trim();
                if (String.IsNullOrEmpty(idContract))
                {
                    Log.Logger("У контракта нет id", File);
                    return;
                }
                List<JToken> products = GetElements(c, "positions.position");
                string pNumber = idContract;
                string regnum = ((string) c.SelectToken("registrationNumber") ?? "").Trim();
                //Console.WriteLine(regnum);
                string currentContractStage = "";
                string placing = "";
                string url = ((string) c.SelectToken("urlOOS") ?? "").Trim();
                DateTime signDate = (DateTime?) c.SelectToken("contractInfo.contractDate") ?? DateTime.MinValue;
                string singleCustomerReasonCode = "";
                string singleCustomerReasonName = "";
                string fz = "223";
                string notificationNumber = ((string) c.SelectToken("contractInfo.name") ?? "").Trim();
                int lotNumber = 1;
                decimal contractPrice = (decimal?) c.SelectToken("positions.total") ?? 0.00m;
                string currency = "";
                if(products.Count>0)
                    currency = ((string) products[0].SelectToken("currency.code") ?? "").Trim();
                int versionNumber = (int?) c.SelectToken("version") ?? 0;
                int cancel = 0;
                DateTime executionStartDate = signDate;
                DateTime executionEndDate = (DateTime?) c.SelectToken("createDateTime") ?? DateTime.MinValue;
                using (Contract223Context db = new Contract223Context())
                {
                    if (!String.IsNullOrEmpty(regnum) && versionNumber != 0)
                    {
                        var maxNumber = db.Contracts223.Where(p => p.RegNum == regnum).Where(p => p != null).Max(d => d.VersionNumber);
                        Console.WriteLine(maxNumber);
                    }
                }

            }
            else
            {
                Log.Logger("Не могу найти тег performanceContractData", File);
            }
        }

    }
}