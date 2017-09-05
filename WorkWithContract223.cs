using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                string Url = ((string) c.SelectToken("urlOOS") ?? "").Trim();
                DateTime signDate = (DateTime?) c.SelectToken("contractInfo.contractDate") ?? DateTime.MinValue;
                string singleCustomerReasonCode = "";
                string singleCustomerReasonName = "";
                string fz = "223";
                string notificationNumber = ((string) c.SelectToken("contractInfo.name") ?? "").Trim();
                int lotNumber = 1;
                decimal contractPrice = (decimal?) c.SelectToken("positions.total") ?? 0.00m;
                string currency = "";
                if (products.Count > 0)
                    currency = ((string) products[0].SelectToken("currency.code") ?? "").Trim();
                int versionNumber = (int?) c.SelectToken("version") ?? 0;
                int cancel = 0;
                DateTime executionStartDate = signDate;
                DateTime executionEndDate = (DateTime?) c.SelectToken("createDateTime") ?? DateTime.MinValue;
                using (Contract223Context db = new Contract223Context())
                {
                    if (!String.IsNullOrEmpty(regnum) && versionNumber != 0)
                    {
                        var maxNumber = db.Contracts223.Where(p => p.RegNum == regnum).Select(p => p.VersionNumber)
                            .DefaultIfEmpty(0).Max();
                        //Console.WriteLine(maxNumber);
                        if (versionNumber > maxNumber)
                        {
                            var Contr223 = db.Contracts223.Where(p => p.RegNum == regnum);
                            foreach (var c223 in Contr223)
                            {
                                c223.Cancel = 1;
                                db.Entry(c223).State = EntityState.Modified;
                            }
                            db.SaveChanges();
                        }
                        else
                        {
                            cancel = 1;
                        }
                    }
                    Customer cus = null;
                    string kppCustomer = ((string) c.SelectToken("customer.mainInfo.kpp") ?? "").Trim();
                    string innCustomer = ((string) c.SelectToken("customer.mainInfo.inn") ?? "").Trim();
                    //Console.WriteLine(innCustomer);
                    if (!String.IsNullOrEmpty(innCustomer))
                    {
                        cus = db.Customers.FirstOrDefault(p => p.Inn == innCustomer && p.Kpp == kppCustomer);
                    }
                    //Console.WriteLine(cus);
                    var contr = db.Contracts223.Include(p => p.Products).FirstOrDefault(
                        p => p.IdContract == idContract && p.RegionCode == Region);
                    if (contr != null)
                    {
                        contr.IdContract = idContract;
                        contr.PNumber = pNumber;
                        contr.RegNum = regnum;
                        contr.CurrentContractStage = currentContractStage;
                        contr.Placing = placing;
                        contr.RegionCode = Region;
                        contr.Url = Url;
                        contr.SignDate = signDate;
                        contr.SingleCustomerReasonCode = singleCustomerReasonCode;
                        contr.SingleCustomerReasonName = singleCustomerReasonName;
                        contr.Fz = fz;
                        contr.NotificationNumber = notificationNumber;
                        contr.LotNumber = lotNumber;
                        contr.ContractPrice = contractPrice;
                        contr.Currency = currency;
                        contr.VersionNumber = versionNumber;
                        contr.ExecutionStartDate = executionStartDate;
                        contr.ExecutionEndDate = executionEndDate;
                        if (cus == null)
                        {
                            contr.CustomerId = 0;
                        }
                        else
                        {
                            contr.Customer = cus;
                        }

                        contr.SupplierId = idSupplier;
                        contr.Xml = xml;
                        foreach (var p in contr.Products)
                        {
                            db.Products.Remove(p);
                            db.Entry(p).State = EntityState.Deleted;
                        }
                        db.Entry(contr).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        contr = new Contract223
                        {
                            IdContract = idContract,
                            PNumber = pNumber,
                            RegNum = regnum,
                            CurrentContractStage = currentContractStage,
                            Placing = placing,
                            RegionCode = Region,
                            Url = Url,
                            SignDate = signDate,
                            SingleCustomerReasonCode = singleCustomerReasonCode,
                            SingleCustomerReasonName = singleCustomerReasonName,
                            Fz = fz,
                            NotificationNumber = notificationNumber,
                            LotNumber = lotNumber,
                            ContractPrice = contractPrice,
                            Currency = currency,
                            VersionNumber = versionNumber,
                            ExecutionStartDate = executionStartDate,
                            ExecutionEndDate = executionEndDate,
                            SupplierId = idSupplier,
                            Xml = xml
                        };
                        if (cus == null)
                        {
                            contr.CustomerId = 0;
                        }
                        else
                        {
                            contr.Customer = cus;
                        }
                        db.Contracts223.Add(contr);
                        db.SaveChanges();
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