using MonitoringTourSystem.Infrastructures.EntityFramework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MonitoringTourSystem.Controllers.FakeTourguide
{
    public class Tourguide1Controller : Controller
    {
        // GET: Tourguide1
        public readonly monitoring_tour_v3Entities MonitoringTourSystem = new monitoring_tour_v3Entities();
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetLocation()
        {
            var listLocation = (from s in MonitoringTourSystem.tracking_temp
                        where s.tour_id == 32
                        select s).ToList();

            var jsonString = JsonConvert.SerializeObject(listLocation);
            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }
    }
}