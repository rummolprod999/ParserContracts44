using System.Data;
using FluentFTP;

namespace ParserContracts44
{
    public interface IParser
    {
        void Parsing();
        DataTable GetRegions();
        FtpClient ClientFtp44();
        void GetListFileArch(string arch, string pathParse, string region);
        string GetArch44(string arch, string pathParse);
        string GetArch223(string arch, string pathParse);
    }
}