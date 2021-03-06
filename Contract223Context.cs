﻿using System.Data.Entity;
using MySql.Data.Entity;

namespace ParserContracts44
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class Contract223Context : DbContext
    {
        public Contract223Context()
            : base(nameOrConnectionString: ConnectToDb.ConnectString)
        {

        }
        
        public DbSet<Contract223> Contracts223 { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contract223>().ToTable(Program.TableContracts);
            modelBuilder.Entity<Product>().ToTable(Program.TableProducts);
            base.OnModelCreating(modelBuilder);
        }
    }
}