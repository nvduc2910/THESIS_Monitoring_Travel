using MonitoringTourSystem.Infrastructures;
using MonitoringTourSystem.Infrastructures.EntityFramework;
using MonitoringTourSystem.Infrastructures.Interfaces.Home;
using MonitoringTourSystem.Models;
using MonitoringTourSystem.ViewModel;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace MonitoringTourSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IHome homeControllerService;

        public List<string> A = new List<string>();
        public HomeController(IHome homeControllerService)
        {
            this.homeControllerService = homeControllerService;
        }

        public monitoring_tour_v3Entities moni = new monitoring_tour_v3Entities();
        public static List<tourguide> ListTourTestRealtime = new List<tourguide>();

        public ActionResult Index()
        {
            ListTourTestRealtime = moni.tourguides.ToList();
            string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            var lstTourIsProcess = homeControllerService.GetTourIsProcessing(username);
            var model = new HomeViewModel() { OptionRenderView = 1, ListTourIsProcessing = lstTourIsProcess, ListWarningWithReceiver = homeControllerService.GetInfoWarning(username), NumberOfTourProcessing = lstTourIsProcess.Count() };
            return View("Index", model);
        }

        public ActionResult Stock()
        {
            return View("Stock");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
        static float longFake = 0.001f;
        static float lagFake = 0.001f;

        #region Get location and fake location

        [HttpGet]
        public JsonResult GetLocationTourGuide()
        {

            //ListTourGuide = MonitoringTourSystem.tourguides.Where(s => s).ToList();
            for (int i = 0; i < ListTourTestRealtime.Count; i++)
            {
                if (i % 2 == 0)
                {
                    ListTourTestRealtime[i].latitude = ListTourTestRealtime[i].latitude + lagFake;
                    ListTourTestRealtime[i].longitude = ListTourTestRealtime[i].longitude + lagFake;

                }
                else
                {
                    ListTourTestRealtime[i].latitude = ListTourTestRealtime[i].latitude - lagFake;
                    ListTourTestRealtime[i].longitude = ListTourTestRealtime[i].longitude - lagFake;
                }
            }

            longFake = longFake + 0.001f;
            lagFake = lagFake + 0.001f;

            var jsonString = JsonConvert.SerializeObject(ListTourTestRealtime);
            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Create Marker for tour is running

        [HttpGet]
        public JsonResult CreateMarker()
        {
            string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            return homeControllerService.CreateMarkerTourGuide(username);
        }

        #endregion


        #region Select marker tourguide
        [HttpGet]
        public JsonResult SelectMarkerTourGuide(int id)
        {
            return homeControllerService.SelectMarkerTourGuide(id);
        }
        #endregion

        #region Search tour guide
        public ActionResult SearchTourGuide(string id)
        {
            string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            var resull = homeControllerService.SearchTourGuide(username, id);
            var model = new HomeViewModel() { ListTourIsProcessing = resull };
            return PartialView("ListTourGuide", model);

        }

        #endregion

        public ActionResult RenderHomeOption(int id)
        {
            //var model = new HomeViewModel() { ListLocationTourGuide = null, OptionRenderView = id };
            //if (model.OptionRenderView == 1)
            //{
            //    return PartialView("Map", model);

            //}
            //if (model.OptionRenderView == 2)
            //{
            //    return PartialView("Message", model);
            //}
            return null;
        }
        #region Caculator distance and warning

        [HttpPost]
        public ActionResult GetListForWarning( Warning obj )
        {
            string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            var lstTourWarningResult = homeControllerService.GetTourForWarningOption(obj, username);
            var model = new HomeViewModel() { OptionRenderView = 1, ListTourIsProcessing = lstTourWarningResult };


            return View("ListTourGuideWarning", model);
        }

        [HttpPost]
        public JsonResult GetListForWarningJson(Warning obj)
        {
            string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            var lstTourWarningResult = homeControllerService.GetTourForWarningOption(obj, username);

            var jsonString = JsonConvert.SerializeObject(new
            {
                objectArray = lstTourWarningResult
            });
            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult GetListWarningRefresh()
        {
            string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            var listWarningRefresh = homeControllerService.GetInfoWarning(username);

            var model = new HomeViewModel() { OptionRenderView = 1, ListWarningWithReceiver = listWarningRefresh };
            return View("ListWarning", model);
        }

        
        //send waring 
        public JsonResult SendWarning(Warning obj)
        {
            string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            return homeControllerService.SendWarningGroup(obj, username);
        }

        [HttpGet]
        public JsonResult CreateMarkerWarning()
        {
            string username = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value).Name;
            return homeControllerService.CreateWarningMarker(username);
        }

        //[HttpGet]
        //public void GetListWarning()
        //{

        //    var ListWarningWithReceiver = new List<WarningWithReceiver>();
        
        //    //Get listwarning is opening
        //    var listWarning = MonitoringTourSystem.warnings.Where(s => s.status == StatusWarning.Opening.ToString()).ToList();
        //    // Get list receiver warning
        //    var listReceiverWarning = MonitoringTourSystem.warning_receiver.ToList();

        //    for (int i = 0; i < listWarning.Count; i++)
        //    {
        //        var listReceiverOfWarning = listReceiverWarning.Where(s => s.warning_id == listWarning[i].warning_id).ToList();
        //        ListWarningWithReceiver.Add(new WarningWithReceiver() { Warning = listWarning[i], ListWarningReceiver = listReceiverOfWarning });
        //    }

        //}

        [HttpGet]
        public JsonResult GetMarkerWarningSelected(int id)
        {
            return homeControllerService.SelectMarkerWarning(id);
        }

        [HttpPost]
        public JsonResult SendWraningForUser(string id, Warning obj)
        {
            return null;
        }

        #endregion
    }
}