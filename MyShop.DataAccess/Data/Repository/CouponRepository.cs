using Microsoft.AspNetCore.Mvc.Rendering;
using MyShop.DataAccess.Data.Repository.IRepository;
using MyShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.Data.Repository
{
    public class CouponRepository : Repository<Coupon>, ICouponRepository
    {
        private readonly ApplicationDbContext _db;

        public CouponRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetCategoryListForDropDown()
        {
            return _db.Category.Select(i => new SelectListItem()
            {
                Value = i.Id.ToString(),
                Text = i.Name,
            });
        }

        public void Update(Coupon coupon)
        {
            var objFormDb = _db.Coupon.FirstOrDefault(s => s.Id == coupon.Id);

            objFormDb.CouponType = coupon.CouponType;
            objFormDb.Discount = coupon.Discount;
            objFormDb.MinimumAmount = coupon.MinimumAmount;
            objFormDb.IsActive = coupon.IsActive;

            _db.SaveChanges();
        }
    }
}
