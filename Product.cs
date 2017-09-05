using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParserContracts44
{
    //[Table(Program.TablecProducts)]
    public class Product
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        
        [Column("name")]
        public string Name { get; set; }
        
        [Column("okpd2_code")]
        public string Okpd2Code { get; set; }
        
        [Column("okpd_code")]
        public string OkpdCode { get; set; }
        
        [Column("okpd2_group_code")]
        public int Okpd2GroupCode { get; set; }
        
        [Column("okpd_group_code")]
        public int OkpdGroupCode { get; set; }
        
        [Column("okpd2_group_level1_code")]
        public string Okpd2GroupLevel1Code { get; set; }
        
        [Column("okpd_group_level1_code")]
        public string OkpdGroupLevel1Code { get; set; }
        
        [Column("price")]
        public decimal Price { get; set; }
        
        [Column("okpd2_name")]
        public string Okpd2Name { get; set; }
        
        [Column("okpd_name")]
        public string OkpdName { get; set; }
        
        [Column("quantity")]
        public decimal Quantiity { get; set; }
        
        [Column("okei")]
        public string Okei { get; set; }
        
        [Column("sum")]
        public decimal Sum { get; set; }
        
        [Column("sid")]
        public string Sid { get; set; }
        
        [Column("id_od_contract")]
        public int Contract223Id { get; set; }
        
        public Contract223 Contract223 { get; set; }
    }
}