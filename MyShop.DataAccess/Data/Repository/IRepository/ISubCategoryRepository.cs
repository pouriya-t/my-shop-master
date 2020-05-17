using Microsoft.AspNetCore.Mvc.Rendering;
using MyShop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.Data.Repository.IRepository
{
    public interface ISubCategoryRepository:IRepository<SubCategory>
    {
        IEnumerable<SelectListItem> GetSubCategoryListForDropDown();
        void Update(SubCategory subCategory);
    }
}
