using System;
using Newtonsoft.Json.Linq;

namespace ParserContracts44
{
    public class Contract615: Contract
    {
        public delegate void AddData(int d);

        protected readonly JObject J615;
        protected readonly string Region;

        public Contract615(JObject json, string f, string r) : base(f)
        {
            J615 = json;
            Region = r;
            UpdateContractEvent += UpdateContract;
            AddContractEvent += AddContract;
        }

        public event AddData UpdateContractEvent;
        public event AddData AddContractEvent;

        public void Work615()
        {
            var xml = GetXml(File);
            var root = ((JObject)J615.SelectToken("export")).Last.First;
            Console.WriteLine(root);
        }
    }
}