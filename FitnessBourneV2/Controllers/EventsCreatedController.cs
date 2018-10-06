using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using FitnessBourneV2.Models;


namespace FitnessBourneV2.Controllers
{
    public class EventsCreatedController : Controller
    {
        private fbmodelContainer db = new fbmodelContainer();

        // GET: EventsCreated
        public ActionResult EventCreated()
        {
            List<EventTable> eventList = db.EventTables.ToList();

            List<EventTable> eventsCreated = new List<EventTable>();

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

            // Get events where login member is the admin
            foreach (EventTable tableObj in eventList)
            {
                //Check admin object
                if(loginUser.Mem_Id == tableObj.MemberTable.Mem_Id && !tableObj.Evnt_IsEdit)
                {

                    eventsCreated.Add(tableObj);
                }
            }

            // list of created events
            List<CreatedEventDetails> eventsToView = new List<CreatedEventDetails>();

            // loop through joined events to make object
            foreach (EventTable subRec in eventsCreated)
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
                foreach (EventType typeObj in db.EventTypes.ToList())
                {
                    if (typeObj.ET_Id == subRec.EventTypeET_Id)
                    {
                        eventTypeStr = typeObj.ET_Name;
                    }
                }


                //Get all members in the created event
                List<MemberTable> memList = new List<MemberTable>();
                foreach (EventMembers memObj in subRec.EventMembers)
                {
                    // Other than admin member
                    if(memObj.MemberTable.Mem_Id != loginUser.Mem_Id)
                    {
                        memList.Add(memObj.MemberTable);
                    }
                    
                }


                //Events created to view
                CreatedEventDetails createdEvent = new CreatedEventDetails()
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
                    EventypeInView = eventTypeStr,
                    EventMembers = memList
                };


                //Pass event
                eventsToView.Add(createdEvent);
            }

            // session store
            Session["CreatedEvents"] = eventsToView;

            // event joined
            EventCreated modeToView = new EventCreated()
            {
                eventList = eventsToView
            };

            // event joined
            if (eventsToView.Count > 0)
            {
                return View(modeToView);
            }
            else
            {
                // Nothing created yet
                Session["AlertMessage"] = "You did not create any events yet!!!";
                return RedirectToAction("Index", "Home");
            }
        }

        [WebMethod]
        public void deleteEvent(Int32 anchorname)
        {
            //get event joined
            EventTable eventDet = db.EventTables.Find(anchorname);

            //delete locations in event
            eventDet.LocationTables.Clear();

            //update object
            // state modified
            db.Entry(eventDet).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            //Delete event members
            List<EventMembers> eveMemList = db.EventMembers.ToList();
            foreach (EventMembers eveMem in eveMemList)
            {
                if(eveMem.EventTable.Evnt_Id == eventDet.Evnt_Id)
                {
                    // delete event members
                    db.EventMembers.Remove(eveMem);
                    db.SaveChanges();
                }
            }

           

            //Delete event
            db.EventTables.Remove(eventDet);
            db.SaveChanges();
        }
    }
}