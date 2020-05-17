using Microsoft.AspNetCore.Mvc.Rendering;
using MyShop.DataAccess.Data.Repository.IRepository;
using MyShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.Data.Repository
{
    public class SubCategoryRepository : Repository<SubCategory>, ISubCategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public SubCategoryRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetSubCategoryListForDropDown()
        {
            return _db.SubCategory.Select(i => new SelectListItem()
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
        }
        public void Update(SubCategory subCategory)
        {
            
            var objFromDb = _db.SubCategory.FirstOrDefault(s => s.Id == subCategory.Id);

            objFromDb.Name = subCategory.Name;
            objFromDb.CategoryId = subCategory.CategoryId;

            _db.SaveChanges();
        }
    }
}
