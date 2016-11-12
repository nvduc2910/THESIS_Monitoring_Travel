using MonitoringTourSystem.Infrastructures.EntityFramework;
using MonitoringTourSystem.Infrastructures.Interfaces.Home;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonitoringTourSystem.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        // GET: Admin

        private readonly IAdmin adminService;
        public AdminController(IAdmin adminService)
        {
            this.adminService = adminService;
        }

        #region For View
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddCountry()
        {
            return View("AddCountry");
        }


        public ActionResult AddProvince()
        {
            return View("AddProvince");
        }


        public ActionResult AddPlace()
        {
            return View("AddPlace");
        }

        public ActionResult AddForeignPlace()
        {
            return View("AddForeignPlace");
        }
        #endregion

        #region Add new country

        [HttpPost]
        public JsonResult AddNewCountry(country obj)
        {
            var countryData = new country();
            countryData.country_id = obj.country_id;
            countryData.country_name = obj.country_name;
            countryData.description = obj.description;
            return adminService.AddCountry(countryData);
        }
        [HttpPost]
        public JsonResult AddNewProvince(province obj)
        {
            var provinceData = new province();
            provinceData.province_id = obj.province_id;
            provinceData.province_name = obj.province_name;
            provinceData.description = obj.description;
            return adminService.AddProvince(provinceData);
        }

        public JsonResult FileUpload(HttpPostedFileBase file)
        {
            string pathImage;
            if (file != null)
            {
                string pic = System.IO.Path.GetFileName(file.FileName);
                string path = System.IO.Path.Combine(
                                       Server.MapPath("~/Content/Images"), pic);

                pathImage = file.FileName;
                file.SaveAs(path);
                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                }
                var result = new { Success = true, PathImage = pathImage };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = new { Success = false, PathImage = "Upload hình thất bại, vui lòng thử lại" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }         
        }

        [HttpPost]
        public JsonResult AddNewPlace(place obj)
        {
            return adminService.AddPlace(obj);
        }
        [HttpPost]
        public JsonResult AddNewPlaceForeign(place obj)
        {
            return adminService.AddPlaceForeign(obj);
        }
        #endregion
    }
}