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
                eventTypeOptions = db.EventTypes.ToList()
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
                //add location
                List<String> locationList = (List<String>)Session["anchorSelected"];

                List<LocationTable> localList = new List<LocationTable>();
                foreach(var adrString in locationList)
                {
                    List<string> adressObj = adrString.Split(',').ToList<string>();

                    AddressTable tableAdr = new AddressTable()
                    {
                        Adr_Street_Name = adressObj[0],
                        Adr_Suburb_Name = adressObj[1] + ", " + adressObj[2],
                        Adr_City_Name = adressObj[3],
                        Adr_State_Name = adressObj[4],
                        Adr_Zipcode = adressObj[5],
                        Adr_House_No = "",
                        Adr_Unit_No = "",
                        Adr_Lat = 0,
                        Adr_Long = 0
                        
                    };

                    //Add address to DB
                    db.AddressTables.Add(tableAdr);
                    db.SaveChanges();

                    LocationTable tableLoc = new LocationTable()
                    {
                        AddressTable = tableAdr,
                        Loc_Ref_Name = adressObj[0]
                    };

                    //Add Location to DB
                    db.LocationTables.Add(tableLoc);
                    db.SaveChanges();

                    localList.Add(tableLoc);
                }

                // Event type linking
                int eventType = 1;
                foreach(var record in db.EventTypes.ToList())
                {
                    if (record.ET_Id == Int32.Parse(eventSave.eventTypeName))
                    {
                        eventType = record.ET_Id;
                    }
                }

                //Member registeration
                MemberTable adminRecord = new MemberTable();
                foreach(var record in db.MemberTables.ToList())
                {
                    if(record.Mem_Email_Id == Session["UserLoggedIn"].ToString())
                    {
                        adminRecord = record;
                    }
                }

                //Check for Fitness club if private
                if (eventSave.isPrivate)
                {
                    //Limited to Fitness Club members
                    //Add event to DB
                    EventTable eventCreated = new EventTable()
                    {
                        Evnt_Is_Private = Convert.ToByte(eventSave.isPrivate),
                        Evnt_Capacity = Int32.Parse(eventSave.mem_capacity),
                        Evnt_Start_DateTime = Convert.ToDateTime(eventSave.startDateTime),
                        Evnt_End_DateTime = Convert.ToDateTime(eventSave.endDateTime),
                        EventTypeET_Id = eventType,
                        Admin = adminRecord,
                        LocationTables = localList,
                        FitnessClub = adminRecord.FitnessClub
                    };

                    db.EventTables.Add(eventCreated);
                    db.SaveChanges();

                }
                else
                {
                    //Add event to DB
                    EventTable eventCreated = new EventTable()
                    {
                        Evnt_Is_Private = Convert.ToByte(eventSave.isPrivate),
                        Evnt_Capacity = Int32.Parse(eventSave.mem_capacity),
                        Evnt_Start_DateTime = Convert.ToDateTime(eventSave.startDateTime),
                        Evnt_End_DateTime = Convert.ToDateTime(eventSave.endDateTime),
                        EventTypeET_Id = eventType,
                        Admin = adminRecord,
                        LocationTables = localList
                    };

                    db.EventTables.Add(eventCreated);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index", "Home");
        }
        
        public void setLocation(List<string> anchorName)
        {
            Session["locationList"] = anchorName[0];
            Session["directionlist"] = anchorName[1];
            //Install-Package GoogleMaps.LocationServices -Version 1.2.0.1 
        }


    }
}