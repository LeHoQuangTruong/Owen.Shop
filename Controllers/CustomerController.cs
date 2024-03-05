using _20T1020637.BusinessLayers;
using _20T1020637.DomainModels;
using _20T1020637.Web.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _20T1020637.Web.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        // GET: Customer

        /// <summary>
        /// 
        /// </summary>
        private const int PAGE_SIZE = 5;
        private const string CUSTOMER_SEARCH = "SearchCustomerCondition";
        public ActionResult Index()
        {
            PaginationSearchInput condition = Session[CUSTOMER_SEARCH] as PaginationSearchInput;
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
            var data = CommonDataService.ListOfCustomers(condition.Page,
                condition.PageSize,
                condition.SearchValue,
                out rowCount);
            var result = new CustomerSearchOutput()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue,
                RowCount = rowCount,
                Data = data
            };
            Session[CUSTOMER_SEARCH] = condition;
            return View(result);
        }

        /// <summary>
        /// Giap diện để bổ sung khách hàng mới
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            var data = new Customer()
            {
                CustomerID = 0
            };
            ViewBag.Title = "Bổ sung khách hàng";
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

            var data = CommonDataService.GetCustomer(id);
            if (data == null)
                return RedirectToAction("Index");
            int customerId = Convert.ToInt32(id);
            ViewBag.Title = "Cập nhật khách hàng";
            return View(data);
        }
        /// <summary>
        /// Save 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Customer data)
        {
            try
            {
                //Kiểm soát đầu vào
                if (string.IsNullOrWhiteSpace(data.CustomerName))
                    ModelState.AddModelError("CustomerName", "Tên khách hàng không được để trống");
                if (string.IsNullOrWhiteSpace(data.ContactName))
                    ModelState.AddModelError("ContactName", "Tên giao dịch không được để trống!");
                if (string.IsNullOrWhiteSpace(data.Address))
                    ModelState.AddModelError("Address", "Địa chỉ không được để trống!");
                if (string.IsNullOrWhiteSpace(data.Country))
                    ModelState.AddModelError("Country", "Chọn quốc gia!");
                if (string.IsNullOrWhiteSpace(data.City))
                    ModelState.AddModelError("City", "Tên thành phố được để trống!");
                if (string.IsNullOrWhiteSpace(data.PostalCode))
                    ModelState.AddModelError("PostalCode", "Mã bưu chính không được để trống!");
                if (string.IsNullOrWhiteSpace(data.Email))
                    ModelState.AddModelError("Email", "Email không được để trống!");
                if (!ModelState.IsValid)
                {
                    ViewBag.Title = data.CustomerID == 0 ? "Bổ sung khác hàng" : "Cập nhật khách hàng";
                    return View("Edit", data);
                }
                if (data.CustomerID == 0)
                {
                    CommonDataService.AddCustomer(data);
                }
                else
                {
                    CommonDataService.UpdateCustomer(data);
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
                CommonDataService.DeleteCustomer(id);
                return RedirectToAction("Index");
            }

            var data = CommonDataService.GetCustomer(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);

        }

    }
}