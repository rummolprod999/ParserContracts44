using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParserContracts44
{
    [Table("od_customer")]
    public class Customer
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        
        [Column("regNumber")]
        public string RegNumber { get; set; }
        
        [Column("inn")]
        public string Inn { get; set; }
        
        [Column("kpp")]
        public string Kpp { get; set; }
        
        [Column("contracts_count")]
        public int ContractsCount { get; set; }
        
        [Column("contracts223_count")]
        public int Contracts223Count { get; set; }
        
        [Column("contracts_sum")]
        public decimal ContractsSum { get; set; }
        
        [Column("contracts223_sum")]
        public decimal Contracts223Sum { get; set; }
        
        [Column("ogrn")]
        public string Ogrn { get; set; }
        
        [Column("region_code")]
        public string RegionCode { get; set; }
        
        [Column("full_name")]
        public string FullName { get; set; }
        
        [Column("postal_address")]
        public string PostalAddress { get; set; }
        
        [Column("phone")]
        public string Phone { get; set; }
        
        [Column("fax")]
        public string Fax { get; set; }
        
        [Column("email")]
        public string Email { get; set; }
        
        [Column("contact_name")]
        public string ContactName { get; set; }
        
        [Column("short_name")]
        public string ShortName { get; set; }
        
        public ICollection<Contract223> Contracts223 { get; set; }
        
        public Customer()
        {
            Contracts223 = new List<Contract223>();
        }
    }
}