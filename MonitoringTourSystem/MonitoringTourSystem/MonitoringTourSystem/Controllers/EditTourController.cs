using MonitoringTourSystem.Infrastructures.EntityFramework;
using MonitoringTourSystem.Services;
using MonitoringTourSystem.ViewModel;
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
    public class EditTourController : Controller
    {
        static string pathImage;
        EditTourViewModel Model = new EditTourViewModel();
        public readonly monitoring_tour_v3Entities MonitoringTourSystem = new monitoring_tour_v3Entities();
        private static int idInt;
        protected ManagerServices _managerServices = new ManagerServices();
        // GET: EditTour
        public ActionResult EditTour(string id)
        {
            string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            var userId = _managerServices.GetUserID(username);

            idInt = Convert.ToInt32(id);
            var listTour = MonitoringTourSystem.tours.ToList();

            int indexDay = 0;
            int indexStart = 0;
            List<ScheduleDay> ListScheduleDay = new List<ScheduleDay>();
            var listSchedule = (from schedule in MonitoringTourSystem.tour_schedule
                                where schedule.tour_id == idInt
                                select schedule).ToList();

            for (int i = 0; i < listSchedule.Count; i++)
            {
                var a = (listSchedule[i].time - listSchedule[indexStart].time).TotalHours;
                if ((listSchedule[i].time - listSchedule[indexStart].time).TotalHours >= 24)
                {
                    var tourSchedule = new List<tour_schedule>();
                    for (int j = indexStart; j < i; j++)
                    {
                        tourSchedule.Add(listSchedule[j]);

                    }
                    ListScheduleDay.Add(new ScheduleDay() { TourSchedule = tourSchedule });
                    indexStart = i;
                    i = i - 1;
                }
            }
            var tourScheduleItem = new List<tour_schedule>();
            for (int j = indexStart; j < listSchedule.Count; j++)
            {
                tourScheduleItem.Add(listSchedule[j]);
            }
            ListScheduleDay.Add(new ScheduleDay() { TourSchedule = tourScheduleItem });

            for (int k = 0; k < ListScheduleDay.Count; k++)
            {
                indexDay = indexDay + 1;
                ListScheduleDay[k].NumberDay = "NGÀY " + indexDay;
            }

            //Get ID Tour Guide of tour
            var idTourGuide = listTour.Where(x => x.tour_id == idInt).First().tourguide_id;

            var tourGuideName = (from tourGuide in MonitoringTourSystem.tourguides
                                 where tourGuide.tourguide_id == idTourGuide
                                 select tourGuide).ToList();
            var touItem = listTour.Where(x => x.tour_id == idInt).First();

            if (touItem.is_foreign_tour == 0)
            {

                var model = new TourDetailViewModel() { TourItem = touItem, ListScheduleDay = ListScheduleDay, TourGuideName = tourGuideName[0].tourguide_name };

                var tourDetail = model;
                var listProvice = MonitoringTourSystem.provinces.ToList();
                var listTourGuide = MonitoringTourSystem.tourguides.ToList();

                var listPlace = MonitoringTourSystem.places.ToList();

                var listProvinceSelect = new List<string>();

                foreach (var item in tourDetail.ListScheduleDay)
                {
                    foreach (var item1 in item.TourSchedule)
                    {
                        var place_id_int = Convert.ToInt32(item1.place_id);

                        var provinceID = listPlace.Where(y => y.place_id == place_id_int).ToList();

                        var provinceItem = listProvice.Where(x => x.province_id == provinceID[0].province_id).ToList();

                        listProvinceSelect.Add(provinceItem[0].province_id);
                    }
                }


                for (int i = 0; i < listProvinceSelect.Count; i++)
                {
                    var listPlaceOfProvince = listPlace.Where(x => x.province_id == listProvinceSelect[i]).ToList();
                    var provinceName = listProvice.Where(x => x.province_id == listProvinceSelect[i]).ToList();
                    Model.ListProvienWithPlace.Add(new ListProvienWithPlace() { ProvinceName = provinceName[0].province_name, ListPlaceWithProvince = listPlaceOfProvince });
                }

                var listTourSchedule = (from item in MonitoringTourSystem.tour_schedule
                                        where item.tour_id == idInt
                                        select item).ToList();


                Model.TourDetail = tourDetail;
                Model.ListTourGuide = listTourGuide;
                Model.ListTourSchedule = listTourSchedule;

                return View("EditTour", Model);
            }
            else
            {
                var model = new TourDetailViewModel() { TourItem = touItem, ListScheduleDay = ListScheduleDay, TourGuideName = tourGuideName[0].tourguide_name };

                var tourDetail = model;
                var listCountry = MonitoringTourSystem.countries.ToList();
                var listTourGuide = MonitoringTourSystem.tourguides.ToList();

                var listPlace = MonitoringTourSystem.places.ToList();

                var listCountrySelect = new List<int>();

                foreach (var item in tourDetail.ListScheduleDay)
                {
                    foreach (var item1 in item.TourSchedule)
                    {
                        var place_id_int = Convert.ToInt32(item1.place_id);

                        var countryID = listPlace.Where(y => y.place_id == place_id_int).ToList();

                        var provinceItem = listCountry.Where(x => x.country_id == countryID[0].country_id).ToList();

                        listCountrySelect.Add(provinceItem[0].country_id);
                    }
                }


                for (int i = 0; i < listCountrySelect.Count; i++)
                {
                    var listPlaceOfProvince = listPlace.Where(x => x.country_id == listCountrySelect[i]).ToList();
                    var countryName = listCountry.Where(x => x.country_id == listCountrySelect[i]).ToList();
                    Model.ListProvienWithPlace.Add(new ListProvienWithPlace() { ProvinceName = countryName[0].country_name, ListPlaceWithProvince = listPlaceOfProvince });
                }

                var listTourSchedule = (from item in MonitoringTourSystem.tour_schedule
                                        where item.tour_id == idInt
                                        select item).ToList();


                Model.TourDetail = tourDetail;
                Model.ListTourGuide = listTourGuide;
                Model.ListTourSchedule = listTourSchedule;

                return View("EditTourForeign", Model);
            }
        }

        [HttpPost]
        public JsonResult UpdateTour(tour obj)
        {
            string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            var userId = _managerServices.GetUserID(username);
            string pathCoverPhoto = null;
            int isForeignTour = 0;
            try
            {          
                var tourSelect = MonitoringTourSystem.tours.Where(x => x.tour_id == idInt).FirstOrDefault();
                if (pathImage == null)
                {
                    pathCoverPhoto = tourSelect.cover_photo;
                    isForeignTour = tourSelect.is_foreign_tour;
                }

                tourSelect.tourguide_id = obj.tourguide_id;
                tourSelect.tour_code = obj.tour_code;
                tourSelect.manager_id = userId;
                tourSelect.tour_name = obj.tour_name;
                tourSelect.departure_date = obj.departure_date;
                tourSelect.return_date = obj.return_date;
                tourSelect.tourist_quantity = obj.tourist_quantity;
                tourSelect.description = obj.description;
                tourSelect.day = obj.day;
                tourSelect.status = tourSelect.status;
                tourSelect.cover_photo = pathCoverPhoto;
                tourSelect.is_foreign_tour = isForeignTour;

                using (var context = new monitoring_tour_v3Entities())
                {
                    context.Entry(tourSelect).State = System.Data.Entity.EntityState.Modified;
                    var listOldSchedule = context.tour_schedule.Where(x => x.tour_id == idInt).ToList();
                    foreach (var item in listOldSchedule)
                    {
                        context.tour_schedule.Remove(item);
                    }
                    foreach (var item in obj.ListTourSchedule)
                    {
                        item.tour_id = idInt;
                        var tourScheduleData = context.Set<tour_schedule>();
                        tourScheduleData.Add(item);
                    }
                    context.SaveChanges();
                }
                Response.StatusCode = (int)HttpStatusCode.OK;
                var result = new { Success = true, Message = "Add Successful" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var result = new { Success = false, Message = ex.Message };
                return Json(result, JsonRequestBehavior.AllowGet);
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
            }
            return null;
        }
    }
}