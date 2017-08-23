using BabyStore.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BabyStore.Models
{
    public class Basket
    {
        private string BasketID { get; set; }
        private const string BasketSessionKey = "BasketID";
        private StoreContext db = new StoreContext();

        private string GetBasketID()
        {
            if (HttpContext.Current.Session[BasketSessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name))
                {
                    HttpContext.Current.Session[BasketSessionKey] =
                    HttpContext.Current.User.Identity.Name;
                }
                else
                {
                    Guid tempBasketID = Guid.NewGuid();
                    HttpContext.Current.Session[BasketSessionKey] = tempBasketID.ToString();
                }
            }

            return HttpContext.Current.Session[BasketSessionKey].ToString();
        }

        public static Basket GetBasket()
        {
            Basket basket = new Basket();
            basket.BasketID = basket.GetBasketID();
            return basket;
        }

        public void AddToBasket(int productID, int quantity)
        {
            var basketLine = db.BasketLines.FirstOrDefault(b => b.BasketID == BasketID &&
            b.ProductID == productID);

            if (basketLine == null)
            {
                basketLine = new BasketLine
                {
                    ProductID = productID,
                    BasketID = BasketID,
                    Quantity = quantity,
                    DateCreated = DateTime.Now
                };
                db.BasketLines.Add(basketLine);
            }
            else
            {
                basketLine.Quantity += quantity;
            }
            db.SaveChanges();
        }

        public void RemoveLine(int productID)
        {
            var basketLine = db.BasketLines.FirstOrDefault(bl => bl.BasketID == BasketID && bl.ProductID == productID);

            if (basketLine != null)
            {
                db.BasketLines.Remove(basketLine);
            }
            db.SaveChanges();
        }

        public void UpdateBasket(List<BasketLine> basketLines)
        {
            foreach (var line in basketLines)
            {
                var basketLine = db.BasketLines.FirstOrDefault(bl => bl.BasketID == BasketID && bl.ProductID == line.ProductID);

                if (basketLine != null)
                {
                    if (line.Quantity == 0)
                    {
                        RemoveLine(line.ProductID);
                    }
                    else
                    {
                        basketLine.Quantity = line.Quantity;
                    }
                }
            }
            db.SaveChanges();
        }

        public void EmptyBasket()
        {
            var basketLines = db.BasketLines.Where(bl => bl.BasketID == BasketID);
            foreach (var line in basketLines)
            {
                db.BasketLines.Remove(line);
            }
            db.SaveChanges();
        }

        public List<BasketLine> GetBasketLines()
        {
            return db.BasketLines.Where(bl => bl.BasketID == BasketID).ToList();
        }

        public decimal GetTotalCost()
        {
            decimal basketTotal = decimal.Zero;

            if (GetBasketLines().Count > 0)
            {
                basketTotal = db.BasketLines.Where(bl => bl.BasketID == BasketID).Sum(bl => bl.Product.Price * bl.Quantity);
            }
            return basketTotal;
        }

        public int GetNumberOfItems()
        {
            int numberOfItems = 0;
            if (GetBasketLines().Count > 0)
            {
                numberOfItems = db.BasketLines.Where(bl => bl.BasketID == BasketID).Sum(bl => bl.Quantity);
            }
            return numberOfItems;
        }

        public void MigrateBasket(string userName)
        {
            //find the current basket and store it in memory using ToList()
            var currentBasket = db.BasketLines.Where(bl => bl.BasketID == BasketID).ToList();
            
            //find if the user already has a basket or not and store it in memory using
            //ToList()
            var usersBasket = db.BasketLines.Where(bl => bl.BasketID == userName).ToList();

            //if the user has a basket then add the current items to it
            if (usersBasket != null)
            {
                //set the basketID to the username
                string prevID = BasketID;
                BasketID = userName;

                //add the lines in anonymous basket to the user's basket
                foreach (var line in currentBasket)
                {
                    AddToBasket(line.ProductID, line.Quantity);
                }

                //delete the lines in the anonymous basket from the database
                BasketID = prevID;
                EmptyBasket();
            }
            else
            {
                //if the user does not have a basket then just migrate this one
                foreach (var line in currentBasket)
                {
                    line.BasketID = userName;
                }
                db.SaveChanges();
            }
            HttpContext.Current.Session[BasketSessionKey] = userName;
        }
    }
}