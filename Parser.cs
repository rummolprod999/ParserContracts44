﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using MySql.Data.MySqlClient;
using MySql.Data.MySqlClient.Memcached;

namespace ParserContracts44
{
    public class Parser : IParser
    {
        protected string arg;

        public Parser(string a)
        {
            this.arg = a;
        }

        public virtual void Parsing()
        {
        }

        public DataTable GetRegions()
        {
            string reg = "SELECT * FROM region";
            DataTable dt;
            using (MySqlConnection connect = ConnectToDb.GetDBConnection())
            {
                connect.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(reg, connect);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                dt = ds.Tables[0];
            }
            return dt;
        }

        public virtual List<String> GetListArchLast(string PathParse, string RegionPath)
        {
            List<String> arch = new List<string>();

            return arch;
        }

        public virtual List<String> GetListArchCurr(string PathParse, string RegionPath)
        {
            List<String> arch = new List<string>();

            return arch;
        }

        public virtual List<String> GetListArchPrev(string PathParse, string RegionPath)
        {
            List<String> arch = new List<string>();

            return arch;
        }

        public WorkWithFtp ClientFtp44()
        {
            WorkWithFtp ftpCl = new WorkWithFtp("ftp://ftp.zakupki.gov.ru", "free", "free");
            return ftpCl;
        }

        public virtual void GetListFileArch(string Arch, string PathParse, string region)
        {
        }

        public string GetArch(string Arch, string PathParse)
        {
            string file = "";
            int count = 0;
            while (true)
            {
                try
                {
                    string FileOnServer = $"{PathParse}/{Arch}";
                    file = $"{Program.TempPath}/{Arch}";
                    WorkWithFtp ftp = ClientFtp44();
                    ftp.DownloadFile(FileOnServer, file);
                    if (count > 0)
                    {
                        Log.Logger("Удалось скачать архив после попытки", count);
                    }
                    return file;
                }
                catch (Exception e)
                {
                    Log.Logger("Не удалось скачать файл", Arch, e);
                    if (count > 50)
                    {
                        return file;
                    }
                    count++;
                    Thread.Sleep(5000);
                }
            }

        }
    }
}