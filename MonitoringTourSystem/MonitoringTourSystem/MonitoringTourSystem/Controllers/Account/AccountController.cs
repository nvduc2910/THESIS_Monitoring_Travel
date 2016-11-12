using MonitoringTourSystem.Infrastructures.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MonitoringTourSystem.Controllers.Account
{
    public class AccountController : Controller
    {

        public readonly monitoring_tour_v3Entities MonitoringTourSystem = new monitoring_tour_v3Entities();
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password, string ReturnUrl)
        {

            var accountInfo = MonitoringTourSystem.managers.ToList();
            foreach (var item in accountInfo)
            {
                if (item.email == username && item.password == password)
                {
                    var userId = MonitoringTourSystem.managers.Where(s => s.email == username).ToList().FirstOrDefault().manager_id;
                    //thay vi username mày thay thành user_id
                    FormsAuthentication.SetAuthCookie(userId.ToString(), true);
                    
                    if (Url.IsLocalUrl(ReturnUrl))
                    {

                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                if (item.email == username && item.password != password)
                {
                    ViewBag.Message = "Mật khẩu không đúng!";
                    return View();
                }
            }
            ViewBag.Message = "Tài khoản không tồn tại!";
            return View();
        }

        [HttpPost]
        public ActionResult Register(string fullname, string position, string email, string phone, string password)
        {
            position = "Trưởng Phòng";
            var accountInfo = MonitoringTourSystem.managers.ToList();

            foreach(var item in accountInfo)
            {
                if(item.email == email)
                {
                    ViewBag.Message = "Email đã tồn tại, Vui lòng chọn email khác !";
                    return View();
                }
            }
            // Email valid
            // Check phonenumber is valid
            //if (!IsPhoneNumber(phone))
            //{
            //    ViewBag.Message = "Số điện thoại không hợp lệ !";
            //    return View();
            //}
            // Phonenumber is valid

            var managerUser = new manager()
            {
                email = email,
                access_token = Guid.NewGuid().ToString(),
                display_photo = "avatardefault.png",
                manager_name = fullname,
                password = password,
                phone_number = phone,
                position = position,
                
            };

            try
            {
                using (var context = new monitoring_tour_v3Entities())
                {
                    var managerUserData = context.Set<manager>();
                    managerUserData.Add(managerUser);
                    context.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                ViewBag.Message = "Đã xảy ra lỗi, vui lòng đăng ký lại !";
                return View();
            }

            ViewBag.Message = "Đăng ký thành công, Đăng nhập ngay bây giờ!";
            return View("Login");
        }

        public static bool IsPhoneNumber(string number)
        {
            if (number != null)
            {
                return Regex.Match(number, @"^(\[0-9]{9})$").Success;
            }
            else
            {
                return false;
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}