using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ParserContracts44
{
    internal class Program
    {
        private static string _database;
        private static string _tempPath44;
        private static string _logPath44;
        private static string _tempPathCPD44;
        private static string _logPathCPD44;
        private static string _tempPathC615;
        private static string _logPathC615;
        private static string _tempPathEacts44;
        private static string _logPathEacts44;
        private static string _tempPath223;
        private static string _logPath223;
        private static string _prefix;
        private static string _user;
        private static string _pass;
        private static string _server;
        private static int _port;
        private static List<string> _years = new List<string>();
        public static string Database => _database;

        public static string TempPath
        {
            get
            {
                switch (Periodparsing)
                {
                    case TypeArguments.Curr44:
                    case TypeArguments.Prev44:
                    case TypeArguments.Last44:
                        return _tempPath44;
                    case TypeArguments.Last223:
                    case TypeArguments.Daily223:
                        return _tempPath223;
                    case TypeArguments.CurrCPD44:
                    case TypeArguments.PrevCPD44:
                    case TypeArguments.LastCPD44:
                        return _tempPathCPD44;
                    case TypeArguments.CurrEacts44:
                    case TypeArguments.PrevEacts44:
                    case TypeArguments.LastEacts44:
                        return _tempPathEacts44;
                    case TypeArguments.CurrC615:
                    case TypeArguments.PrevC615:
                    case TypeArguments.LastC615:
                        return _tempPathC615;
                    default:
                        return "";
                }
            }
        }

        public static string LogPath
        {
            get
            {
                switch (Periodparsing)
                {
                    case TypeArguments.Curr44:
                    case TypeArguments.Prev44:
                    case TypeArguments.Last44:
                        return _logPath44;
                    case TypeArguments.Last223:
                    case TypeArguments.Daily223:
                        return _logPath223;
                    case TypeArguments.CurrCPD44:
                    case TypeArguments.PrevCPD44:
                    case TypeArguments.LastCPD44:
                        return _logPathCPD44;
                    case TypeArguments.CurrEacts44:
                    case TypeArguments.PrevEacts44:
                    case TypeArguments.LastEacts44:
                        return _logPathEacts44;
                    case TypeArguments.CurrC615:
                    case TypeArguments.PrevC615:
                    case TypeArguments.LastC615:
                        return _logPathC615;
                    default:
                        return "";
                }
            }
        }

        public static string Prefix => _prefix;
        public static string User => _user;
        public static string Pass => _pass;
        public static string Server => _server;
        public static int Port => _port;
        public static List<string> Years => _years;
        public static readonly DateTime LocalDate = DateTime.Now;
        public static string FileLog;
        public static TypeArguments Periodparsing;
        public static string StrArg;
        public static int AddCustomer = 0;
        public static int AddSupplier = 0;
        public static int AddContract = 0;
        public static int AddProduct = 0;
        public static int UpdateContract = 0;
        public static string PathProgram;
        public static string TableContracts;
        public static string TableProducts;
        public static string TableArchive223;

        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(
                    "Недостаточно аргументов для запуска, используйте last44 или prev44 или curr44, last223, daily223, currcpd44, prevcpd44, lastcpd44,curreacts44, preveacts44, lasteacts44, lastc615, prevc615, currc615 в качестве аргумента");
                return;
            }

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName()
                .CodeBase);
            PathProgram = path.Substring(5);
            StrArg = args[0];
            switch (args[0])
            {
                case "last44":
                    Periodparsing = TypeArguments.Last44;
                    Init(Periodparsing);
                    ParserC44(Periodparsing);
                    break;
                case "prev44":
                    Periodparsing = TypeArguments.Prev44;
                    Init(Periodparsing);
                    ParserC44(Periodparsing);
                    break;
                case "curr44":
                    Periodparsing = TypeArguments.Curr44;
                    Init(Periodparsing);
                    ParserC44(Periodparsing);
                    break;
                case "lastcpd44":
                    Periodparsing = TypeArguments.LastCPD44;
                    Init(Periodparsing);
                    ParserCPD44(Periodparsing);
                    break;
                case "prevcpd44":
                    Periodparsing = TypeArguments.PrevCPD44;
                    Init(Periodparsing);
                    ParserCPD44(Periodparsing);
                    break;
                case "currcpd44":
                    Periodparsing = TypeArguments.CurrCPD44;
                    Init(Periodparsing);
                    ParserCPD44(Periodparsing);
                    break;
                case "lasteacts44":
                    Periodparsing = TypeArguments.LastEacts44;
                    Init(Periodparsing);
                    ParserEacts44(Periodparsing);
                    break;
                case "preveacts44":
                    Periodparsing = TypeArguments.PrevEacts44;
                    Init(Periodparsing);
                    ParserEacts44(Periodparsing);
                    break;
                case "curreacts44":
                    Periodparsing = TypeArguments.CurrEacts44;
                    Init(Periodparsing);
                    ParserEacts44(Periodparsing);
                    break;
                case "daily223":
                    Periodparsing = TypeArguments.Daily223;
                    Init(Periodparsing);
                    ParserC223(Periodparsing);
                    break;
                case "last223":
                    Periodparsing = TypeArguments.Last223;
                    Init(Periodparsing);
                    ParserC223(Periodparsing);
                    break;
                case "lastc615":
                    Periodparsing = TypeArguments.LastC615;
                    Init(Periodparsing);
                    ParserC615(Periodparsing);
                    break;
                case "prevc615":
                    Periodparsing = TypeArguments.PrevC615;
                    Init(Periodparsing);
                    ParserC615(Periodparsing);
                    break;
                case "currc615":
                    Periodparsing = TypeArguments.CurrC615;
                    Init(Periodparsing);
                    ParserC615(Periodparsing);
                    break;
            }
        }

        private static void Init(TypeArguments arg)
        {
            var set = new GetSettings();
            _database = set.Database;
            _logPath44 = set.LogPathContracts44;
            _logPathCPD44 = set.LogPathCPDs44;
            _logPathC615 = set.LogPathC615;
            _logPathEacts44 = set.LogPathEacts44;
            _logPath223 = set.LogPathContracts223;
            _prefix = set.Prefix;
            _user = set.UserDb;
            _pass = set.PassDb;
            _tempPath44 = set.TempPathContracts44;
            _tempPath223 = set.TempPathContracts223;
            _tempPathCPD44 = set.TempPathCPD44;
            _tempPathC615 = set.TempPathC615;
            _tempPathEacts44 = set.TempPathEacts44;
            _server = set.Server;
            _port = set.Port;
            var tmp = set.Years;
            TableContracts = $"{Prefix}od_contract";
            TableProducts = $"{Prefix}od_contract_product";
            TableArchive223 = $"{Prefix}arhiv_contract223";
            var tempYears = tmp.Split(new char[] {','});

            foreach (var s in tempYears.Select(v => $"{v.Trim()}"))
            {
                _years.Add(s);
            }

            if (Directory.Exists(TempPath))
            {
                var dirInfo = new DirectoryInfo(TempPath);
                dirInfo.Delete(true);
                Directory.CreateDirectory(TempPath);
            }
            else
            {
                Directory.CreateDirectory(TempPath);
            }
            if (!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }
            switch (Periodparsing)
            {
                case TypeArguments.Curr44:
                case TypeArguments.Prev44:
                case TypeArguments.Last44:
                    FileLog = $"{LogPath}{Path.DirectorySeparatorChar}Contracts44_{LocalDate:dd_MM_yyyy}.log";
                    break;
                case TypeArguments.Last223:
                case TypeArguments.Daily223:
                    FileLog = $"{LogPath}{Path.DirectorySeparatorChar}Contracts223_{LocalDate:dd_MM_yyyy}.log";
                    break;
                case TypeArguments.CurrCPD44:
                case TypeArguments.PrevCPD44:
                case TypeArguments.LastCPD44:
                    FileLog = $"{LogPath}{Path.DirectorySeparatorChar}ContractProcedureDocs44_{LocalDate:dd_MM_yyyy}.log";
                    break;
                case TypeArguments.CurrEacts44:
                case TypeArguments.PrevEacts44:
                case TypeArguments.LastEacts44:
                    FileLog = $"{LogPath}{Path.DirectorySeparatorChar}Eacts44_{LocalDate:dd_MM_yyyy}.log";
                    break;
                case TypeArguments.CurrC615:
                case TypeArguments.PrevC615:
                case TypeArguments.LastC615:
                    FileLog = $"{LogPath}{Path.DirectorySeparatorChar}Contract615_{LocalDate:dd_MM_yyyy}.log";
                    break;
            }
        }

        private static void ParserC44(TypeArguments arg)
        {
            Log.Logger("Время начала парсинга Contracts44");
            try
            {
                var c44 = new ParserContr44(StrArg);
                c44.Parsing();
            }
            catch (Exception e)
            {
                Log.Logger(e);
            }
            /*ParserContr44 p = new ParserContr44("last");
            p.ParsingXML("/var/www/admin/data/www/tenders.enter-it.ru/python/Release/contract_2381103776916000030_27318511.xml", "38")*/
            /*ParserContr44 d = new ParserContr44("last");
            d.GetListFileArch("contract_Sankt-Peterburg_2016060100_2016070100_058.xml.zip", "/fcs_regions/Sankt-Peterburg/contracts/", "77");*/
            Log.Logger("Добавлено customer", AddCustomer);
            Log.Logger("Добавлено supplier", AddSupplier);
            Log.Logger("Добавлено contract", AddContract);
            Log.Logger("Обновлено contract", UpdateContract);
            Log.Logger("Добавлено product", AddProduct);
            Log.Logger("Время окончания парсинга Contracts44");
        }
        
        private static void ParserCPD44(TypeArguments arg)
        {
            Log.Logger("Время начала парсинга ContractProcedureDocs44");
            try
            {
                var c44 = new ParserCPD44(StrArg);
                c44.Parsing();
            }
            catch (Exception e)
            {
                Log.Logger(e);
            }
            Log.Logger("Добавлено contract", AddContract);
            Log.Logger("Обновлено contract", UpdateContract);
            Log.Logger("Время окончания парсинга ContractProcedureDocs44");
        }
        
        private static void ParserC615(TypeArguments arg)
        {
            Log.Logger("Время начала парсинга Contract615");
            try
            {
                var c44 = new ParserContract615(StrArg);
                c44.Parsing();
            }
            catch (Exception e)
            {
                Log.Logger(e);
            }
            Log.Logger("Добавлено contract615", AddContract);
            Log.Logger("Обновлено contract615", UpdateContract);
            Log.Logger("Время окончания парсинга Contract615");
        }

        private static void ParserC223(TypeArguments arg)
        {
            Log.Logger("Время начала парсинга Contracts223");
            try
            {
                var c223 = new ParserContr223(StrArg);
                c223.Parsing();
            }
            catch (Exception e)
            {
                Log.Logger(e);
            }
            /*ParserContr223 p = new ParserContr223("last223");
            p.ParsingXml(
                "/home/alex/Рабочий стол/parser/contractCompleting_Adygeya_Resp_20170701_000000_20170731_235959_001.xml",
                "38");*/

            Log.Logger("Добавлено customer", AddCustomer);
            Log.Logger("Добавлено supplier", AddSupplier);
            Log.Logger("Добавлено contract", AddContract);
            Log.Logger("Обновлено contract", UpdateContract);
            Log.Logger("Добавлено product", AddProduct);
            Log.Logger("Время окончания парсинга Contracts223");
        }
        
        private static void ParserEacts44(TypeArguments arg)
        {
            Log.Logger("Время начала парсинга Eacts44");
            try
            {
                var c44 = new ParserEacts44(StrArg);
                c44.Parsing();
            }
            catch (Exception e)
            {
                Log.Logger(e);
            }
            Log.Logger("Добавлено Eacts44", AddContract);
            Log.Logger("Обновлено Eacts44", UpdateContract);
            Log.Logger("Время окончания парсинга Eacts44");
        }
    }
}