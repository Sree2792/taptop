using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FitnessBourneV2.Models;

namespace FitnessBourneV2.Controllers
{
    public class EventHomeMapController : Controller
    {
        private fbmodelContainer db = new fbmodelContainer();

        // GET: EventHomeMap
        public ActionResult MapHome(string eventType)
        {

            Session["eventtype"] = eventType;
            EventHomeModel homeModel = new EventHomeModel()
            {
                EventypeInView = eventType.ToUpper()
            };

            return View(homeModel);
        }

        [HttpPost]
        public JsonResult GetMap()
        {
            // map details
            List<List<string>> mapDetail = new List<List<string>>();
            List<string> latDetail = new List<string>();
            List<string> longDetail = new List<string>();
            List<string> eventIdDetail = new List<string>();
            List<string> startDTList = new List<string>();
            List<string> stopDTList = new List<string>();

            string typeStr = Session["eventtype"].ToString();

            Int32 typeId = 1;
            foreach(EventType typeObj in db.EventTypes.ToList())
            {
                if(typeObj.ET_Name == typeStr.ToLower())
                {
                    typeId = typeObj.ET_Id;
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

            foreach (EventTable tble in db.EventTables.ToList())
            {
                bool memToJoin = true;
                bool notAdmin = true;

                foreach (EventMembers memObj in tble.EventMembers.ToList())
                {
                    if (memObj.MemberTable.Mem_Id == loginUser.Mem_Id)
                    {
                        memToJoin = false;
                        break;
                    }
                }

                if (tble.MemberTable.Mem_Id == loginUser.Mem_Id)
                {
                    notAdmin = false;
                }

                if (tble.EventTypeET_Id == typeId && memToJoin && notAdmin && !tble.Evnt_IsEdit)
                {
                    if (Convert.ToBoolean(tble.Evnt_Is_Private))
                    {
                        //only same club events
                        if (tble.FitnessClub == loginUser.FitnessClub)
                        {
                            LocationTable locTble = tble.LocationTables.ToList()[0];
                            latDetail.Add(locTble.AddressTable.Adr_Lat.ToString());
                            longDetail.Add(locTble.AddressTable.Adr_Long.ToString());
                            eventIdDetail.Add(tble.Evnt_Id.ToString());
                            startDTList.Add(tble.Evnt_Start_DateTime.ToString());
                            stopDTList.Add(tble.Evnt_End_DateTime.ToString());
                        }
                    }
                    else
                    {
                        LocationTable locTble = tble.LocationTables.ToList()[0];
                        latDetail.Add(locTble.AddressTable.Adr_Lat.ToString());
                        longDetail.Add(locTble.AddressTable.Adr_Long.ToString());
                        eventIdDetail.Add(tble.Evnt_Id.ToString());
                        startDTList.Add(tble.Evnt_Start_DateTime.ToString());
                        stopDTList.Add(tble.Evnt_End_DateTime.ToString());
                    }
                    
                }
                
            }

            mapDetail.Add(latDetail);
            mapDetail.Add(longDetail);
            mapDetail.Add(eventIdDetail);
            mapDetail.Add(startDTList);
            mapDetail.Add(stopDTList);

            return Json(mapDetail, JsonRequestBehavior.AllowGet);
        }

        // 
        public ActionResult MapEventJoin(string eventId)
        {

            EventTable tableEvent = db.EventTables.Find(Convert.ToInt32(eventId));

            //setting record up
            string navInstr = "";
            string distanceStr = "";

            string[] words = tableEvent.Evnt_NavigDetails.Split(';');

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
            List<LocationTable> locList = tableEvent.LocationTables.ToList();
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
            if (tableEvent.EventMembers.Count > 0)
            {
                // seat occupied
                seatOccupied = tableEvent.EventMembers.Count.ToString();
            }

            EventListForType eventToPass = new EventListForType()
            {
                eventStartTime = tableEvent.Evnt_Start_DateTime.ToString(),
                eventEndTime = tableEvent.Evnt_End_DateTime.ToString(),
                totalCapacity = tableEvent.Evnt_Capacity.ToString(),
                navInstructions = navInstr,
                totalDistance = distanceStr,
                checkPoints = checkPoint,
                startLoc = startLoc,
                stopLoc = stopLoc,
                seatAvailblity = seatOccupied,
                eventID = tableEvent.Evnt_Id
            };

            string typeStr = "";
            foreach(EventType typeObj in db.EventTypes.ToList())
            {
                if(typeObj.ET_Id == tableEvent.EventTypeET_Id)
                {
                    typeStr = typeObj.ET_Name;
                    break;
                }
            }

            List<EventListForType> eveTypeList = new List<EventListForType>();
            eveTypeList.Add(eventToPass);

            EventHomeModel modelObj = new EventHomeModel()
            {
                EventypeInView = typeStr.ToUpper(),
                eventList = eveTypeList
            };


            return View(modelObj);
        }
    }
}