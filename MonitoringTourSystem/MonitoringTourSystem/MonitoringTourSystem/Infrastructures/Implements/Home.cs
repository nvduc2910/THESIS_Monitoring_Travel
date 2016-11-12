using MonitoringTourSystem.Infrastructures.Interfaces.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MonitoringTourSystem.ViewModel;
using MonitoringTourSystem.Services;
using MonitoringTourSystem.Models;
using System.Web.Mvc;
using Newtonsoft.Json;
using MonitoringTourSystem.Infrastructures.EntityFramework;
using System.Net;

namespace MonitoringTourSystem.Infrastructures.Implements
{
    public class Home : Controller, IHome 
    {
        protected DbContext _dbContextPool = new DbContext();
        protected ManagerServices _managerServices = new ManagerServices();
        public JsonResult CreateMarkerTourGuide(string userName)
        {
            var jsonString = JsonConvert.SerializeObject(new
            {
                objectArray = GetTourIsProcessing(userName)
            });
            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }

        public List<WarningWithReceiver> GetInfoWarning(string username)
        {
            var userID = _managerServices.GetUserID(username);
            // Get list warning
            var lstWarningWithReceiver = new List<WarningWithReceiver>();

            //Get listwarning is opening
            var listWarning = _dbContextPool.GetContext().warnings.Where(s => s.status == StatusWarning.Opening.ToString()).ToList();

            // Get list receiver warning
            var listReceiverWarning = _dbContextPool.GetContext().warning_receiver.Where(s => s.warner_id == userID).ToList();

            for (int i = 0; i < listWarning.Count; i++)
            {
                var listReceiverOfWarning = listReceiverWarning.Where(s => s.warning_id == listWarning[i].warning_id).ToList();

                if (listReceiverOfWarning.Count != 0)
                {
                    lstWarningWithReceiver.Add(new WarningWithReceiver() { Warning = listWarning[i], ListWarningReceiver = listReceiverOfWarning, QuanityRecevied = listReceiverOfWarning.Where(x => x.status == StatusWarning.Received.ToString()).ToList().Count });
                }
            }


            return lstWarningWithReceiver;
        }

        public List<TourIsProcessing> GetTourForWarningOption(Warning obj, string userName)
        {
            var lstTourGuideForWarning = GetTourIsProcessing(userName);

            var listTourGuideResult = new List<TourIsProcessing>();
            if (obj.Distance != 0)
            {
                

                for (int i = 0; i < lstTourGuideForWarning.Count; i++)
                {
                    var a = GetDistanceFromLatLonInKm(obj.Lat, obj.Long, lstTourGuideForWarning[i].TourGuide.latitude, lstTourGuideForWarning[i].TourGuide.longitude);
                    if (GetDistanceFromLatLonInKm(obj.Lat, obj.Long, lstTourGuideForWarning[i].TourGuide.latitude, lstTourGuideForWarning[i].TourGuide.longitude) < obj.Distance)
                    {
                        listTourGuideResult.Add(lstTourGuideForWarning[i]);
                    }
                }
                return listTourGuideResult;
            }
            else
            {
                return lstTourGuideForWarning;
            }
        }
        double GetDistanceFromLatLonInKm(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371d; // Radius of the earth in km
            var dLat = Deg2Rad(lat2 - lat1);  // deg2rad below
            var dLon = Deg2Rad(lon2 - lon1);
            var a =
              Math.Sin(dLat / 2d) * Math.Sin(dLat / 2d) +
              Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) *
              Math.Sin(dLon / 2d) * Math.Sin(dLon / 2d);
            var c = 2d * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1d - a));
            var d = R * c; // Distance in km
            return d;
        }
        double Deg2Rad(double deg)
        {
            return deg * (Math.PI / 180d);
        }
        public List<TourIsProcessing> GetTourIsProcessing(string username)
        {
            var userID = _managerServices.GetUserID(username);
            var lstTourGuide = _dbContextPool.GetContext().tourguides.ToList();
            var lstTour = _dbContextPool.GetContext().tours.ToList();

            // Join tourguide table and tour table with key is tourguide_id
            var tourManage = (from s in lstTour
                              join r in lstTourGuide on s.tourguide_id equals r.tourguide_id
                              where s.status == StatusTour.Running.ToString() && s.manager_id == userID
                              select new
                              {
                                  TourSelect = s,
                                  TourGuideSelect = r,

                              }).ToList();

            var tourWithTourGuide = new List<TourIsProcessing>();

            for (int i = 0; i < tourManage.Count; i++)
            {
                tourWithTourGuide.Add(new TourIsProcessing() { Tour = tourManage[i].TourSelect, TourGuide = tourManage[i].TourGuideSelect });
            }
            return tourWithTourGuide;
        }

        
        public List<TourIsProcessing> SearchTourGuide(string username, string id)
        {
            List<TourIsProcessing> listSearch;
            if (id != null && id.Length > 3)
            {
                System.Threading.Thread.Sleep(1000);
                listSearch = GetTourIsProcessing(username);
                id = id.ToUpper();
                var listSearchResult = (listSearch.Where(x => x.Tour.tour_code.Contains(id))).ToList();
                return listSearchResult;
            }
            else
            {
                return listSearch = GetTourIsProcessing(username);
            }
        }

        public JsonResult SelectMarkerTourGuide(int id)
        {
            var itemResult = _dbContextPool.GetContext().tourguides.Where(item => item.tourguide_id == id);
            var jsonString = JsonConvert.SerializeObject(itemResult);
            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SendWarningGroup(Warning obj, string userName)
        {
            try
            {
                var userId = _managerServices.GetUserID(userName);
                int warningId;
                if (obj != null && obj.ListTourGuideId.Count != 0)
                {
                    using (var context = _dbContextPool.GetContext())
                    {
                        var warningModel = new warning()
                        {
                            warning_name = obj.WarningName,
                            description = obj.DescriptionWarning,
                            latitude = obj.Lat,
                            longitude = obj.Long,
                            status = StatusWarning.Opening.ToString(),
                            type = obj.CategoryWarnig,
                            distance = obj.Distance
                        };
                        var warningData = context.Set<warning>();
                        warningData.Add(warningModel);
                        context.SaveChanges();
                    }

                    warningId = _dbContextPool.GetContext().warnings.Max(x => x.warning_id);

                    for (int i = 0; i < obj.ListTourGuideId.Count; i++)
                    {
                        var warningReceiver = new warning_receiver()
                        {
                            warning_id = warningId,
                            receiver_id = Convert.ToInt32(obj.ListTourGuideId[i]),
                            status = StatusWarning.Opening.ToString(),
                            warner_id = userId
                        };

                        using (var context = new monitoring_tour_v3Entities())
                        {
                            var warningReceviceData = context.Set<warning_receiver>();
                            warningReceviceData.Add(warningReceiver);
                            context.SaveChanges();
                        }
                    }
                    var result = new { Success = true, Message = "Gửi cảnh báo thành công!" };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var result = new { Success = false, Message = "Danh sách tour nhận cảnh báo rỗng!" };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var result = new { Success = false, Message = "Send failed" };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CreateWarningMarker(string userName)
        {

            var userId = _managerServices.GetUserID(userName);

            var listWarningReceiver = _dbContextPool.GetContext().warning_receiver.Where(s => s.warner_id == userId).GroupBy(x => x.warning_id).Select(y => y.FirstOrDefault()).ToList();

            var listWarningAll = _dbContextPool.GetContext().warnings.ToList();

            var listWarningOfUser = new List<warning>();
            for (int i = 0; i < listWarningReceiver.Count; i++)
            {
                var warningItem = listWarningAll.Where(x => x.warning_id == listWarningReceiver[i].warning_id).FirstOrDefault();
                if (warningItem != null)
                {
                    listWarningOfUser.Add(warningItem);
                }

            }

            //
            var jsonString = JsonConvert.SerializeObject(listWarningOfUser);
            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SelectMarkerWarning(int id)
        {
            var itemResult = _dbContextPool.GetContext().warnings.Where(item => item.warning_id == id);
            var jsonString = JsonConvert.SerializeObject(itemResult);
            return Json(jsonString, JsonRequestBehavior.AllowGet);
        }
    }
}