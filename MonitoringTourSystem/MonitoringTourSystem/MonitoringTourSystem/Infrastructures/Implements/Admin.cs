using MonitoringTourSystem.Infrastructures.Interfaces.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MonitoringTourSystem.Infrastructures.EntityFramework;
using System.Web.Mvc;
using MonitoringTourSystem.Services;

namespace MonitoringTourSystem.Infrastructures.Implements
{
    public class Admin : Controller, IAdmin
    {
        protected DbContext _dbContextPool = new DbContext();
        protected ManagerServices _managerServices = new ManagerServices();
        public JsonResult AddCountry(country obj)
        {
            var countryId = _dbContextPool.GetContext().countries.Where(s => s.country_id == obj.country_id).FirstOrDefault();

            if(countryId == null)
            {

                using (var context = new monitoring_tour_v3Entities())
                {
                    var countryData = context.Set<country>();
                    countryData.Add(obj);
                    context.SaveChanges();
                }
                var result = new { Success = true, Message = "Thêm Thành công" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = new { Success = false, Message = "Mã nước đã tồn tại" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult AddPlace(place obj)
        {
            if (obj.cover_photo != null)
            {
                try
                {
                    using (var context = new monitoring_tour_v3Entities())
                    {
                        var placeData = context.Set<place>();
                        placeData.Add(obj);
                        context.SaveChanges();
                    }
                    var result = new { Success = true, Message = "Thêm Thành công" };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    var result = new { Success = false, Message = "Thêm thất bại" };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var result = new { Success = true, Message = "Vui lòng chọn hình ảnh đại diện" };
                return Json(result, JsonRequestBehavior.AllowGet);

            }
        }

        public JsonResult AddPlaceForeign(place obj)
        {
            if (obj.cover_photo != null)
            {
                try
                {
                    using (var context = new monitoring_tour_v3Entities())
                    {
                        var placeData = context.Set<place>();
                        placeData.Add(obj);
                        context.SaveChanges();
                    }
                    var result = new { Success = true, Message = "Thêm Thành công" };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                catch
                {
                    var result = new { Success = false, Message = "Thêm thất bại" };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var result = new { Success = true, Message = "Vui lòng chọn hình ảnh đại diện" };
                return Json(result, JsonRequestBehavior.AllowGet);

            }
        }

        public JsonResult AddProvince(province obj)
        {
            var provinceId = _dbContextPool.GetContext().provinces.Where(s => s.province_id == obj.province_id).FirstOrDefault();

            if (provinceId == null)
            {
                obj.country_id = 84;
                using (var context = new monitoring_tour_v3Entities())
                {
                    var provinceData = context.Set<province>();
                    provinceData.Add(obj);
                    context.SaveChanges();
                }
                var result = new { Success = true, Message = "Thêm Thành công" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var result = new { Success = true, Message = "Mã nước đã tồn tại" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }
    }
}