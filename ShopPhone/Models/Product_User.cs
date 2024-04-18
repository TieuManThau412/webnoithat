using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ShopNoiThat.Models
{
    [Table("Product_User")]
    public class Product_User
    {
        public int Id { get; set; }

        public int IdUser { get; set; }

        [StringLength(50)]
        public string NameProduct { get; set; }

        public string Description { get; set; }
        public string PhoTo { get; set; }
        public DateTime CreateDate { get; set; }
        public Boolean IsPhanHoi { get; set; }
        public double Price { get; set; }
        public string Image { get; set; }
    }
}