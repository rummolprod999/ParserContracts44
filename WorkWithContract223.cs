﻿using System;
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
                List<JToken> products2 = GetElements(c, "docs.doc");
                string pNumber = idContract;
                string regnum = ((string) c.SelectToken("registrationNumber") ?? "").Trim();
                //Console.WriteLine(regnum);
                string signNumber = ((string) c.SelectToken("contractRegNumber") ?? "").Trim();
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
                if (contractPrice == 0.00m)
                {
                    contractPrice = (decimal?) c.SelectToken("docs.total") ?? 0.00m;
                }
                string currency = "";
                if (products.Count > 0)
                    currency = ((string) products[0].SelectToken("currency.code") ?? "").Trim();
                else if (products2.Count > 0)
                    currency = ((string) products2[0].SelectToken("currency.code") ?? "").Trim();
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
                            contr.CustomerId = idCustomer;
                        }
                        else
                        {
                            contr.Customer = cus;
                        }

                        contr.SupplierId = idSupplier;
                        contr.Xml = xml;
                        contr.SignNumber = signNumber;
                        db.Entry(contr).State = EntityState.Modified;
                        db.SaveChanges();
                        UpdateContractEvent?.Invoke(1);
                        foreach (var p in contr.Products.ToList())
                        {
                            db.Products.Remove(p);
                            db.Entry(p).State = EntityState.Deleted;
                        }
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
                            Xml = xml,
                            SignNumber = signNumber
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
                        AddContractEvent?.Invoke(1);
                    }
                    if (products.Count > 0)
                    {
                        List<Product> pr = new List<Product>();
                        foreach (var pr223 in products)
                        {
                            var docs = GetElements(pr223, "docs.doc");
                            if (docs.Count == 0)
                                continue;
                            int okpd2GroupCode = 0;
                            string okpd2GroupLevel1Code = "";
                            int okpdGroupCode = 0;
                            string okpdGroupLevel1Code = "";
                            string nameP = ((string) docs[0].SelectToken("contractPosition.name") ?? "").Trim();
                            //Console.WriteLine(nameP);
                            nameP = Regex.Replace(nameP, @"\s+", " ");
                            if (String.IsNullOrEmpty(nameP))
                                nameP = "Нет названия";
                            string okpd2Code =
                                ((string) docs[0].SelectToken("contractPosition.okpd2.code") ?? "").Trim();
                            if (!String.IsNullOrEmpty(okpd2Code))
                            {
                                GetOkpd(okpd2Code, out okpd2GroupCode, out okpd2GroupLevel1Code);
                            }
                            string okpd2Name =
                                ((string) docs[0].SelectToken("contractPosition.okpd2.name") ?? "").Trim();
                            string okpdCode = ((string) docs[0].SelectToken("contractPosition.okpd.code") ?? "").Trim();
                            if (!String.IsNullOrEmpty(okpdCode))
                            {
                                GetOkpd(okpdCode, out okpdGroupCode, out okpdGroupLevel1Code);
                            }
                            string okpdName = ((string) docs[0].SelectToken("contractPosition.okpd.name") ?? "").Trim();
                            decimal sumP = (decimal?) pr223.SelectToken("price") ?? 0.00m;
                            decimal quantity = (decimal?) docs[0].SelectToken("qty") ?? 0.00m;
                            decimal price = 0.00m;
                            if (quantity != 0.00m)
                            {
                                price = sumP / quantity;
                            }
                            string sid = "";
                            string okei = ((string) docs[0].SelectToken("okei.name") ?? "").Trim();
                            Product p223 = new Product
                            {
                                Name = nameP,
                                Okpd2Code = okpd2Code,
                                OkpdCode = okpdCode,
                                Okpd2GroupCode = okpd2GroupCode,
                                OkpdGroupCode = okpdGroupCode,
                                Okpd2GroupLevel1Code = okpd2GroupLevel1Code,
                                OkpdGroupLevel1Code = okpdGroupLevel1Code,
                                Price = price,
                                Okpd2Name = okpd2Name,
                                OkpdName = okpdName,
                                Quantiity = quantity,
                                Okei = okei,
                                Sum = sumP,
                                Sid = sid,
                                Contract223 = contr
                            };
                            pr.Add(p223);
                            AddProductEvent?.Invoke(1);
                        }
                        db.Products.AddRange(pr);
                        db.SaveChanges();
                    }
                    else if (products2.Count > 0)
                    {
                        List<Product> pr = new List<Product>();
                        foreach (var pr223 in products2)
                        {
                            int okpd2GroupCode = 0;
                            string okpd2GroupLevel1Code = "";
                            int okpdGroupCode = 0;
                            string okpdGroupLevel1Code = "";
                            string nameP = ((string) pr223.SelectToken("contractPosition.name") ?? "").Trim();
                            //Console.WriteLine(nameP);
                            nameP = Regex.Replace(nameP, @"\s+", " ");
                            if (String.IsNullOrEmpty(nameP))
                                nameP = "Нет названия";
                            string okpd2Code =
                                ((string) pr223.SelectToken("contractPosition.okpd2.code") ?? "").Trim();
                            if (!String.IsNullOrEmpty(okpd2Code))
                            {
                                GetOkpd(okpd2Code, out okpd2GroupCode, out okpd2GroupLevel1Code);
                            }
                            string okpd2Name =
                                ((string) pr223.SelectToken("contractPosition.okpd2.name") ?? "").Trim();
                            string okpdCode = ((string) pr223.SelectToken("contractPosition.okpd.code") ?? "").Trim();
                            if (!String.IsNullOrEmpty(okpdCode))
                            {
                                GetOkpd(okpdCode, out okpdGroupCode, out okpdGroupLevel1Code);
                            }
                            string okpdName = ((string) pr223.SelectToken("contractPosition.okpd.name") ?? "").Trim();
                            decimal sumP = (decimal?) pr223.SelectToken("price") ?? 0.00m;
                            decimal quantity = (decimal?) pr223.SelectToken("qty") ?? 0.00m;
                            decimal price = 0.00m;
                            if (quantity != 0.00m)
                            {
                                price = sumP / quantity;
                            }
                            string sid = "";
                            string okei = ((string) pr223.SelectToken("okei.name") ?? "").Trim();
                            Product p223 = new Product
                            {
                                Name = nameP,
                                Okpd2Code = okpd2Code,
                                OkpdCode = okpdCode,
                                Okpd2GroupCode = okpd2GroupCode,
                                OkpdGroupCode = okpdGroupCode,
                                Okpd2GroupLevel1Code = okpd2GroupLevel1Code,
                                OkpdGroupLevel1Code = okpdGroupLevel1Code,
                                Price = price,
                                Okpd2Name = okpd2Name,
                                OkpdName = okpdName,
                                Quantiity = quantity,
                                Okei = okei,
                                Sum = sumP,
                                Sid = sid,
                                Contract223 = contr
                            };
                            pr.Add(p223);
                            AddProductEvent?.Invoke(1);
                        }
                        db.Products.AddRange(pr);
                        db.SaveChanges();
                    }
                }
            }
            else
            {
                //Log.Logger("Не могу найти тег performanceContractData", File);
            }
        }
    }
}