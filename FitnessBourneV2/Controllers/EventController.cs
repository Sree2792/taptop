using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FitnessBourneV2.Models;

namespace FitnessBourneV2.Controllers
{
    public class EventController : Controller
    {
        private fbmodelContainer db = new fbmodelContainer();
        // GET: Event
        public ActionResult Index()
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
        }


    }
}