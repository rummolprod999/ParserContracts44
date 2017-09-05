using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParserContracts44
{
    [Table("od_contract")]
    public class Contract223
    {
        [Key, DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        
        [Column("id_contract")]
        public string IdContract { get; set; }
        
        [Column("p_number")]
        public string PNumber { get; set; }
        
        [Column("regnum")]
        public string RegNum { get; set; }
        
        [Column("current_contract_stage")]
        public string CurrentContractStage { get; set; }
        
        [Column("placing")]
        public string Placing { get; set; }
        
        [Column("region_code")]
        public string RegionCode { get; set; }
        
        [Column("url")]
        public string Url { get; set; }
        
        [Column("sign_date")]
        public DateTime SignDate { get; set; }
        
        [Column("single_customer_reason_code")]
        public string SingleCustomerReasonCode { get; set; }
        
        [Column("single_customer_reason_name")]
        public string SingleCustomerReasonName { get; set; }
        
        [Column("fz")]
        public string Fz { get; set; }
        
        [Column("notification_number")]
        public string NotificationNumber { get; set; }
        
        [Column("lot_number")]
        public int LotNumber { get; set; }
        
        [Column("contract_price")]
        public decimal ContractPrice { get; set; }
        
        [Column("currency")]
        public string Currency { get; set; }
        
        [Column("version_number")]
        public int VersionNumber { get; set; }
        
        [Column("execution_start_date")]
        public DateTime ExecutionStartDate { get; set; }
        
        [Column("execution_end_date")]
        public DateTime ExecutionEndDate { get; set; }
        
        [Column("id_customer")]
        public int? CustomerId { get; set; }
        
        public Customer Customer { get; set; }
        
        [Column("id_supplier")]
        public int SupplierId{ get; set; }
        
        [Column("cancel")]
        public int Cancel{ get; set; }
        
        [Column("xml")]
        public string Xml{ get; set; }
        
        public ICollection<Product> Products { get; set; }
        
        public Contract223()
        {
            Products = new List<Product>();
        }
    }
}