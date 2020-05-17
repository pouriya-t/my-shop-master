using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.DataAccess.Data.Repository.IRepository;
using MyShop.Models;
using MyShop.Models.ViewModels;
using MyShop.Utility;

namespace MyShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Admin + "," + SD.Manager)]
    public class CouponController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public CouponVM CouponVM{ get; set; }

        public CouponController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View(_unitOfWork.Coupon.GetAll());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Coupon coupons)
        {
            if (ModelState.IsValid)
            {
                //var files = HttpContext.Request.Form.Files;
                //if (files.Count > 0)
                //{
                //    byte[] p1 = null;
                //    using (var fs1 = files[0].OpenReadStream())
                //    {
                //        using (var ms1 = new MemoryStream())
                //        {
                //            fs1.CopyTo(ms1);
                //            p1 = ms1.ToArray();
                //        }
                //    }
                //    coupons.Picture = p1;
                //}
                _unitOfWork.Coupon.Add(coupons);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(coupons);
        }


        //GET Edit Coupon
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var coupon = _unitOfWork.Coupon.GetFirstOrDefault(m => m.Id == id);
            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Coupon coupons)
        {
            if (coupons.Id == 0)
            {
                return NotFound();
            }

            var couponFromDb = _unitOfWork.Coupon.GetFirstOrDefault(c => c.Id == coupons.Id);

            if (ModelState.IsValid)
            {
                //var files = HttpContext.Request.Form.Files;
                //if (files.Count > 0)
                //{
                //    byte[] p1 = null;
                //    using (var fs1 = files[0].OpenReadStream())
                //    {
                //        using (var ms1 = new MemoryStream())
                //        {
                //            fs1.CopyTo(ms1);
                //            p1 = ms1.ToArray();
                //        }
                //    }
                //    couponFromDb.Picture = p1;
                //}
                couponFromDb.MinimumAmount = coupons.MinimumAmount;
                couponFromDb.Name = coupons.Name;
                couponFromDb.Discount = coupons.Discount;
                couponFromDb.CouponType = coupons.CouponType;
                couponFromDb.IsActive = coupons.IsActive;

                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(coupons);
        }


        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = _unitOfWork.Coupon
                .GetFirstOrDefault(m => m.Id == id);
            if (coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }

        //GET Delete Coupon
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var coupon = _unitOfWork.Coupon.GetFirstOrDefault(m => m.Id == id);
            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var coupons = _unitOfWork.Coupon.GetFirstOrDefault(m => m.Id == id);
            _unitOfWork.Coupon.Remove(coupons);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}