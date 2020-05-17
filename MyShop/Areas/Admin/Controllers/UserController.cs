using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShop.DataAccess.Data.Repository.IRepository;
using MyShop.Models;
using MyShop.Utility;

namespace MyShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Admin)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            return View(_unitOfWork.ApplicationUser.GetAll(u=>u.Id != claims.Value));
        }

        public IActionResult Remove(string id)
        {

            var objFromDb = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == id);
            return View(objFromDb);
        }

        [ActionName("Remove")]
        [HttpPost]
        public IActionResult RemoveUser(ApplicationUser applicationUser)
        {


            var user = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == applicationUser.Id);

            _unitOfWork.ApplicationUser.Remove(user);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Lock(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            _unitOfWork.User.LockUser(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Unlock(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            _unitOfWork.User.UnlockUser(id);
            return RedirectToAction(nameof(Index));
        }
    }
}