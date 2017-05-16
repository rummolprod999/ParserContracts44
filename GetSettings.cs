using System;
using System.Xml;

namespace ParserContracts44
{
    public class GetSettings
    {
        public readonly string Database;
        public readonly string TempdirContracts44;
        public readonly string LogdirContracts44;
        public readonly string Prefix;
        public readonly string UserDB;
        public readonly string PassDB;

        public GetSettings()
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(Environment.CurrentDirectory + "/setting_contracts44.xml");
            /*Console.WriteLine(Environment.CurrentDirectory+ "/setting_contracts44.xml");*/
            XmlElement xRoot = xDoc.DocumentElement;
            if (xRoot != null)
            {
                foreach (XmlNode xnode in xRoot)
                {
                    switch (xnode.Name)
                    {
                        case "database":
                            Database = xnode.InnerText;
                            break;
                        case "tempdir_contracts44":
                            TempdirContracts44 = $"{Environment.CurrentDirectory}/{xnode.InnerText}";
                            break;
                        case "logdir_contracts44":
                            LogdirContracts44 = $"{Environment.CurrentDirectory}/{xnode.InnerText}";
                            break;
                        case "prefix":
                            Prefix = xnode.InnerText;
                            break;
                        case "userdb":
                            UserDB = xnode.InnerText;
                            break;
                        case "passdb":
                            PassDB = xnode.InnerText;
                            break;
                    }
                }
            }
        }
    }
}