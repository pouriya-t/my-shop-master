using MyShop.DataAccess.Data.Repository.IRepository;
using MyShop.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.Data.Repository
{
    public class OrderHeaderRepository:Repository<OrderHeader>,IOrderHeaderRepository
    {
        private readonly ApplicationDbContext _db;

        public OrderHeaderRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public void ChangeOrderStatus(int orderHeaderId, string status)
        {
            var orderFromDb = _db.OrderHeader.FirstOrDefault(o => o.Id == orderHeaderId);
            orderFromDb.Status = status;
            _db.SaveChanges();
        }

        public void Update(OrderHeader orderHeader)
        {
            var objFormDb = _db.OrderHeader.FirstOrDefault(s => s.Id == orderHeader.Id);

            objFormDb.UserId = orderHeader.UserId;
            objFormDb.OrderDate = orderHeader.OrderDate;
            objFormDb.OrderTotalOriginal = orderHeader.OrderTotalOriginal;
            objFormDb.OrderTotal = orderHeader.OrderTotal;
            objFormDb.PickUpTime = orderHeader.PickUpTime;
            objFormDb.PickUpDate = orderHeader.PickUpDate;
            objFormDb.Comments = orderHeader.Comments;
            objFormDb.PickupName = orderHeader.PickupName;
            objFormDb.PhoneNumber = orderHeader.PhoneNumber;
            objFormDb.TransactionId = orderHeader.TransactionId;
            objFormDb.Email = orderHeader.Email;


            _db.SaveChanges();
        }
    }
}
