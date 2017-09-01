using System;
using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;
using ParserContracts44;
using Newtonsoft.Json.Linq;
namespace Test
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Test1()
        {
            Assert.True(true);
        }

        [Test]
        public void TestParser()
        {
            ParserContr44 p = new ParserContr44("last");
            p.ParsingXml("contract_2366200608816001262_30400522.xml", "32");
            Assert.True(true);
        }

        [Test]
        public void TestOkpd()
        {
            string okpd2GroupLevel1Code = "";
            int okpd2GroupCode = 0;
            void Test()
            {
                WorkWithContract44 w = new WorkWithContract44(new JObject(), "test.file", "32");
                w.GetOkpd("11.22.00", out okpd2GroupCode, out okpd2GroupLevel1Code);
            }

            Test();
            /*Assert.IsTrue(String.IsNullOrEmpty(okpd2_group_level1_code));
            Assert.IsFalse(String.IsNullOrEmpty(okpd2_group_level1_code));*/
            Assert.AreEqual(okpd2GroupLevel1Code, "2");
        }
    }
}