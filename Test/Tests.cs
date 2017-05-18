using System;
using NUnit.Framework;
using ParserContracts44;

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
            p.ParsingXML("contract_2366200608816001262_30400522.xml", "32");
            Assert.True(true);
        }
    }
}