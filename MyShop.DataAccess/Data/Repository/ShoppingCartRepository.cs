using MyShop.DataAccess.Data.Repository.IRepository;
using MyShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.Data.Repository
{
    public class ShoppingCartRepository :Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _db;

        public ShoppingCartRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }


        public void Update(ShoppingCart shoppingCart)
        {
            var objFromDb = _db.ShoppingCart.FirstOrDefault(s => s.Id == shoppingCart.Id);

            objFromDb.Count = shoppingCart.Count;
            objFromDb.ApplicationUserId = shoppingCart.ApplicationUserId;
            objFromDb.ServiceId = shoppingCart.ServiceId;

            _db.SaveChanges();
        }
    }
}
