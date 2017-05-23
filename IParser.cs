using System.Data;

namespace ParserContracts44
{
    public interface IParser
    {
        void Parsing();
        DataTable GetRegions();
        WorkWithFtp ClientFtp44();
        void GetListFileArch(string Arch, string PathParse, string region);
        string GetArch(string Arch, string PathParse);
    }
}