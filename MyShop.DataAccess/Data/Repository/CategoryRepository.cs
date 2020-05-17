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
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db):base(db)
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

        public void Update(Category category)
        {
            var objFormDb = _db.Category.FirstOrDefault(s => s.Id == category.Id);

            objFormDb.Name = category.Name;

            _db.SaveChanges();
        }
    }
}
