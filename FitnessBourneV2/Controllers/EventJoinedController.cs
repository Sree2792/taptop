using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using FitnessBourneV2.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

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
                if (Session["JoinedEvents"] != null)
                {
                    Session["AlertMessage"] = "No more joined events!!!";
                }
                else
                {
                    Session["AlertMessage"] = "You did not join any events yet!!!";
                }
                
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

            
            List<List<double>> latlongList = new List<List<double>>();

            foreach (LocationTable locTble in tableObj.LocationTables)
            {
                List<double> latList = new List<double>();
                latList.Add(locTble.AddressTable.Adr_Lat);
                latList.Add(locTble.AddressTable.Adr_Long);

                latlongList.Add(latList);
            }

            return Json(latlongList, JsonRequestBehavior.AllowGet);
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

            //EventMember record
            EventMembers memObj = new EventMembers();

            //get event member table
            List<EventMembers> eventMembers = db.EventMembers.ToList();

            foreach (EventMembers obj in eventMembers)
            {
                if (obj.MemberTable.Mem_Id == loginUser.Mem_Id && obj.EventTable.Evnt_Id == eventDet.Evnt_Id)
                {
                    memObj = obj;
                    break;
                }
            }

            //check if the event of member is confirmed
            if (memObj.EvMem_IsConfirmed)
            {
                // check if event capacity
                if (Convert.ToInt32(eventDet.Evnt_Capacity) < eventDet.EventMembers.ToList().Count)
                {
                    // capaciy exceeded case
                    foreach (EventMembers obj in eventMembers)
                    {
                        if (obj.EventTable.Evnt_Id == eventDet.Evnt_Id)
                        {
                            // first non confirmed member in list
                            if (!obj.EvMem_IsConfirmed)
                            {
                                // send mail
                                string subject = "Event status changed to Confirmation";
                                string plainTextContent = "Your event from " + eventDet.Evnt_Start_DateTime.ToString() + " to " + eventDet.Evnt_End_DateTime.ToString() + "has been confirmed.";
                                changeStatusTo(subject, plainTextContent, obj.MemberTable.Mem_Email_Id.ToString());

                                //change his status to confirmed after sending mail
                                EventMembers changeStatus = db.EventMembers.Find(obj.EvMem_Id);
                                changeStatus.EvMem_IsConfirmed = true;

                                // update
                                db.Entry(changeStatus).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();

                                break;
                            }
                        }
                    }

                }
            }

            //delete event event member
            db.EventMembers.Remove(memObj);
            db.SaveChanges();
            
        }

        public void changeStatusTo(string subject, string plainTextContent, string emailTo)
        {
            //To install package-: Install-Package SendGrid
            //var apiKey = Environment.GetEnvironmentVariable("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");
            var client = new SendGridClient("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");

            var from = new EmailAddress("noreply@localhost.com", "Admin");

            var to = new EmailAddress(emailTo, "User");

            var htmlContent = "<strong>" + plainTextContent + "</strong>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = client.SendEmailAsync(msg);
        }
    }
}