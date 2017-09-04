using System.Data.Entity;
using MySql.Data.Entity;


namespace ParserContracts44
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class Archive223Context : DbContext
    {
        public Archive223Context()
            : base(nameOrConnectionString: ConnectToDb.ConnectString)
        {

        }
        
        public DbSet<ArchiveContracts223> ArchiveContracts223Results { get; set; }
    }
}