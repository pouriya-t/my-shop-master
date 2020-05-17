using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Models.ViewModels
{
    public class SubCategoryVM
    {
        public SubCategory SubCategory { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        public Category Category { get; set; }
    }
}
