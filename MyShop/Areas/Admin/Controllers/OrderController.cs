using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyShop.DataAccess.Data.Repository.IRepository;
using MyShop.Models;
using MyShop.Models.ViewModels;
using MyShop.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Admin + "," + SD.Manager)]
    public class OrderController:Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderViewModel OrderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var order = _unitOfWork.OrderHeader.GetAll();
            return View(order);
        }

        public IActionResult Details(int? id)
        {
            OrderVM = new OrderViewModel();

            OrderVM.OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == id);
            OrderVM.OrderDetailsList = _unitOfWork.OrderDetails.GetAll(o => o.OrderHeaderId == id);

            return View(OrderVM);
        }

        public IActionResult Edit(int? id)
        {
            OrderVM = new OrderViewModel();

            OrderVM.OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == id);
            OrderVM.OrderDetailsList = _unitOfWork.OrderDetails.GetAll(o => o.OrderHeaderId == id);

            return View(OrderVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        public IActionResult EditOrder(OrderViewModel orderView)
        {
            OrderVM = new OrderViewModel();

            OrderVM.OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == orderView.OrderHeader.Id);
            OrderVM.OrderHeader = orderView.OrderHeader;

            _unitOfWork.OrderHeader.Update(OrderVM.OrderHeader);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult EditOrderDetails(int id)
        {


            var objFromDb = _unitOfWork.OrderDetails.GetFirstOrDefault(o => o.Id == id);
            return View(objFromDb);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("EditOrderDetails")]
        public IActionResult EditDetails(OrderDetails orderDetails)
        {

            if (ModelState.IsValid)
            {
                _unitOfWork.OrderDetails.Update(orderDetails);
                _unitOfWork.Save();
            }
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Delete(int? id)
        {
            var objFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.Id == id);
            return View(objFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public IActionResult DeleteOrderHeader(OrderHeader orderHeader)
        {
            _unitOfWork.OrderHeader.Remove(orderHeader);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        



        public IActionResult Approve(int id)
        {
            var orderFromDb = _unitOfWork.OrderHeader.Get(id);
            if (orderFromDb == null)
            {
                return NotFound();
            }
            _unitOfWork.OrderHeader.ChangeOrderStatus(id, SD.StatusApproved);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Reject(int id)
        {
            var orderFromDb = _unitOfWork.OrderHeader.Get(id);
            if (orderFromDb == null)
            {
                return NotFound();
            }
            _unitOfWork.OrderHeader.ChangeOrderStatus(id, SD.StatusRejected);
            return RedirectToAction(nameof(Index));
        }
    }
}
