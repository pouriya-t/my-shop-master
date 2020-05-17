using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyShop.Models
{
    public class ShoppingCart
    {
        public ShoppingCart()
        {
            Count = 1;
        }

        public int Id { get; set; }

        public string ApplicationUserId { get; set; }

        [NotMapped]
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }


        public int ServiceId { get; set; }

        [NotMapped]
        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }


        [Range(1,int.MaxValue,ErrorMessage = "Please enter a value greater than or equal to {1}")]
        public int Count { get; set; }

    }
}
