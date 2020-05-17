using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using MyShop.DataAccess.Data.Repository.IRepository;
using MyShop.Models;
using MyShop.Models.ViewModels;
using MyShop.Utility;

namespace MyShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Admin)]
    public class ServiceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        [BindProperty]
        public ServiceVM ServiceVM { get; set; }

        public ServiceController(IUnitOfWork unitOfWork,IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            var service = _unitOfWork.Service.GetAll(includProperties: "Category,SubCategory");
            return View(service);
        }

        public IActionResult Create()
        {
            ServiceVM = new ServiceVM()
            {
                Service = new Service(),
                CategoryList = _unitOfWork.Category.GetCategoryListForDropDown(),
                SubCategoryList = _unitOfWork.SubCategory.GetSubCategoryListForDropDown()
            };

            return View(ServiceVM);
        }

        [HttpPost,ActionName("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult CreateService()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (ServiceVM.Service.ImageUrl != "")
                    {
                        string webRootPath = _hostEnvironment.WebRootPath;
                        var files = HttpContext.Request.Form.Files;

                        string fileName = Guid.NewGuid().ToString();
                        var uploads = Path.Combine(webRootPath, @"images/services");
                        var extension = Path.GetExtension(files[0].FileName);

                        using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStreams);
                        }

                        ServiceVM.Service.ImageUrl = @"/images/services/" + fileName + extension;

                    }
                }
                catch
                {

                }

                //serviceVM.Service.ImageUrl = @"\images\services" + fileName + extension;

                _unitOfWork.Service.Add(ServiceVM.Service);
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

            ServiceVM = new ServiceVM();

            ServiceVM.Service = _unitOfWork.Service.Get(id.GetValueOrDefault());
            ServiceVM.Category = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == ServiceVM.Service.CategoryId);
            ServiceVM.SubCategory = _unitOfWork.SubCategory.GetFirstOrDefault(c => c.Id == ServiceVM.Service.SubCategoryId);

            return View(ServiceVM);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ServiceVM = new ServiceVM()
            {
                Service = new Service(),
                CategoryList = _unitOfWork.Category.GetCategoryListForDropDown(),
                SubCategoryList = _unitOfWork.SubCategory.GetSubCategoryListForDropDown()
            };

            //ServiceVM = new ServiceVM();
            ServiceVM.Service = _unitOfWork.Service.Get(id.GetValueOrDefault());
            ServiceVM.Category = _unitOfWork.Category.GetFirstOrDefault(c => c.Id == ServiceVM.Service.CategoryId);
            ServiceVM.SubCategory = _unitOfWork.SubCategory.GetFirstOrDefault(s => s.Id == ServiceVM.Service.SubCategoryId);

            return View(ServiceVM);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Edit()
        {
            if (ModelState.IsValid)
            {
                var serviceFromDb = _unitOfWork.Service.Get(ServiceVM.Service.Id);

                string webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;

                if (files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"images/services");
                    var extension_new = Path.GetExtension(files[0].FileName);

                    if(serviceFromDb.ImageUrl != null)
                    {
                        var imagePath = Path.Combine(webRootPath, serviceFromDb.ImageUrl.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    

                    using(var fileStreams = new FileStream(Path.Combine(uploads,fileName + extension_new),FileMode.Create))
                    {
                        files[0].CopyTo(fileStreams);
                    }

                    ServiceVM.Service.ImageUrl = @"/images/services/" + fileName + extension_new;
                }
                else
                {
                    ServiceVM.Service.ImageUrl = serviceFromDb.ImageUrl;
                }
                _unitOfWork.Service.Update(ServiceVM.Service);
                _unitOfWork.Save();
            }
            return RedirectToAction(nameof(Index));
            
        }

        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = _unitOfWork.Service.Get(id.Value);

            if (service == null)
            {
                return NotFound();
            }

            return View(service);
        }

        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int? id)
        {
            var serviceFromDb = _unitOfWork.Service.GetFirstOrDefault(s=>s.Id == id);

            if(serviceFromDb.ImageUrl != null)
            {
                string webRootPath = _hostEnvironment.WebRootPath;
                var imagePath = Path.Combine(webRootPath, serviceFromDb.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }

            _unitOfWork.Service.Remove(serviceFromDb);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));

        }
    }
}