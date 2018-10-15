using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.Services;
using FitnessBourneV2.Models;
using Newtonsoft.Json;

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
                bool notAdmin = true;

                foreach(EventMembers memObj in subRecord.EventMembers.ToList())
                {
                    if(memObj.MemberTable.Mem_Id == loginUser.Mem_Id)
                    {
                        memToJoin = false;
                        break;
                    }
                }

                if(subRecord.MemberTable.Mem_Id == loginUser.Mem_Id)
                {
                    notAdmin = false;
                }

                //filter on event type , club if private
                if (subRecord.EventType.ET_Name == type.ToLower() && memToJoin && notAdmin && !subRecord.Evnt_IsEdit)
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
                // Nothing created yet
                Session["AlertMessage"] = "No events available for you to join!!!";
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

            //Event to edit
            EventAddModel eventAdd = new EventAddModel()
            {
                eventTypeOptions = db.EventTypes.ToList(),
                eventTypeName = eventTypeSTR,
                eventAdmin = eventObj.MemberTable,
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
            EventTable eventObjOrg = db.EventTables.Find(eventObjOnEdit.Evnt_Id);
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

                // create map request
                string requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/json?address={0}&key={1}",
                    Uri.EscapeDataString(adrString.ToString()), "AIzaSyBGU14gIoD2RQILeBNWE7ST4rEu-FJ_gcA");

                // Create request
                WebRequest request = WebRequest.Create(requestUri);
                // Get response for request
                WebResponse response = request.GetResponse();
                // get data from response
                Stream data = response.GetResponseStream();
                // get reader
                StreamReader reader = new StreamReader(data);

                // json-formatted string from maps api
                string responseFromServer = reader.ReadToEnd();
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                MapResponse mapResponseData = JsonConvert.DeserializeObject<MapResponse>(responseFromServer);

                response.Close();

                AddressTable tableAdr = new AddressTable()
                {
                    Adr_Street_Name = "",
                    Adr_Suburb_Name = "",
                    Adr_City_Name = "",
                    Adr_State_Name = "",
                    Adr_Zipcode = "",
                    Adr_House_No = "",
                    Adr_Unit_No = "",
                    Adr_FullAddress = mapResponseData.results[0].formatted_address,
                    Adr_Lat = Convert.ToDouble(mapResponseData.results[0].geometry.location.lat),
                    Adr_Long = Convert.ToDouble(mapResponseData.results[0].geometry.location.lng)

                };

                int counter = 0;
                for (int i = mapResponseData.results[0].address_components.Length; i > 0; i--)
                {
                    if (counter == 0)
                    {
                        tableAdr.Adr_Zipcode = mapResponseData.results[0].address_components[i - 1].long_name.ToString();
                    }
                    else if (counter == 2)
                    {
                        tableAdr.Adr_State_Name = mapResponseData.results[0].address_components[i - 1].long_name.ToString();
                    }
                    else if (counter == 3)
                    {
                        tableAdr.Adr_City_Name = mapResponseData.results[0].address_components[i - 1].long_name.ToString();
                    }
                    else if (counter == 4)
                    {
                        tableAdr.Adr_Suburb_Name = mapResponseData.results[0].address_components[i - 1].long_name.ToString();
                    }
                    else if (counter == 5)
                    {
                        tableAdr.Adr_Street_Name = mapResponseData.results[0].address_components[i - 1].long_name.ToString();
                    }
                    else if (counter == 6)
                    {
                        tableAdr.Adr_House_No = mapResponseData.results[0].address_components[i - 1].long_name.ToString();
                    }
                    counter = counter + 1;
                }

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

                //Notification object
                LocationTable locTble = db.LocationTables.ToList()[db.LocationTables.ToList().Count - 1];
                localList.Add(locTble);
            }

            
            //maintain location table if empty
            if (localList.Count == 0)
            {
                localList = eventObjOnEdit.LocationTables.ToList();
            }

            // Event type linking
            int eventType = 1;
            foreach (var record in db.EventTypes.ToList())
            {
                if (record.ET_Name == eventSave.eventTypeName.ToLower())
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

            if (navDetails == "")
            {
                navDetails = eventObjOnEdit.Evnt_NavigDetails;
            }

            // Check if the event is edited by the admin
            if (loginUser.Mem_Id == eventObjOnEdit.MemberTable.Mem_Id)
            {
                //Admin edited event

                // create notification for participant
                //Notification table set up for event - check for members
                NotificationTable notifTable = new NotificationTable()
                {
                    Notif_Type = "P",
                    Notif_Message = "Confirm event edit for event ID: " + eventObjOnEdit.Evnt_Id.ToString() + " participated by you."
                };

                // save changes to notification added
                db.NotificationTables.Add(notifTable);
                db.SaveChanges();

                //Notification object
                NotificationTable notifForP = db.NotificationTables.ToList()[db.NotificationTables.ToList().Count - 1];

                // paricipant notifications
                foreach (EventMembers eveMemObj in eventObjOnEdit.EventMembers)
                {
                    MemberTable mem = db.MemberTables.Find(eveMemObj.MemberTable.Mem_Id);

                    NotificationActionTable notifAct = new NotificationActionTable()
                    {
                        NA_Decision = "NO",
                        NotificationTableNotif_Id = notifForP.Notif_Id,
                        MemberTable = mem,
                        NotificationTable = notifForP,
                        EventTable = eventObjOrg

                    };

                    db.NotificationActionTables.Add(notifAct);
                    db.SaveChanges();
                }


                // Edit event without suspended events
                //Update event
                EventTable eventTable = db.EventTables.Find(eventObjOnEdit.Evnt_Id);

                eventTable.Evnt_Is_Private = Convert.ToByte(eventSave.isPrivate);
                eventTable.Evnt_Capacity = Int32.Parse(eventSave.mem_capacity);
                eventTable.Evnt_Start_DateTime = Convert.ToDateTime(eventSave.startDateTime);
                eventTable.Evnt_End_DateTime = Convert.ToDateTime(eventSave.endDateTime);
                eventTable.EventTypeET_Id = eventType;
                eventTable.Evnt_NavigDetails = navDetails;

                //Update location
                eventTable.LocationTables.Clear();
                eventTable.LocationTables = localList;


                // state modified
                db.Entry(eventTable).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                //Participant edited event

                // Created notification for  admin
                //Notification table set up for event
                NotificationTable notifTable = new NotificationTable()
                {
                    Notif_Type = "A",
                    Notif_Message = "Confirm event edit for event ID: " + eventObjOnEdit.Evnt_Id.ToString() + " created by you."
                };

                // save changes to notification added
                db.NotificationTables.Add(notifTable);
                db.SaveChanges();

                //Notification object
                NotificationTable notifForA = db.NotificationTables.ToList()[db.NotificationTables.ToList().Count - 1];

                //Event admin
                MemberTable adminForNotif = db.MemberTables.Find(eventObjOnEdit.MemberTable.Mem_Id);

                //Create Notification object for admin
                NotificationActionTable notifAct = new NotificationActionTable()
                {
                    NA_Decision = "NO",
                    NotificationTableNotif_Id = notifForA.Notif_Id,
                    MemberTable = adminForNotif,
                    NotificationTable = notifForA,
                    EventTable = eventObjOrg
                };

                db.NotificationActionTables.Add(notifAct);
                db.SaveChanges();

                // edited events
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
                        Evnt_IsEdit = true
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
                        Creator = loginUser,
                        EE_EventIdToEdit = eventObjOnEdit.Evnt_Id,
                        EE_DateTime = DateTime.Now,
                        NotificationTable = notifForA
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
                        Evnt_IsEdit = true
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
                        Creator = loginUser,
                        EE_EventIdToEdit = eventObjOnEdit.Evnt_Id,
                        EE_DateTime = DateTime.Now,
                        NotificationTable = notifForA
                    };
                    db.EventEdits.Add(eventToEdit);
                    db.SaveChanges();
                }
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
                    
                    // create map request
                    string requestUri = string.Format("https://maps.googleapis.com/maps/api/geocode/json?address={0}&key={1}",
                        Uri.EscapeDataString(adrString.ToString()), "AIzaSyBGU14gIoD2RQILeBNWE7ST4rEu-FJ_gcA");

                    // Create request
                    WebRequest request = WebRequest.Create(requestUri);
                    // Get response for request
                    WebResponse response = request.GetResponse();
                    // get data from response
                    Stream data = response.GetResponseStream();
                    // get reader
                    StreamReader reader = new StreamReader(data);

                    // json-formatted string from maps api
                    string responseFromServer = reader.ReadToEnd();
                    var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                    MapResponse mapResponseData  =  JsonConvert.DeserializeObject<MapResponse>(responseFromServer);

            
                    response.Close();

                    AddressTable tableAdr = new AddressTable()
                    {
                        Adr_Street_Name = "",
                        Adr_Suburb_Name = "",
                        Adr_City_Name = "",
                        Adr_State_Name = "",
                        Adr_Zipcode = "",
                        Adr_House_No = "",
                        Adr_Unit_No = "",
                        Adr_FullAddress = mapResponseData.results[0].formatted_address,
                        Adr_Lat = Convert.ToDouble(mapResponseData.results[0].geometry.location.lat),
                        Adr_Long = Convert.ToDouble(mapResponseData.results[0].geometry.location.lng)

                    };

                    int counter = 0;
                    for(int i = mapResponseData.results[0].address_components.Length; i > 0; i--)
                    {
                        if(counter == 0)
                        {
                            tableAdr.Adr_Zipcode = mapResponseData.results[0].address_components[i - 1].long_name.ToString();
                        }else if(counter == 2)
                        {
                            tableAdr.Adr_State_Name = mapResponseData.results[0].address_components[i - 1].long_name.ToString();
                        }else if (counter == 3)
                        {
                            tableAdr.Adr_City_Name = mapResponseData.results[0].address_components[i - 1].long_name.ToString();
                        }
                        else if (counter == 4)
                        {
                            tableAdr.Adr_Suburb_Name = mapResponseData.results[0].address_components[i - 1].long_name.ToString();
                        }
                        else if (counter == 5)
                        {
                            tableAdr.Adr_Street_Name = mapResponseData.results[0].address_components[i - 1].long_name.ToString();
                        }
                        else if (counter == 6)
                        {
                            tableAdr.Adr_House_No = mapResponseData.results[0].address_components[i - 1].long_name.ToString();
                        }
                        counter = counter + 1;
                    }

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
                        FitnessClub = adminRecord.FitnessClub,
                        Evnt_IsEdit = false
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
                        EventMembers = new List<EventMembers>(),
                        Evnt_IsEdit = false
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
                if (subRecord.EventType.ET_Name == type.ToLower() && memToJoin)
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

            //List<string> locationString = new List<string>();
            //foreach (LocationTable locTble in subRec.LocationTables)
            //{
            //    // Address string
            //    //var addrStr = "";

            //    //if (locTble.AddressTable.Adr_Unit_No != "")
            //    //{
            //    //    addrStr = locTble.AddressTable.Adr_Unit_No + ", ";
            //    //}

            //    //if (locTble.AddressTable.Adr_House_No != "")
            //    //{
            //    //    addrStr = addrStr + locTble.AddressTable.Adr_House_No + ", ";
            //    //}

            //    //addrStr = locTble.AddressTable.Adr_Street_Name + ", " + locTble.AddressTable.Adr_Suburb_Name + ", " + locTble.AddressTable.Adr_City_Name +
            //    //    ", " + locTble.AddressTable.Adr_State_Name + ", " + locTble.AddressTable.Adr_Zipcode + "\n";

            //    // Append address string to list
            //    locationString.Add(locTble.AddressTable.Adr_FullAddress);
            //}

            List<List<double>> latlongList = new List<List<double>>();

            foreach (LocationTable locTble in subRec.LocationTables)
            {
                List<double> latList = new List<double>();
                latList.Add(locTble.AddressTable.Adr_Lat);
                latList.Add(locTble.AddressTable.Adr_Long);

                latlongList.Add(latList);
            }

            return Json(latlongList, JsonRequestBehavior.AllowGet);

        }

        // get location for event to edit
        [WebMethod]
        public JsonResult GetLocationOnEdit(int counterVal)
        {
            // Event table on edit
            EventTable eventTable = db.EventTables.Find(counterVal);

            //List<string> locationString = new List<string>();
            //foreach (LocationTable locTble in eventTable.LocationTables)
            //{
            //    // Address string
            //    //var addrStr = "";

            //    //if (locTble.AddressTable.Adr_Unit_No != "")
            //    //{
            //    //    addrStr = locTble.AddressTable.Adr_Unit_No + ", ";
            //    //}

            //    //if (locTble.AddressTable.Adr_House_No != "")
            //    //{
            //    //    addrStr = addrStr + locTble.AddressTable.Adr_House_No + ", ";
            //    //}

            //    //addrStr = locTble.AddressTable.Adr_Street_Name + ", " + locTble.AddressTable.Adr_Suburb_Name + ", " + locTble.AddressTable.Adr_City_Name +
            //    //    ", " + locTble.AddressTable.Adr_State_Name + ", " + locTble.AddressTable.Adr_Zipcode + "\n";

            //    // Append address string to list
            //    locationString.Add(locTble.AddressTable.Adr_FullAddress);
            //}

            List<List<double>> latlongList = new List<List<double>>();

            foreach (LocationTable locTble in eventTable.LocationTables)
            {
                List<double> latList = new List<double>();
                latList.Add(locTble.AddressTable.Adr_Lat);
                latList.Add(locTble.AddressTable.Adr_Long);

                latlongList.Add(latList);
            }

            return Json(latlongList, JsonRequestBehavior.AllowGet);

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