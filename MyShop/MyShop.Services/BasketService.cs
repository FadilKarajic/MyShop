using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using MyShop.Core.Contracts;
using MyShop.Core.Models;
using MyShop.Core.ViewModels;

namespace MyShop.Services
{
    public class BasketService : IBasketService
    {
        IRepository<Product> ProductContext;
        IRepository<Basket> BasketContext;

        public const string BasketSessionName = "eCommerceBasket";

        public BasketService(IRepository<Product> ProductContext, IRepository<Basket> BasketContext)
        {
            this.BasketContext = BasketContext;
            this.ProductContext = ProductContext;
        }

        private Basket GetBasket(HttpContextBase httpContext, bool createIfNull)
        {
            HttpCookie cookie = httpContext.Request.Cookies.Get(BasketSessionName);
            Basket basket = new Basket();

            if (cookie != null)
            {
                string basketId = cookie.Value;
                if (!string.IsNullOrEmpty(basketId))
                {
                    basket = BasketContext.Find(basketId);
                }
                else
                {
                    if (createIfNull)
                    {
                        basket = CreateNewBasket(httpContext);
                    }
                }
            }
            else
            {
                if (createIfNull)
                {
                    basket = CreateNewBasket(httpContext);
                }
            }

            return basket;
        }

        private Basket CreateNewBasket(HttpContextBase httpContext)
        {
            Basket basket=new Basket();
            BasketContext.Insert(basket);
            BasketContext.Commit();
            
            HttpCookie cookie = new HttpCookie(BasketSessionName);
            cookie.Value = basket.Id;
            //sets cookie expiration time
            cookie.Expires = DateTime.Now.AddDays(1);
            httpContext.Response.Cookies.Add(cookie);

            return basket;
        }

        public void AddToBasket(HttpContextBase httpContext, string productId)
        {
            Basket basket = GetBasket(httpContext, true);
            BasketItem item = basket.BasketItems.FirstOrDefault(i => i.ProductId == productId);

            if (item == null)
            {
                item = new BasketItem()
                {
                    BasketId = basket.Id,
                    ProductId = productId,
                    Quantity = 1
                };
                basket.BasketItems.Add(item);
            }
            else
            {
                item.Quantity = item.Quantity + 1;
            }
            BasketContext.Commit();
        }

        public void RemoveFromBasket(HttpContextBase httpContext, string itemId)
        {
            Basket basket = GetBasket(httpContext, true);
            BasketItem item = basket.BasketItems.FirstOrDefault(i => i.Id == itemId);

            if (itemId != null)
            {
                basket.BasketItems.Remove(item);
                BasketContext.Commit();
            }
        }

        public List<BasketItemViewModel> GetBasketItem(HttpContextBase httpContext)
        {
            Basket basket = GetBasket(httpContext, false);
            if (basket != null)
            {
                var results = (from b in basket.BasketItems
                    join p in ProductContext.Collection() on
                        b.ProductId equals p.Id
                    select new BasketItemViewModel()
                    {
                        Id = b.Id,
                        Quantity = b.Quantity,
                        ProductName = p.Name,
                        Image = p.Image,
                        Price = p.Price
                    }).ToList();
                return results;
            }
            else

            {
                return new List<BasketItemViewModel>();
            }

        }

        public BasketSummaryViewModel GetBasketSummary(HttpContextBase httpContext)
        {
            Basket basket = GetBasket(httpContext, false);
            BasketSummaryViewModel model = new BasketSummaryViewModel(0,0);
            if (basket != null)
            {
                int? basketCount = (from item in basket.BasketItems
                    select
                        item.Quantity).Sum();
                decimal? basketTotal = (from item in basket.BasketItems
                    join p in ProductContext.Collection()
                        on item.ProductId equals p.Id
                    select item.Quantity * p.Price).Sum();
                //if there is a basket count return value of the basket and if null
                //return zero
                model.BasketCount = basketCount ?? 0;
                model.BasketTotal = basketTotal ?? 0;
                return model;
            }
            else
            {
                return model;
            }
        }

        
    }
}
