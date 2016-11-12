using MonitoringTourSystem.Infrastructures.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MonitoringTourSystem.Infrastructures.Interfaces.Home
{
    public interface IAdmin
    {
        JsonResult AddCountry(country obj);

        JsonResult AddProvince(province obj);

        JsonResult AddPlace(place obj);

        JsonResult AddPlaceForeign(place obj);
    }
}
