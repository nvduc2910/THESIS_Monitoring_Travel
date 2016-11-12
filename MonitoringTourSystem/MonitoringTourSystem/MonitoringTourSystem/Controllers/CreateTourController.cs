using MonitoringTourSystem.Infrastructures.EntityFramework;
using MonitoringTourSystem.Models;
using MonitoringTourSystem.Services;
using MonitoringTourSystem.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MonitoringTourSystem.Controllers
{
    [Authorize]
    public class CreateTourController : Controller
    {
        
        static string pathImage;
        public readonly monitoring_tour_v3Entities MonitoringTourSystem = new monitoring_tour_v3Entities();
        private static bool isForeignTour = false;

        // GET: CreateTour
        public ActionResult TourVietNam()
        {
            isForeignTour = false;
            GetPlaceForTourSchedule();
            return View("TourVietNam");
        }

        public ActionResult TourForeign()
        {
            isForeignTour = true;
            GetPlaceForTourSchedule();
            return View("TourForeign");
        }

        public ActionResult CreateTourChoose()
        {
            return View("CreateTourChoose");
        }

        public ActionResult GetPlaceForTourSchedule()
        {
            var listPlaceTour = MonitoringTourSystem.places.ToList();
            var listTourGuide = MonitoringTourSystem.tourguides.ToList();
            var model = new CreateTourViewModel() { ListPlace = listPlaceTour, ListTourGuide = listTourGuide};
            return PartialView("Index", model);
        }

        [HttpGet]
        public JsonResult GetListPlace(int id)
        {
            var listPlaceTour = from place in MonitoringTourSystem.places
                                where place.province_id == id.ToString()
                                select place;

   
            var jsonString = JsonConvert.SerializeObject(listPlaceTour);
            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetListTourGuide()
        {
            //var listTourGuide = MonitoringTourSystem.tourguides.ToList();
            //var jsonString = JsonConvert.SerializeObject(listTourGuide);
            //return Json(jsonString, JsonRequestBehavior.AllowGet);
            return null;
        }

        [HttpGet]
        public JsonResult GetListPlaceForCountry(int id)
        {
            var listPlaceTour = from place in MonitoringTourSystem.places
                                where place.country_id == id
                                select place;


            var jsonString = JsonConvert.SerializeObject(listPlaceTour);
            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProvince()
        {
            var listProvince = MonitoringTourSystem.provinces.ToList();
            var jsonString = JsonConvert.SerializeObject(listProvince);
            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }

        //Get list country
        [HttpGet]
        public JsonResult GetCountry()
        {
            var listCountry = MonitoringTourSystem.countries.ToList();
            listCountry.Remove(listCountry.Where(s => s.country_id == 84).FirstOrDefault());
            var jsonString = JsonConvert.SerializeObject(listCountry);
            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AddNewTour(tour obj)
        {
            ManagerServices _managerServices = new ManagerServices();
            string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;

            var managerId = _managerServices.GetUserID(username);

            int foreignTour = 0;

            if(isForeignTour == false)
            {
                foreignTour = 0;
            }
            else
            {
                foreignTour = 1;
            }

            if (pathImage == null)
            {
                var result = new { Success = true, Message = "Dang Upload hinh anh" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var statusTour = StatusTour.Opening;
                try
                {
                    var tourModel = new tour()
                    {
                        tourguide_id = obj.tourguide_id,
                        tour_code = obj.tour_code,
                        manager_id = managerId,
                        tour_name = obj.tour_name,
                        departure_date = obj.departure_date,
                        return_date = obj.return_date,
                        tourist_quantity = obj.tourist_quantity,
                        status = statusTour.ToString(),
                        description = "N'" + obj.description + "'",
                        day = obj.day,
                        cover_photo = pathImage,
                        is_foreign_tour = foreignTour,
                    };

                    using (var context = new monitoring_tour_v3Entities())
                    {
                        var tourModelData = context.Set<tour>();
                        tourModelData.Add(tourModel);
                        context.SaveChanges();
                    }

                    var tourID = MonitoringTourSystem.tours.Max(x => x.tour_id);
                    for (int i = 0; i < obj.ListTourSchedule.Count; i++)
                    {
                        obj.ListTourSchedule[i].tour_id = tourID + 1;
                    }
                    if (AddNewTour(obj.ListTourSchedule))
                    {
                        Response.StatusCode = (int)HttpStatusCode.OK;
                        var result = new { Success = true, Message = "Add Successful" };
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        var result = new { Success = false, Message = "Add Fail" };
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    var result = new { Success = false, Message = ex.Message };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
        }

        // Add data to database
        public bool AddNewTour(List<tour_schedule> tourScheduleModel)
        {
            try
            {
                // Insert database to tour table
                using (var context = new monitoring_tour_v3Entities())
                {
                    foreach(var item in tourScheduleModel)
                    {
                        var tourScheduleData = context.Set<tour_schedule>();
                        tourScheduleData.Add(item);
                        context.SaveChanges();
                    }
                    context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public ActionResult FileUpload(HttpPostedFileBase file)
        {
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
        public JsonResult GetTourGuideAvailable(DateTime departuredate, DateTime returndate)
        {


            var lstTourGuideAvaible = (from s in MonitoringTourSystem.tours
                                         where (returndate > s.return_date && departuredate > s.return_date) || (departuredate < s.departure_date && returndate < s.departure_date)
                                       select s).ToList();


            var lstAllTourGuideIsProcessing = MonitoringTourSystem.tours.ToList();

            for (int i = 0; i < lstTourGuideAvaible.Count; i++)
            {
                var item = lstAllTourGuideIsProcessing.Where(s => s.tourguide_id == lstTourGuideAvaible[i].tourguide_id).FirstOrDefault();

                if (item != null)
                {
                    lstAllTourGuideIsProcessing.Remove(item);
                }
            }
            // After remove lstAllTourGuideIsProcessing is lstTourguideNotAvailble

            var lstTourGuide = MonitoringTourSystem.tourguides.ToList();

            for(int i = 0; i < lstAllTourGuideIsProcessing.Count; i++)
            {
                var item = lstTourGuide.Where(s => s.tourguide_id == lstAllTourGuideIsProcessing[i].tourguide_id).FirstOrDefault();
                if (item != null)
                {
                    lstTourGuide.Remove(item);
                }
            }
            var jsonString = JsonConvert.SerializeObject(lstTourGuide);
            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }
    }
}