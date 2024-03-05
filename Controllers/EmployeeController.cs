using _20T1020637.BusinessLayers;
using _20T1020637.DomainModels;
using _20T1020637.Web.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.UI;

namespace _20T1020637.Web.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {

        private const int PAGE_SIZE = 6;
        private const string EMPLOYEE_SEARCH = "SearchEmployeeCondition";

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            PaginationSearchInput condition = Session[EMPLOYEE_SEARCH] as PaginationSearchInput;
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
            var data = CommonDataService.ListOfEmployees(condition.Page,
                                                         condition.PageSize,
                                                         condition.SearchValue,
                                                         out rowCount);
            var result = new EmployeeSearchOutput()
            {
                Page = condition.Page,
                PageSize = condition.PageSize,
                SearchValue = condition.SearchValue,
                RowCount = rowCount,
                Data = data
            };
            Session[EMPLOYEE_SEARCH] = condition;
              return View(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            var data = new Employee()
            {
                EmployeeID = 0
            };
            ViewBag.Title = "Bổ sung nhân viên";
            return View("Edit", data);
        }
        /// <summary>
        /// Upload Image
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private string UploadFile(HttpPostedFileBase file)
        {
            try
            {
                if (file != null)
                {
                    string path = Server.MapPath("~/Uploads/Employee");
                    // lay file Name upload photo
                    string fileName = $"{DateTime.Now.Ticks}_{file.FileName}";
                    //duong dan upload
                    string filePath = System.IO.Path.Combine(path, fileName);
                    file.SaveAs(filePath);
                    return $"/Uploads/Employee/{fileName}";
                }
                return "";
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit(int id = 0)
        {
            if (id == 0)
                return RedirectToAction("Index");

            var data = CommonDataService.GetEmployee(id);
            if (data == null)
                return RedirectToAction("Index");
            ViewBag.Title = "Cập nhật nhân viên";
            return View(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Save(Employee data, string birthday, HttpPostedFileBase uploadPhoto)
        {
            DateTime? d = Converter.DMYStringToDateTime(birthday);

            if (d == null)
                ModelState.AddModelError("BirthDate", $"Ngày {birthday} không hợp lệ! Vui lòng nhập ngày sinh theo định dạng dd/MM/yyyy");
            else
                data.BirthDate = d.Value;
            try
            {
                string imageFile = UploadFile(uploadPhoto);
                if (imageFile != "")
                    data.Photo = imageFile;

                if (string.IsNullOrWhiteSpace(data.LastName))
                    ModelState.AddModelError("LastName","Họ không được để trống!");
                if (string.IsNullOrWhiteSpace(data.FirstName))
                    ModelState.AddModelError("FirstName", "Tên không được để trống!");
                if (string.IsNullOrWhiteSpace(data.BirthDate.ToString()))
                    ModelState.AddModelError("BirthDate", "Vui lòng thêm ảnh!");
                if (string.IsNullOrWhiteSpace(data.Photo))
                    ModelState.AddModelError("Photo", "Vui lòng thêm ảnh!");
                if (string.IsNullOrWhiteSpace(data.Notes))
                    ModelState.AddModelError("Notes", "Vui lòng diền ghi chú!");
                if (string.IsNullOrWhiteSpace(data.Email))
                    ModelState.AddModelError("Email", "Email không được để trống");

                if (!ModelState.IsValid)
                {
                    ViewBag.Title = data.EmployeeID == 0 ? "Bổ sung nhân viên" : "Cập nhật nhân viên";
                    return View("Edit", data);
                }

                
                if (data.EmployeeID == 0)
                {
                    CommonDataService.AddEmployee(data);
                }
                else
                {
                    CommonDataService.UpdateEmployee(data);
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                //Ghi lại log lỗi
                return Content("Có lỗi xảy ra.Vui lòng thử lại sau!" + ex.Message);
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
                CommonDataService.DeleteEmployee(id);
                return RedirectToAction("Index");
            }

            var data = CommonDataService.GetEmployee(id);
            if (data == null)
                return RedirectToAction("Index");
            return View(data);
        }

    }
}