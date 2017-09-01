using System;
using System.Xml;
using System.IO;

namespace ParserContracts44
{
    public class GetSettings
    {
        public readonly string Database;
        public readonly string TempPathContracts44;
        public readonly string LogPathContracts44;
        public readonly string TempPathContracts223;
        public readonly string LogPathContracts223;
        public readonly string Prefix;
        public readonly string UserDb;
        public readonly string PassDb;
        public readonly string Server;
        public readonly int Port;
        public readonly string Years;

        public GetSettings()
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(Program.PathProgram + Path.DirectorySeparatorChar + "setting_contracts44.xml");
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
                            TempPathContracts44 = $"{Program.PathProgram}{Path.DirectorySeparatorChar}{xnode.InnerText}";
                            break;
                        case "logdir_contracts44":
                            LogPathContracts44 = $"{Program.PathProgram}{Path.DirectorySeparatorChar}{xnode.InnerText}";
                            break;
                        case "tempdir_contracts223":
                            TempPathContracts223 = $"{Program.PathProgram}{Path.DirectorySeparatorChar}{xnode.InnerText}";
                            break;
                        case "logdir_contracts223":
                            LogPathContracts223 = $"{Program.PathProgram}{Path.DirectorySeparatorChar}{xnode.InnerText}";
                            break;
                        case "prefix":
                            Prefix = xnode.InnerText;
                            break;
                        case "userdb":
                            UserDb = xnode.InnerText;
                            break;
                        case "passdb":
                            PassDb = xnode.InnerText;
                            break;
                        case "server":
                            Server = xnode.InnerText;
                            break;
                        case "port":
                            Port = Int32.TryParse(xnode.InnerText, out Port)?Int32.Parse(xnode.InnerText): 3306;
                            break;
                        case "years":
                            Years = xnode.InnerText;
                            break;
                    }
                }
            }

            if (String.IsNullOrEmpty(LogPathContracts44) || String.IsNullOrEmpty(TempPathContracts44) || String.IsNullOrEmpty(Database) || String.IsNullOrEmpty(UserDb) || String.IsNullOrEmpty(Server) || String.IsNullOrEmpty(Years) || String.IsNullOrEmpty(TempPathContracts223) || String.IsNullOrEmpty(LogPathContracts223))
            {
                Console.WriteLine("Некоторые поля в файле настроек пустые");
                Environment.Exit(0);
            }
        }
    }
}