using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;

namespace ParserContracts44
{
    internal class Program
    {
        private static string _database;
        private static string _tempdir;
        private static string _logdir;
        private static string _prefix;
        private static string _user;
        private static string _pass;
        public static string Database => _database;
        public static string Tempdir => _tempdir;
        public static string Logdir => _logdir;
        public static string Prefix => _prefix;
        public static string User => _user;
        public static string Pass => _pass;
        private static readonly DateTime LocalDate = DateTime.Now;
        public static string FileLog;
        public static TypeArguments Periodparsing;

        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(
                    "Недостаточно аргументов для запуска, используйте last или prev или curr в качестве аргумента");
                return;
            }

            switch (args[0])
            {
                case "last":
                    Periodparsing = TypeArguments.Last;
                    Init(Periodparsing);
                    break;
                case "prev":
                    Periodparsing = TypeArguments.Prev;
                    Init(Periodparsing);
                    break;
                case "curr":
                    Periodparsing = TypeArguments.Curr;
                    Init(Periodparsing);
                    break;
            }
        }

        private static void Init(TypeArguments arg)
        {
            GetSettings set = new GetSettings();
            _database = set.Database;
            _logdir = set.LogdirContracts44;
            _prefix = set.Prefix;
            _user = set.UserDB;
            _pass = set.PassDB;
            _tempdir = set.TempdirContracts44;
            if (Directory.Exists(Tempdir))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(Tempdir);
                dirInfo.Delete(true);
                Directory.CreateDirectory(Tempdir);
            }
            else
            {
                Directory.CreateDirectory(Tempdir);
            }
            if (!Directory.Exists(Logdir))
            {
                Directory.CreateDirectory(Logdir);
            }
            FileLog = $"./{Logdir}/{arg}_{LocalDate:dd_MM_yyyy}.log";
            Console.WriteLine(Tempdir +Logdir + Prefix + User + Pass + Database);
        }
    }
}