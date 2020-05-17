using MyShop.DataAccess.Data.Repository.IRepository;
using MyShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.Data.Repository
{
    public class OrderDetailsRepository:Repository<OrderDetails>,IOrderDetailsRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderDetailsRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public void Update(OrderDetails orderDetails)
        {
            var objFormDb = _db.OrderDetails.FirstOrDefault(s => s.Id == orderDetails.Id);

            objFormDb.ServiceName = orderDetails.ServiceName;
            objFormDb.Count = orderDetails.Count;
            objFormDb.Name = orderDetails.Name;
            objFormDb.Description = orderDetails.Description;
            objFormDb.Price = orderDetails.Price;
            

            _db.SaveChanges();
        }
    }
}
