using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using FitnessBourneV2.Models;

namespace FitnessBourneV2.Controllers
{
    public class EventJoinedController : Controller
    {
        private fbmodelContainer db = new fbmodelContainer();

        // GET: EventJoined
        public ActionResult EventJoin()
        {
            List<EventTable> eventList = db.EventTables.ToList();

            List<EventTable> eventsJoined = new List<EventTable>();

            //get login member table
            MemberTable loginUser = new MemberTable();
            foreach (MemberTable record in db.MemberTables.ToList())
            {
                if (record.Mem_Email_Id == User.Identity.Name)
                {
                    loginUser = record;
                    break;
                }
            }

            // Get members who joined the event
            foreach(EventTable tableObj in eventList)
            {
                // Event member list
                List<EventMembers> memObj = tableObj.EventMembers.ToList();

                // event that is created and not part of edit event
                if (!tableObj.Evnt_IsEdit)
                {
                    //get the events joints by the member
                    foreach (EventMembers eveMemObj in memObj)
                    {
                        if (eveMemObj.MemberTable.Mem_Id == loginUser.Mem_Id)
                        {
                            eventsJoined.Add(tableObj);
                            break;
                        }
                    }
                }
                
            }

            // List of events to join
            List<EventJoined> eventJoin = new List<EventJoined>();

            // loop through joined events to make object
            foreach (EventTable subRec in eventsJoined)
            {
                //setting record up
                string navInstr = "";
                string distanceStr = "";

                string[] words = subRec.Evnt_NavigDetails.Split(';');

                int counter = 0;

                //Splitting navigation string to essential components
                foreach (string content in words)
                {
                    if (counter == 0)
                    {
                        distanceStr = content;
                    }
                    else
                    {
                        if (counter == 1)
                        {
                            navInstr = content;
                        }
                        else
                        {
                            navInstr = navInstr + " -> " + content;
                        }

                    }
                    counter = counter + 1;
                }

                //Setting location feeds
                List<LocationTable> locList = subRec.LocationTables.ToList();
                int startId = locList[0].Loc_Id;
                int stopId = 0;
                string startLoc = "";
                string stopLoc = "";
                List<string> checkPointList = new List<string>();
                string checkPoint = "";
                //getting location string
                foreach (LocationTable record in locList)
                {
                    if (startId >= record.Loc_Id)
                    {
                        startLoc = record.Loc_Ref_Name;
                        startId = record.Loc_Id;
                    }
                    if (stopId < record.Loc_Id)
                    {
                        stopLoc = record.Loc_Ref_Name;
                        stopId = record.Loc_Id;
                    }
                    checkPointList.Add(record.Loc_Ref_Name);
                }
                //remove start and stop from check points
                checkPointList.Remove(startLoc);
                checkPointList.Remove(stopLoc);

                //Setting up check point string
                checkPoint = String.Join(" -> ", checkPointList.ToArray());

                //setting up joined members list
                string seatOccupied = "0";
                if (subRec.EventMembers.Count > 0)
                {
                    // seat occupied
                    seatOccupied = subRec.EventMembers.Count.ToString();
                }

                // Event type string set
                string eventTypeStr = "";
                foreach(EventType typeObj in db.EventTypes.ToList())
                {
                    if(typeObj.ET_Id == subRec.EventTypeET_Id)
                    {
                        eventTypeStr = typeObj.ET_Name;
                    }
                }

                // event to pass to view
                EventJoined eventToPass = new EventJoined()
                {
                    eventStartTime = subRec.Evnt_Start_DateTime.ToString(),
                    eventEndTime = subRec.Evnt_End_DateTime.ToString(),
                    totalCapacity = subRec.Evnt_Capacity.ToString(),
                    navInstructions = navInstr,
                    totalDistance = distanceStr,
                    checkPoints = checkPoint,
                    startLoc = startLoc,
                    stopLoc = stopLoc,
                    seatAvailblity = seatOccupied,
                    eventID = subRec.Evnt_Id,
                    EventypeInView = eventTypeStr
                };

                //Pass event
                eventJoin.Add(eventToPass);
            }

            // session store
            Session["JoinedEvents"] = eventJoin;

            // event joined
            EventJoinedModel eveJoinModel = new EventJoinedModel()
            {
                listOfEventJoined = eventJoin
            };

            // event joined
            if(eventJoin.Count > 0)
            {
                return View(eveJoinModel);
            }
            else
            {
                // Nothing created yet
                Session["AlertMessage"] = "You did not join any events yet!!!";
                return RedirectToAction("Index", "Home");
            }
            
        }

        [WebMethod]
        public JsonResult GetLocation(int counterVal)
        {
            //List of events
            List<EventTable> eventList = db.EventTables.ToList();
            
            // Event joined
            List<EventJoined> joinedEventList = (List<EventJoined>)Session["JoinedEvents"];
            EventJoined joinedEvent = joinedEventList[counterVal];

            // Get event table
            EventTable tableObj = db.EventTables.Find(joinedEvent.eventID);

            //Location string list
            List<string> locationString = new List<string>();
            foreach (LocationTable locTble in tableObj.LocationTables)
            {
                // Address string
                var addrStr = "";

                if (locTble.AddressTable.Adr_Unit_No != "")
                {
                    addrStr = locTble.AddressTable.Adr_Unit_No + ", ";
                }

                if (locTble.AddressTable.Adr_House_No != "")
                {
                    addrStr = addrStr + locTble.AddressTable.Adr_House_No + ", ";
                }

                addrStr = locTble.AddressTable.Adr_Street_Name + ", " + locTble.AddressTable.Adr_Suburb_Name + ", " + locTble.AddressTable.Adr_City_Name +
                    ", " + locTble.AddressTable.Adr_State_Name + ", " + locTble.AddressTable.Adr_Zipcode + "\n";

                // Append address string to list
                locationString.Add(addrStr);
            }
            return Json(locationString, JsonRequestBehavior.AllowGet);
        }

        [WebMethod]
        public void deleteEvent(Int32 anchorname)
        {
            //get login member table
            MemberTable loginUser = new MemberTable();
            foreach (MemberTable record in db.MemberTables.ToList())
            {
                if (record.Mem_Email_Id == User.Identity.Name)
                {
                    loginUser = record;
                    break;
                }
            }

            //get event joined
            EventTable eventDet = db.EventTables.Find(anchorname);

            //get event member table
            List<EventMembers> eventMembers = db.EventMembers.ToList();

            //scan through to delete event
            foreach (EventMembers memObj in eventMembers)
            {
                if(memObj.MemberTable.Mem_Id == loginUser.Mem_Id && memObj.EventTable.Evnt_Id == eventDet.Evnt_Id)
                {
                    db.EventMembers.Remove(memObj);
                    db.SaveChanges();
                    break;
                }
            }
        }
    }
}