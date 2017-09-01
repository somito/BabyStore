﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BabyStore.DAL;
using BabyStore.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;

namespace BabyStore.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private StoreContext db = new StoreContext();

        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ??
                HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Orders
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return View(db.Orders.ToList());
            }
            else
            {
                return View(db.Orders.Where(o => o.UserID == User.Identity.Name));
            }

        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Include(o => o.OrderLines).Where(o => o.OrderID == id).SingleOrDefault();

            if (order == null)
            {
                return HttpNotFound();
            }

            if (order.UserID == User.Identity.Name || User.IsInRole("Admin"))
            {
                return View(order);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }
        }

        // GET: Orders/Review
        public async Task<ActionResult> Review()
        {
            Basket basket = Basket.GetBasket();
            Order order = new Order();

            order.UserID = User.Identity.Name;
            ApplicationUser user = await UserManager.FindByNameAsync(order.UserID);
            order.DeliveryName = user.FirstName + " " + user.LastName;
            order.DeliveryAddress = user.Address;
            order.OrderLines = new List<OrderLine>();
            foreach (var basketLine in basket.GetBasketLines())
            {
                OrderLine line = new OrderLine
                {
                    Product = basketLine.Product,
                    ProductID = basketLine.ProductID,
                    ProductName = basketLine.Product.Name,
                    Quantity = basketLine.Quantity,
                    UnitPrice = basketLine.Product.Price
                };
                order.OrderLines.Add(line);
            }
            order.TotalPrice = basket.GetTotalCost();
            return View(order);
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "OrderID,UserID,DeliveryName,DeliveryAddress,TotalPrice,DateCreated")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(order);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
