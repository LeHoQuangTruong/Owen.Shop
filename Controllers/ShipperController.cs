using _20T1020637.BusinessLayers;
using _20T1020637.DomainModels;
using _20T1020637.Web.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace _20T1020637.Web.Controllers
{
    [Authorize]
    public class ShipperController : Controller
    {
        // GET: Shipper
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private const int PAGE_SIZE = 5;
        private const string SHIPPER_SEARCH = "SearchShipperCondition";

        public ActionResult Index()
        {
            PaginationSearchInput condition = Session[SHIPPER_SEARCH] as PaginationSearchInput;
            if (condition == null)
            {
                condition = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }

            return View(condition);
        }

        public ActionResult Search(PaginationSearchInput condition, int page = 1)
        {
            int rowCount = 0;
            ViewBag.Page = page;
            var data = CommonDataService.ListOfShippers(condition.Page,
                condition.PageSize,
                condition.SearchValue,
                out rowCount);
            var result = new ShipperSearchOutput()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue,
                RowCount = rowCount,
                Data = data
            };
            Session[SHIPPER_SEARCH] = condition;
            return View(result);
        }

        /// <summary>
        /// Giao diện để bôt sung nhân viên giao hàng mới
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            var data = new Shipper()
            {
                ShipperID = 0
            };
            ViewBag.Title = "Bổ sung nhà nhân viên giao hàng";
            return View("Edit", data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {
            if (id == 0)
                return RedirectToAction("Index");

            var data = CommonDataService.GetShipper(id);
            if (data == null)
                return RedirectToAction("Index");
            int shipperID = Convert.ToInt32(id);
            ViewBag.Title = "Cập nhật nhân viên giao hàng";
            return View(data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Shipper data)
        {
            try
            {
                //Kiểm soát đầu vào
                if (string.IsNullOrWhiteSpace(data.ShipperName))
                    ModelState.AddModelError("ShipperName", "Tên nhân viên giao hàng không được để trống");
                if (string.IsNullOrWhiteSpace(data.Phone))
                    ModelState.AddModelError("Phone", "Số điện thoại không được để trống!");

                if (!ModelState.IsValid)
                {
                    ViewBag.Title = data.ShipperID == 0 ? "Bổ sung nhân viên giao hàng" : "Cập nhật nhân viên giao hàng";
                    return View("Edit", data);
                }
                if (data.ShipperID == 0)
                {
                    CommonDataService.AddShipper(data);
                }
                else
                {
                    CommonDataService.UpdateShipper(data);
                }
                return RedirectToAction("Index");

            }
            catch (Exception)
            {
                //Ghi lại log lỗi
                return Content("Có lỗi xảy ra.Vui lòng thử lại sau!");
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Delete(int id = 0)
        {
            if (id == 0)
                return RedirectToAction("Index");

            if (Request.HttpMethod == "POST")
            {
                CommonDataService.DeleteShipper(id);
                return RedirectToAction("Index");
            }

            var data = CommonDataService.GetShipper(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);

        }
    }
}