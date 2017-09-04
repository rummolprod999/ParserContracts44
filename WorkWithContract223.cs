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
                string pNumber = idContract;
                string regnum = ((string) c.SelectToken("registrationNumber") ?? "").Trim();

            }
            else
            {
                Log.Logger("Не могу найти тег performanceContractData", File);
            }
        }

    }
}