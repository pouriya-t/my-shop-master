﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Models
{
    public class Coupon
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string CouponType { get; set; }

        public enum ECouponType { Percent = 0, Dollar = 1 }

        public double Discount { get; set; }

        public double MinimumAmount { get; set; }


        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
    }
}
