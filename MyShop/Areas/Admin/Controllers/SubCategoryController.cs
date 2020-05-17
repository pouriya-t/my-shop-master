using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Markup;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.DataAccess.Data.Repository.IRepository;
using MyShop.Models;
using MyShop.Models.ViewModels;
using MyShop.Utility;

namespace MyShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Admin)]
    public class SubCategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public SubCategoryVM SubCategoryVM { get; set; }

        public SubCategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var subCategories = _unitOfWork.SubCategory.GetAll(includProperties: "Category");
            return View(subCategories);
        }

        public IActionResult Create()
        {
            SubCategoryVM = new SubCategoryVM()
            {
                SubCategory = new SubCategory(),
                CategoryList = _unitOfWork.Category.GetCategoryListForDropDown()
            };

            return View(SubCategoryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SubCategoryVM subCategoryVM)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.SubCategory.Add(subCategoryVM.SubCategory);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(subCategoryVM);
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            SubCategoryVM = new SubCategoryVM()
            {
                SubCategory = new SubCategory(),
                CategoryList = _unitOfWork.Category.GetCategoryListForDropDown()
            };

            if (id != null)
            {
                SubCategoryVM.SubCategory = _unitOfWork.SubCategory.Get(id.GetValueOrDefault());
            }


            return View(SubCategoryVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(SubCategoryVM subCategoryVM)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.SubCategory.Update(subCategoryVM.SubCategory);
                _unitOfWork.Save();
            }
            
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SubCategoryVM = new SubCategoryVM();

            SubCategoryVM.SubCategory = _unitOfWork.SubCategory.Get(id.GetValueOrDefault());
            SubCategoryVM.Category = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == SubCategoryVM.SubCategory.CategoryId);

            return View(SubCategoryVM);
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subCategory = _unitOfWork.SubCategory.Get(id.Value);

            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);
        }

        [HttpPost,ActionName("Delete")]
        [AutoValidateAntiforgeryToken]
        public IActionResult DeleteConfirmed(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var subCategory = _unitOfWork.SubCategory.Get(id.Value);
            _unitOfWork.SubCategory.Remove(subCategory);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }
    }
}