using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyShop.DataAccess;
using MyShop.DataAccess.Data.Repository.IRepository;
using MyShop.Extensions;
using MyShop.Models;
using MyShop.Models.ViewModels;
using MyShop.Utility;

namespace MyShop.Areas.Customer
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private HomeViewModel HomeVM;


        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            HomeVM = new HomeViewModel()
            {
                CategoryList = _unitOfWork.Category.GetAll(),
                ServiceList = _unitOfWork.Service.GetAll(includProperties: "Category,SubCategory")
            };


            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var cnt = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value);
                
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount,cnt.Count());
            }


            return View(HomeVM);
        }

        
        [HttpGet]
        [Authorize]
        public IActionResult Details(int? id)
        {
            var menuItemFromDb = _unitOfWork.Service.GetFirstOrDefault(m => m.Id == id, includeProperties: "Category,SubCategory");

            ShoppingCart shoppingCart = new ShoppingCart()
            {
                Service = menuItemFromDb,
                ServiceId = menuItemFromDb.Id
            };

            return View(shoppingCart);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Details(ShoppingCart CartObject)
        {
            CartObject.Id = 0;
            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                CartObject.ApplicationUserId = claim.Value;

                ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                    c => c.ApplicationUserId == CartObject.ApplicationUserId &&
                    c.ServiceId == CartObject.ServiceId);

                if(cartFromDb == null)
                {
                    _unitOfWork.ShoppingCart.Add(CartObject);
                }

                else
                {
                    cartFromDb.Count = cartFromDb.Count + CartObject.Count;
                }

                _unitOfWork.Save();

                var count = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.ApplicationUserId == CartObject.ApplicationUserId).Count;
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, count);

                return RedirectToAction(nameof(Index));
            }
            else
            {
                var serviceFromDb = _unitOfWork.Service.GetFirstOrDefault(m => m.Id == CartObject.ServiceId);

                ShoppingCart cartObj = new ShoppingCart()
                {
                    Service = serviceFromDb,
                    ServiceId = serviceFromDb.Id
                };

                return View(cartObj);
            }
        }

        public IActionResult AddToCart(int serviceId)
        {
            List<int> sessionList = new List<int>();
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(SD.SessionCart)))
            {
                sessionList.Add(serviceId);
                HttpContext.Session.SetObject(SD.SessionCart, sessionList);
            }
            else
            {
                sessionList = HttpContext.Session.GetObject<List<int>>(SD.SessionCart);
                if (!sessionList.Contains(serviceId))
                {
                    sessionList.Add(serviceId);
                    HttpContext.Session.SetObject(SD.SessionCart, sessionList);
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
