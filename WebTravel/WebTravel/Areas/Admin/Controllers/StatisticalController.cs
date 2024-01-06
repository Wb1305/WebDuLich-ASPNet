using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTravel.Models;

namespace WebTravel.Areas.Admin.Controllers
{
    public class StatisticalController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Admin/Statistical
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetStatistical(string fromDate, string toDate)
        {
            var query = from o in db.Orders
                        join od in db.OrderDetails
                        on o.Id equals od.OrderId
                        join p in db.Products
                        on od.ProductId equals p.Id
                        select new
                        {
                            CreatedDate = o.CreatedDate,
                            Quantity = od.Quantity,
                            Price = od.Price,
                            //OriginalPrice = p.OriginalPrice
                            PriceSale = p.PriceSale
                        };
            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime startDate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate >= startDate);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate < endDate);
            }

            var result = query.GroupBy(x => DbFunctions.TruncateTime(x.CreatedDate)).Select(x => new
            {
                Date = x.Key.Value,
                //TotalSale = (x. != null) ? x.Sum(y => y.Quantity * y.PriceSale) :  x.Sum(y => y.Quantity * y.Price),
                TotalSale = x.Sum(y => y.Quantity * y.PriceSale),
                TotalBuy = x.Sum(y => y.Quantity * y.Price),
            }).Select(x => new
            {
                Date = x.Date,
                DoanhThu = x.TotalBuy,
                //LoiNhuan = x.TotalSell - x.TotalSale
            });
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

    }
}