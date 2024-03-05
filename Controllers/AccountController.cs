using _20T1020637.BusinessLayers;
using _20T1020637.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace _20T1020637.Web.Controllers
{
    //Cần login để xem 
    [Authorize]
    public class AccountController : Controller
    {
        // GET: Account
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }


        [ValidateAntiForgeryToken]
        //cho phep truy cập mà k cần đăng nhập
        [AllowAnonymous]
        //PT quy định của controller. Những thông tin điền vào form đều ẩn - gửi thông qua body
        //Dùng khi login, register, create - update data...
        [HttpPost]
        public ActionResult Login(string userName = "", string password = "")
        {
            ViewBag.UserName = userName;
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {

                ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin!");
                return View();
            }

            var userAccount = UserAccountService.Authorize(AccountTypes.Employee, userName, password);

            if (userAccount == null)
            {
                ModelState.AddModelError("", "Đăng nhập thất bại!");
                return View();
            }

            // -> JSON; cookie chỉ set được với chuỗi string; phân giải thành chuỗi dạng json
            string cookieValue = Newtonsoft.Json.JsonConvert.SerializeObject(userAccount);
            FormsAuthentication.SetAuthCookie(cookieValue, false);
            return RedirectToAction("Index", "Home");

        }

        public ActionResult Logout()
        {
            //Xoa thong tin dang nhap cua nguoi dung
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }

        public ActionResult ChangePassword(string userName, string oldPassword, string newPassword, string confirmPassword)
        {
            if (Request.HttpMethod == "POST")
            {
                ViewBag.UserName = userName;
                if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
                {
                    ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin! ");
                    return View();
                }

                if(newPassword == oldPassword)
                {
                    ModelState.AddModelError("NewPassword", "Mật khẩu mới đã trùng với mật khẩu cũ!");
                    return View();
                }

                if(newPassword != confirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "Mật khẩu nhập lại không trùng với mật khẩu mới!");
                    return View();
                }
                
                bool check = UserAccountService.ChangePassword(AccountTypes.Employee, userName, oldPassword,newPassword);
                if (!check)
                {
                    ModelState.AddModelError("", "Đổi mật khẩu thất bại!");
                    return View();
                }
                return RedirectToAction("Logout");
            }
            return View();
        }
    }
}

