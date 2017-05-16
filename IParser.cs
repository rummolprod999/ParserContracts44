using System.Data;

namespace ParserContracts44
{
    public interface IParser
    {
        void Parsing();
        DataTable GetRegions();
    }
}