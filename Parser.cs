﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using FluentFTP;
using Limilabs.FTP.Client;
using MySql.Data.MySqlClient;

namespace ParserContracts44
{
    public class Parser : IParser
    {
        protected string Arg;
        public Func<WorkWithFtp> Wftp44;
        public Func<WorkWithFtp> Wftp223;
        public Parser(string a)
        {
            this.Arg = a;
            Wftp44 = ClientFtp44_old;
            Wftp223 = ClientFtp223_old;
        }

        public virtual void Parsing()
        {
        }

        public DataTable GetRegions()
        {
            var reg = "SELECT * FROM region";
            DataTable dt;
            using (var connect = ConnectToDb.GetDbConnection())
            {
                connect.Open();
                var adapter = new MySqlDataAdapter(reg, connect);
                var ds = new DataSet();
                adapter.Fill(ds);
                dt = ds.Tables[0];
            }
            return dt;
        }

        public virtual List<String> GetListArchLast(string pathParse, string regionPath)
        {
            var arch = new List<string>();

            return arch;
        }

        public virtual List<String> GetListArchCurr(string pathParse, string regionPath)
        {
            var arch = new List<string>();

            return arch;
        }

        public virtual List<String> GetListArchPrev(string pathParse, string regionPath)
        {
            var arch = new List<string>();

            return arch;
        }

        public WorkWithFtp ClientFtp44_old()
        {
            var ftpCl = new WorkWithFtp("ftp://ftp.zakupki.gov.ru", "free", "VNIMANIE!_otkluchenie_FTP_s_01_01_2025_podrobnee_v_ATFF");
            return ftpCl;
        }
        
        public WorkWithFtp ClientFtp223_old()
        {
            var ftpCl = new WorkWithFtp("ftp://ftp.zakupki.gov.ru", "fz223free", "VNIMANIE!_otkluchenie_FTP_s_01_01_2025_podrobnee_v_ATFF");
            return ftpCl;
        }
        
        public FtpClient ClientFtp44()
        {
            var client = new FtpClient("ftp://ftp.zakupki.gov.ru", "free", "VNIMANIE!_otkluchenie_FTP_s_01_01_2025_podrobnee_v_ATFF");
            client.Connect();
            return client;
        }
        
        public FtpClient ClientFtp223()
        {
            var client = new FtpClient("ftp.zakupki.gov.ru", "fz223free", "VNIMANIE!_otkluchenie_FTP_s_01_01_2025_podrobnee_v_ATFF");
            client.Connect();
            return client;
        }

        public virtual void GetListFileArch(string arch, string pathParse, string region)
        {
        }

        public string GetArch44(string arch, string pathParse)
        {
            var file = "";
            var count = 1;
            while (true)
            {
                try
                {
                    var fileOnServer = $"{pathParse}/{arch}";
                    file = $"{Program.TempPath}{Path.DirectorySeparatorChar}{arch}";
                    var ftp = ClientFtp44();
                    ftp.SetWorkingDirectory(pathParse);
                    ftp.DownloadFile(file, fileOnServer);
                    ftp.Disconnect();
                    /*using (Ftp client = new Ftp())
                    {
                        client.Connect("ftp.zakupki.gov.ru");
                        client.Login("free", "free");
                        client.ChangeFolder(pathParse);
                        client.Download(fileOnServer, file);
                        client.Close();
                    }*/
                    if (count > 1)
                    {
                        Log.Logger("Удалось скачать архив после попытки", count, pathParse);
                    }
                    return file;
                }
                catch (Exception e)
                {
                    Log.Logger("Не удалось скачать файл", arch, e);
                    if (count > 50)
                    {
                        return file;
                    }

                    count++;
                    Thread.Sleep(5000);
                }
            }

        }
        
        public string GetArch223(string arch, string pathParse)
        {
            var file = "";
            var count = 1;
            while (true)
            {
                try
                {
                    var fileOnServer = $"{pathParse}/{arch}";
                    file = $"{Program.TempPath}{Path.DirectorySeparatorChar}{arch}";
                    var ftp = ClientFtp223();
                    ftp.SetWorkingDirectory(pathParse);
                    ftp.DownloadFile(file, fileOnServer);
                    ftp.Disconnect();
                    /*using (Ftp client = new Ftp())
                    {
                        client.Connect("ftp.zakupki.gov.ru");
                        client.Login("fz223free", "fz223free");
                        client.ChangeFolder(pathParse);
                        client.Download(fileOnServer, file);
                        client.Close();
                    }*/
                    if (count > 1)
                    {
                        Log.Logger("Удалось скачать архив после попытки", count, pathParse);
                    }
                    return file;
                }
                catch (Exception e)
                {
                    Log.Logger("Не удалось скачать файл", arch, e);
                    if (count > 50)
                    {
                        return file;
                    }

                    count++;
                    Thread.Sleep(5000);
                }
            }

        }
        
        public List<string> GetListFtp(string PathParse, Func<WorkWithFtp> connectFtp)
        {
            var archtemp = new List<string>();
            var count = 1;
            while (true)
            {
                try
                {
                    var ftp = connectFtp?.Invoke();
                    ftp.ChangeWorkingDirectory(PathParse);
                    archtemp = ftp.ListDirectory();
                    if (count > 1)
                    {
                        Log.Logger("Удалось получить список архивов после попытки", count);
                    }
                    break;
                }
                catch (Exception e)
                {
                    if (count > 3)
                    {
                        Log.Logger($"Не смогли найти директорию после попытки {count}", PathParse, e);
                        break;
                    }
                    count++;
                    Thread.Sleep(2000);
                }
            }
            return archtemp;
        }
    }
}