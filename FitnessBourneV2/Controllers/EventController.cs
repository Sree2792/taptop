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

        //Pending
        public ActionResult EventSave(EventAddModel eventSave)
        {

            if (!ModelState.IsValid)
            {
                return View(eventSave);
            }
            else
            {
                var locationList = Session["anchorSelected"];

                int eventType = 1;

                foreach(var record in db.EventTypes.ToList())
                {
                    if (record.ET_Name == eventSave.eventTypeName)
                    {
                        eventType = record.ET_Id;
                    }
                }

                MemberTable userRecord = (MemberTable)Session["UserLoggedIn"];


                EventTable eventCreated = new EventTable()
                {
                    Evnt_Is_Private = Convert.ToByte(eventSave.isPrivate),
                    Evnt_Capacity = eventSave.mem_capacity,
                    Evnt_Start_DateTime = eventSave.startDateTime,
                    Evnt_End_DateTime = eventSave.endDateTime,
                    EventTypeET_Id = eventType,
                    Admin = userRecord
                };

                db.EventTables.Add(eventCreated);
                db.SaveChanges();

            }

            return RedirectToAction("EventAdd", "Event");
        }
        
        public void setLocation(List<string> anchorName)
        {
            Session["anchorSelected"] = anchorName;

            //Install-Package GoogleMaps.LocationServices -Version 1.2.0.1 
        }


    }
}