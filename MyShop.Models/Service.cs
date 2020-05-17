using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        
        [DataType(DataType.ImageUrl)]
        public string ImageUrl { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        [Display(Name = "SubCategory")]
        public int? SubCategoryId { get; set; }

        [ForeignKey("SubCategoryId")]
        public virtual SubCategory SubCategory { get; set; }

        


    }
}
