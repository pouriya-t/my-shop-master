using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using MyShop.DataAccess.Data.Repository;
using MyShop.DataAccess.Data.Repository.IRepository;
using MyShop.Extensions;
using MyShop.Models;
using MyShop.Models.ViewModels;
using MyShop.Utility;
using ZarinpalSandbox;

namespace MyShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = SD.Admin + "," + SD.Manager + "," + SD.User)]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;


        [BindProperty]
        public OrderDetailsCart detailsCart { get; set; }

        

        

        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }



        public IActionResult Index()
        {

            

            detailsCart = new OrderDetailsCart()
            {
                OrderHeader = new OrderHeader(),
            };

            detailsCart.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var cart = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value);
            if (cart != null)
            {
                detailsCart.listCart = cart.ToList();
            }

            foreach (var list in detailsCart.listCart)
            {
                list.Service = _unitOfWork.Service.GetFirstOrDefault(m => m.Id == list.ServiceId);
                detailsCart.OrderHeader.OrderTotal = detailsCart.OrderHeader.OrderTotal +
                    (list.Service.Price * list.Count);
                //list.Service.Description = SD.ConvertToRawHtml(list.Service.Description);
                //if(list.Service.Description.Length > 100)
                //{
                //    list.Service.Description = list.Service.Description.Substring(0, 99) + "...";
                //}
            }
            

            detailsCart.OrderHeader.OrderTotalOriginal = detailsCart.OrderHeader.OrderTotal;

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                detailsCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = _unitOfWork.Coupon.GetFirstOrDefault(c => c.Name.ToLower() == detailsCart.OrderHeader.CouponCode.ToLower());
                detailsCart.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, detailsCart.OrderHeader.OrderTotalOriginal);
            }



            return View(detailsCart);
        }





        
        public IActionResult Summary()
        {
            detailsCart = new OrderDetailsCart()
            {
                OrderHeader = new OrderHeader()
            };

            detailsCart.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(c => c.Id == claim.Value);

            var cart = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value);

            if (cart != null)
            {
                detailsCart.listCart = cart.ToList();
            }

            foreach (var list in detailsCart.listCart)
            {
                list.Service = _unitOfWork.Service.GetFirstOrDefault(s => s.Id == list.ServiceId);
                detailsCart.OrderHeader.OrderTotal = detailsCart.OrderHeader.OrderTotal +
                    (list.Service.Price * list.Count);
            }
            detailsCart.OrderHeader.OrderTotalOriginal = detailsCart.OrderHeader.OrderTotal;
            detailsCart.OrderHeader.PickupName = applicationUser.Name;
            detailsCart.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
            detailsCart.OrderHeader.Email = applicationUser.Email;
            detailsCart.OrderHeader.Comments = applicationUser.StreetAddress;
            detailsCart.OrderHeader.PickUpTime = DateTime.Now;

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                detailsCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = _unitOfWork.Coupon.GetFirstOrDefault(c => c.Name.ToLower() == detailsCart.OrderHeader.CouponCode.ToLower());
                detailsCart.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, detailsCart.OrderHeader.OrderTotalOriginal);
            }

            return View(detailsCart);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            detailsCart.listCart = _unitOfWork.ShoppingCart.GetAll(c => c.ApplicationUserId == claim.Value).ToList();

            detailsCart.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            detailsCart.OrderHeader.OrderDate = DateTime.Now;
            detailsCart.OrderHeader.UserId = claim.Value;
            detailsCart.OrderHeader.Status = SD.PaymentStatusPending;
            detailsCart.OrderHeader.PickUpTime = Convert.ToDateTime(detailsCart.OrderHeader.PickUpDate.ToShortDateString()
                + " " + detailsCart.OrderHeader.PickUpTime.ToShortTimeString());

            List<OrderDetails> orderDetailsList = new List<OrderDetails>();
            _unitOfWork.OrderHeader.Add(detailsCart.OrderHeader);
            _unitOfWork.Save();

            detailsCart.OrderHeader.OrderTotalOriginal = 0;


            Random RNG = new Random();
            var builder = new StringBuilder();
            while (builder.Length < 16)
            {
                builder.Append(RNG.Next(10).ToString());
            }
            long transactionId = Convert.ToInt64(builder.ToString());




            foreach (var item in detailsCart.listCart)
            {
                item.Service = _unitOfWork.Service.GetFirstOrDefault(m => m.Id == item.ServiceId);
                OrderDetails orderDetails = new OrderDetails
                {
                    ServiceId = item.ServiceId,
                    OrderHeaderId = detailsCart.OrderHeader.Id,
                    Description = item.Service.Description,
                    ServiceName = item.Service.Name,
                    Price = item.Service.Price,
                    Count = item.Count
                };
                detailsCart.OrderHeader.OrderTotalOriginal += orderDetails.Count * orderDetails.Price;
                detailsCart.OrderHeader.TransactionId = transactionId.ToString();
                _unitOfWork.OrderDetails.Add(orderDetails);

            }

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                detailsCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb =_unitOfWork.Coupon.GetFirstOrDefault(c => c.Name.ToLower() == detailsCart.OrderHeader.CouponCode.ToLower());
                detailsCart.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, detailsCart.OrderHeader.OrderTotalOriginal);
            }
            else
            {
                detailsCart.OrderHeader.OrderTotal = detailsCart.OrderHeader.OrderTotalOriginal;
            }
            detailsCart.OrderHeader.CouponCodeDiscount = detailsCart.OrderHeader.OrderTotalOriginal - detailsCart.OrderHeader.OrderTotal;





            _unitOfWork.ShoppingCart.RemoveRange(detailsCart.listCart);
            HttpContext.Session.SetInt32(SD.ssShoppingCartCount, 0);
            _unitOfWork.Save();

            //var options = new ChargeCreateOptions
            //{
            //    Amount = Convert.ToInt32(detailCart.OrderHeader.OrderTotal * 100),
            //    Currency = "usd",
            //    Description = "Order ID : " + detailCart.OrderHeader.Id,
            //    Source = stripeToken

            //};
            //var service = new ChargeService();
            //Charge charge = service.Create(options);

            //if (charge.BalanceTransactionId == null)
            //{
            //    detailCart.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
            //}
            //else
            //{
            //    detailCart.OrderHeader.TransactionId = charge.BalanceTransactionId;
            //}

            //if (charge.Status.ToLower() == "succeeded")
            //{
            //    detailCart.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
            //    detailCart.OrderHeader.Status = SD.StatusSubmitted;
            //}
            //else
            //{
            //    detailCart.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
            //}

            //await _db.SaveChangesAsync();
            //return RedirectToAction("Index", "Home");
            //return RedirectToAction("OrderConfirmation", "Cart" ,  new { id = transactionId  } );


            return RedirectToAction("OrderConfirmation","Cart",new { id = transactionId });
            //return RedirectToAction("Payment","Cart",new { id = transactionId });




            //return RedirectToAction("OrderConfirmation", "Cart", new { id = transactionId });



        }

        public IActionResult Payment(long id)
        {
            var order = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.TransactionId == id.ToString());
            if (order == null)
            {
                return NotFound();
            }

            var payment = new Payment(Convert.ToInt32(order.OrderTotal));
            var res = payment.PaymentRequest($"پرداخت فاکتور شماره : {order.TransactionId}"
                , "https://localhost:44381/Customer/Cart/OnlinePayment/"+order.TransactionId,
                order.Email,order.PhoneNumber);

            if (res.Result.Status == 100)
            {
                return Redirect("https://sandbox.zarinpal.com/pg/StartPay/" + res.Result.Authority);
            }
            else
            {
                return BadRequest();
            }

            
        }

        public IActionResult OnlinePayment(long id)
        {
            if (HttpContext.Request.Query["Status"] != "" &&
                HttpContext.Request.Query["Status"].ToString().ToLower() == "ok" &&
                HttpContext.Request.Query["Authority"] != "" )
            {
                string authority = HttpContext.Request.Query["Authority"].ToString();
                var order = _unitOfWork.OrderHeader.GetFirstOrDefault(o => o.TransactionId == id.ToString());
                var payment = new Payment(Convert.ToInt32(order.OrderTotal));
                var res = payment.Verification(authority).Result;
                if(res.Status== 100)
                {
                    ViewBag.code = res.RefId;
                    return View();
                }
            }
            return NotFound();
        }

        public IActionResult OrderConfirmation(long id)
        {
            return View(id);
        }

        //public IActionResult OrderConfirmation(int id)
        //{
        //    return View(id);
        //}


        public IActionResult ViewDetails()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            OrderViewModel OrderVM = new OrderViewModel();

            OrderVM.OrderHeaderList = _unitOfWork.OrderHeader.GetAll(o => o.UserId == claim.Value);
            OrderVM.OrderDetailsList = _unitOfWork.OrderDetails.GetAll();


            return View(OrderVM);
        }

        public IActionResult AddCoupon()
        {
            if (detailsCart.OrderHeader.CouponCode == null)
            {
                detailsCart.OrderHeader.CouponCode = "";
            }
            HttpContext.Session.SetString(SD.ssCouponCode, detailsCart.OrderHeader.CouponCode);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveCoupon()
        {

            HttpContext.Session.SetString(SD.ssCouponCode, string.Empty);

            return RedirectToAction(nameof(Index));
        }


        public IActionResult Plus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId);
            cart.Count += 1;
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId);
            if (cart.Count == 1)
            {
                _unitOfWork.ShoppingCart.Remove(cart);
                _unitOfWork.Save();

                var cnt = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
                HttpContext.Session.SetInt32(SD.ssShoppingCartCount, cnt);
            }
            else
            {
                cart.Count -= 1;
                _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(c => c.Id == cartId);

            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();

            var cnt = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32(SD.ssShoppingCartCount, cnt);


            return RedirectToAction(nameof(Index));
        }
    }
}