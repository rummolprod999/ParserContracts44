using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.IO;
using System.Globalization;

namespace ParserContracts44
{
    internal class Program
    {
        private static string _database;
        private static string _tempPath;
        private static string _logPath;
        private static string _prefix;
        private static string _user;
        private static string _pass;
        public static string Database => _database;
        public static string TempPath => _tempPath;
        public static string LogPath => _logPath;
        public static string Prefix => _prefix;
        public static string User => _user;
        public static string Pass => _pass;
        public static readonly DateTime LocalDate = DateTime.Now;
        public static string FileLog;
        public static TypeArguments Periodparsing;
        public static string StrArg;
        public static int AddCustomer = 0;
        public static int AddSupplier = 0;
        public static int AddContract = 0;
        public static int AddProduct = 0;
        public static int UpdateContract = 0;

        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(
                    "Недостаточно аргументов для запуска, используйте last или prev или curr в качестве аргумента");
                return;
            }

            StrArg = args[0];
            switch (args[0])
            {
                case "last":
                    Periodparsing = TypeArguments.Last;
                    Init(Periodparsing);
                    ParserC44(Periodparsing);
                    break;
                case "prev":
                    Periodparsing = TypeArguments.Prev;
                    Init(Periodparsing);
                    ParserC44(Periodparsing);
                    break;
                case "curr":
                    Periodparsing = TypeArguments.Curr;
                    Init(Periodparsing);
                    ParserC44(Periodparsing);
                    break;
            }
        }

        private static void Init(TypeArguments arg)
        {
            GetSettings set = new GetSettings();
            _database = set.Database;
            _logPath = set.LogPathContracts44;
            _prefix = set.Prefix;
            _user = set.UserDB;
            _pass = set.PassDB;
            _tempPath = set.TempPathContracts44;
            if (Directory.Exists(TempPath))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(TempPath);
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
            FileLog = $"{LogPath}/Contracts44_{LocalDate:dd_MM_yyyy}.log";
        }

        private static void ParserC44(TypeArguments arg)
        {
            Log.Logger("Время начала парсинга Contracts44");
            ParserContr44 c44 = new ParserContr44(StrArg);
            c44.Parsing();
            /*ParserContr44 p = new ParserContr44("last");
            p.ParsingXML("/home/alex/RiderProjects/ParserContracts44/bin/Debug/contract_3312100001816000003_25649142.xml", "32");*/

        }
    }
}