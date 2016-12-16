using MonitoringTourSystem.Models;
using MonitoringTourSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MonitoringTourSystem.Infrastructures.Interfaces.Home
{
    public interface IHome 
    {
        List<TourIsProcessing> GetTourIsProcessing(string username);
        List<WarningWithReceiver> GetInfoWarning(string username);
        JsonResult CreateMarkerTourGuide(string userName);
        JsonResult SelectMarkerTourGuide(int id);
        List<TourIsProcessing> SearchTourGuide(string username, string id);
        List<TourIsProcessing> GetTourForWarningOption(Warning obj, string userName);
        JsonResult SendWarningGroup(Warning obj, string userName);
        JsonResult CreateWarningMarker(string userName);
        JsonResult SelectMarkerWarning(int id);
        JsonResult CreateMarkerPlace(string userName);
        JsonResult GetPointLocation(int tourguideId);
        List<TourIsProcessing> SearchTourByRegion(string username, int id);

        JsonResult GetTourGuideInfo(int tourguideId);

        List<HelpViewModel> GetListHelp(string username);
    }
}
