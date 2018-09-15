using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FitnessBourneV2.Models;
using GoogleMaps.LocationServices;

namespace FitnessBourneV2.Controllers
{
    public class EventController : Controller
    {
        private fbmodelContainer db = new fbmodelContainer();
        // GET: Event
        public ActionResult EventHome()
        {
            return View();
        }

        public ActionResult EventAdd()
        {
            EventAddModel eventAdd = new EventAddModel()
            {
                eventType = db.EventTypes.ToList(),
                clubList = db.FitnessClubs.ToList()
                
            };
            return View(eventAdd);
        }

        public ActionResult EventSave(EventAddModel eventSave)
        {
            
            return RedirectToAction("EventAdd", "Event");
        }
        
        public void setLocation(List<string> anchorName)
        {
            Session["anchorSelected"] = anchorName;

            //Install-Package GoogleMaps.LocationServices -Version 1.2.0.1 
            var address = anchorName[0];

            var locationService = new GoogleLocationService();
            var point = locationService.GetLatLongFromAddress(address);

            var latitude = point.Latitude;
            var longitude = point.Longitude;
        }


    }
}