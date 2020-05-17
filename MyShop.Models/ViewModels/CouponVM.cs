using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Models.ViewModels
{
    public class CouponVM
    {
        public Coupon Coupon { get; set; }
        public IEnumerable<SelectListItem> Services { get; set; }
    }
}
