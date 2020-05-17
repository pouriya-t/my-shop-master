using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Models.ViewModels
{
    public class OrderViewModel
    {
        public OrderHeader OrderHeader { get; set; }
        //public OrderDetails OrderDetails { get; set; }
        //public Service Service { get; set; }
        public IEnumerable<OrderHeader> OrderHeaderList { get; set; }
        public IEnumerable<OrderDetails> OrderDetailsList { get; set; }
    }
}
