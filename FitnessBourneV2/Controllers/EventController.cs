using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using FitnessBourneV2.Models;

namespace FitnessBourneV2.Controllers
{
    public class EventController : Controller
    {
        private fbmodelContainer db = new fbmodelContainer();

        // event home refresh
        public ActionResult EventRefresh(EventHomeModel eventObj)
        {
            string typeStr = Session["EventType"].ToString();
            return RedirectToAction("EventHome", "Event", new { type = typeStr });
        }


        // GET: Event view and join
        public ActionResult EventHome(string type)
        {
            List<EventTable> eventList = db.EventTables.ToList();

            List<EventTable> eventsFiltered = new List<EventTable>();

            Session["EventType"] = type;

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

            foreach (EventTable subRecord in eventList)
            {
                //filter if member already joined
                bool memToJoin = true;

                if (subRecord.EventMembers.Count > 0)
                {
                    memToJoin = false;
                }

                //filter on event type , club if private
                if (subRecord.EventType.ET_Name == type && memToJoin)
                {
                    if (Convert.ToBoolean(subRecord.Evnt_Is_Private))
                    {
                        //only same club events
                        if (subRecord.FitnessClub == loginUser.FitnessClub)
                        {
                            eventsFiltered.Add(subRecord);
                        }
                    }
                    else
                    {
                        eventsFiltered.Add(subRecord);
                    }

                }
            }

            if (eventsFiltered.Count > 0)
            {


                EventHomeModel modelToView = new EventHomeModel();
                modelToView.EventypeInView = type.ToUpper();

                List<EventListForType> eventListRefined = new List<EventListForType>();
                //Populate the model for view
                foreach (EventTable subRec in eventsFiltered)
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

                    EventListForType eventToPass = new EventListForType()
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
                        eventID = subRec.Evnt_Id
                    };

                    //Pass event
                    eventListRefined.Add(eventToPass);
                }

                // add refined event list
                modelToView.eventList = eventListRefined;

                return View(modelToView);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }

        }

        // edit an event
        public ActionResult EventEdit(Int32 eventID)
        {
            //edit event
            EventTable eventObj = db.EventTables.Find(eventID);
            Session["EventToEdit"] = eventObj;

            //Event type
            string eventTypeSTR = "";
            foreach(EventType type in db.EventTypes.ToList())
            {
                if(eventObj.EventTypeET_Id == type.ET_Id)
                {
                    eventTypeSTR = type.ET_Name;
                    break;
                }
            }

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

            //Event to edit
            EventAddModel eventAdd = new EventAddModel()
            {
                eventTypeOptions = db.EventTypes.ToList(),
                eventTypeName = eventTypeSTR,
                eventAdmin = loginUser,
                isPrivate = Convert.ToBoolean(eventObj.Evnt_Is_Private),
                isCheckIn = Convert.ToBoolean(eventObj.Evnt_Is_Checkd_In),
                endDateTime = eventObj.Evnt_End_DateTime.Date.ToString("yyyy-MM-dd HH:mm"),
                startDateTime = eventObj.Evnt_Start_DateTime.ToString("yyyy-MM-dd HH:mm"),
                isEditMode = true,
                mem_capacity = eventObj.Evnt_Capacity.ToString(),
                eventID = eventObj.Evnt_Id
            };
            return View(eventAdd);
        }

        //update the event
        public ActionResult EventSaveOnEdit(EventAddModel eventSave)
        {
            //Event table on edit
            EventTable eventObjOnEdit = (EventTable)Session["EventToEdit"];

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

            //add location
            List<String> locationList = (List<String>)Session["locationList"];

            List<LocationTable> localList = new List<LocationTable>();
            foreach (var adrString in locationList)
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

            //maintain location table if empty
            if(localList.Count == 0)
            {
                localList = eventObjOnEdit.LocationTables.ToList();
            }

            // Event type linking
            int eventType = 1;
            foreach (var record in db.EventTypes.ToList())
            {
                if (record.ET_Name == eventSave.eventTypeName)
                {
                    eventType = record.ET_Id;
                }
            }

            //Member registeration
            MemberTable adminRecord = new MemberTable();
            foreach (var record in db.MemberTables.ToList())
            {
                if (record.Mem_Email_Id == User.Identity.Name)
                {
                    adminRecord = record;
                }
            }

            //generate navigation details
            List<String> directionList = (List<String>)Session["directionlist"];

            String navDetails = "";
            foreach (string stringTxt in directionList)
            {
                navDetails = navDetails + stringTxt + ";";
            }

            if(navDetails == "")
            {
                navDetails = eventObjOnEdit.Evnt_NavigDetails;
            }

            if (eventSave.isPrivate)
            {
                //Limited to Fitness Club members
                
                EventTable eventCreated = new EventTable()
                {
                    Evnt_Is_Private = Convert.ToByte(eventSave.isPrivate),
                    Evnt_Capacity = Int32.Parse(eventSave.mem_capacity),
                    Evnt_Start_DateTime = Convert.ToDateTime(eventSave.startDateTime),
                    Evnt_End_DateTime = Convert.ToDateTime(eventSave.endDateTime),
                    EventTypeET_Id = eventType,
                    MemberTable = adminRecord,
                    LocationTables = localList,
                    Evnt_NavigDetails = navDetails,
                    FitnessClub = adminRecord.FitnessClub,
                    EventMembers = eventObjOnEdit.EventMembers
                   
                };

                //db.Entry(eventCreated).State = System.Data.Entity.EntityState.Modified;
                db.EventTables.Add(eventCreated);
                db.SaveChanges();

                // Event just added
                EventTable tableObj = db.EventTables.ToList()[db.EventTables.ToList().Count - 1];

                //Add event to DB
                EventEdit eventToEdit = new EventEdit()
                {
                    EventTable = tableObj,
                    LocationTables = localList,
                    Creator = loginUser,
                    EE_EventIdToEdit = eventObjOnEdit.Evnt_Id,
                    EE_DateTime = DateTime.Now
                };
                db.EventEdits.Add(eventToEdit);
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
                    MemberTable = adminRecord,
                    LocationTables = localList,
                    Evnt_NavigDetails = navDetails,
                    EventMembers = eventObjOnEdit.EventMembers
                };

                //db.Entry(eventCreated).State = System.Data.Entity.EntityState.Modified;
                db.EventTables.Add(eventCreated);
                db.SaveChanges();

                // Event just added
                EventTable tableObj = db.EventTables.ToList()[db.EventTables.ToList().Count - 1];

                //Add event to DB
                EventEdit eventToEdit = new EventEdit()
                {
                    EventTable = tableObj,
                    LocationTables = localList,
                    Creator = loginUser,
                    EE_EventIdToEdit = eventObjOnEdit.Evnt_Id,
                    EE_DateTime = DateTime.Now
                };
                db.EventEdits.Add(eventToEdit);
                db.SaveChanges();
            }
            
            return RedirectToAction("Index", "Home");
        }

            // add an event
        public ActionResult EventAdd()
        {
            EventAddModel eventAdd = new EventAddModel()
            {
                eventTypeOptions = db.EventTypes.ToList()
            };
            return View(eventAdd);
        }

        // Save the new event
        public ActionResult EventSave(EventAddModel eventSave)
        {

            if (!ModelState.IsValid)
            {
                return View(eventSave);
            }
            else
            {
                //add location
                List<String> locationList = (List<String>)Session["locationList"];

                List<LocationTable> localList = new List<LocationTable>();
                foreach (var adrString in locationList)
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
                foreach (var record in db.EventTypes.ToList())
                {
                    if (record.ET_Id == Int32.Parse(eventSave.eventTypeName))
                    {
                        eventType = record.ET_Id;
                    }
                }

                //Member registeration
                MemberTable adminRecord = new MemberTable();
                foreach (var record in db.MemberTables.ToList())
                {
                    if (record.Mem_Email_Id == User.Identity.Name)
                    {
                        adminRecord = record;
                    }
                }

                //generate navigation details
                List<String> directionList = (List<String>)Session["directionlist"];

                String navDetails = "";
                foreach (string stringTxt in directionList)
                {
                    navDetails = navDetails + stringTxt + ";";
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
                        MemberTable = adminRecord,
                        LocationTables = localList,
                        Evnt_NavigDetails = navDetails,
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
                        MemberTable = adminRecord,
                        LocationTables = localList,
                        Evnt_NavigDetails = navDetails,
                        EventMembers = new List<EventMembers>()
                    };

                    db.EventTables.Add(eventCreated);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [WebMethod]
        public void setLocation(List<string> anchorname)
        {
            bool isDirection = false;
            List<string> locationList = new List<string>();
            List<string> directionList = new List<string>();

            foreach (var textStr in anchorname)
            {
                if (textStr == ";;;;")
                {
                    isDirection = true;
                }
                else
                {
                    //Add all other elements other than delimiter
                    //check if location or direction
                    if (isDirection)
                    {
                        //populate direction
                        directionList.Add(textStr);
                    }
                    else
                    {
                        //populate location
                        locationList.Add(textStr);

                    }
                }
            }

            Session["locationList"] = locationList;
            Session["directionlist"] = directionList;
            //Install-Package GoogleMaps.LocationServices -Version 1.2.0.1 
        }


        [WebMethod]
        public JsonResult GetLocation(int counterVal)
        {

            List<EventTable> eventList = db.EventTables.ToList();
            String type = Session["EventType"].ToString();
            List<EventTable> eventsFiltered = new List<EventTable>();

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

            foreach (EventTable subRecord in eventList)
            {
                //filter if member already joined
                bool memToJoin = true;

                if (subRecord.EventMembers.Count > 0)
                {
                    memToJoin = false;
                }

                //filter on event type , club if private
                if (subRecord.EventType.ET_Name == type && memToJoin)
                {
                    if (Convert.ToBoolean(subRecord.Evnt_Is_Private))
                    {
                        //only same club events
                        if (subRecord.FitnessClub == loginUser.FitnessClub)
                        {
                            eventsFiltered.Add(subRecord);
                        }
                    }
                    else
                    {
                        eventsFiltered.Add(subRecord);
                    }

                }
            }

            // Event table in view
            EventTable subRec = eventsFiltered[counterVal];

            List<string> locationString = new List<string>();
            foreach (LocationTable locTble in subRec.LocationTables)
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

        // get location for event to edit
        [WebMethod]
        public JsonResult GetLocationOnEdit(int counterVal)
        {
            // Event table on edit
            EventTable eventTable = db.EventTables.Find(counterVal);

            List<string> locationString = new List<string>();
            foreach (LocationTable locTble in eventTable.LocationTables)
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
        public void joinEvent(Int32 anchorname)
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
            EventMembers membEvent = new EventMembers();
            membEvent.MemberTable = loginUser;
            membEvent.EventTable = eventDet;
            db.EventMembers.Add(membEvent);
            db.SaveChanges();
        }
    }
}