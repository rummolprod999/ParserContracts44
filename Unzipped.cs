﻿using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace ParserContracts44
{
    public class Unzipped
    {
        public static string Unzip(string filea)
        {
            FileInfo fileInf = new FileInfo(filea);
            if (fileInf.Exists)
            {
                int rPoint = filea.LastIndexOf('.');
                string lDir = filea.Substring(0, rPoint);
                Directory.CreateDirectory(lDir);
                try
                {
                    ZipFile.ExtractToDirectory(filea, lDir);
                    fileInf.Delete();
                    return lDir;
                }
                catch (Exception e)
                {
                    Log.Logger("Не удалось извлечь файл", e, filea);
                    try
                    {
                        var myProcess = new Process {StartInfo = new ProcessStartInfo("unzip", $"-B {filea} -d {lDir}")};
                        myProcess.Start();
                        Log.Logger("Извлекли файл альтернативным методом", filea);
                        myProcess.WaitForExit();
                        return lDir;
                    }
                    catch (Exception exception)
                    {
                        Log.Logger("Не удалось извлечь файл альтернативным методом", exception, filea);
                        return lDir;
                    }
                }
            }

            return "";
        }
    }
}