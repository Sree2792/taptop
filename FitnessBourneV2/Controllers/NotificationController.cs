using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using FitnessBourneV2.Models;

namespace FitnessBourneV2.Controllers
{
    public class NotificationController : Controller
    {
        private fbmodelContainer db = new fbmodelContainer();

        // GET: Notification
        public ActionResult NotificationCenter()
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

            //Get Notification for user
            //List<NotificationActionTable> listNotifTable = new List<NotificationActionTable>();
            List<NotificationObject> listOfNotif = new List<NotificationObject>();

            foreach(NotificationActionTable actTable in db.NotificationActionTables.ToList())
            {
                // Add all notif for the login user
                if(actTable.MemberTable.Mem_Id == loginUser.Mem_Id)
                {
                    //listNotifTable.Add(actTable);

                    // Notification table
                    NotificationTable notifTable = actTable.NotificationTable;

                    //Setting location feeds
                    List<LocationTable> locList = notifTable.EventEdit.EventTable.LocationTables.ToList();
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

                    //set if admin or participant
                    bool isAdmin = false;
                    string userType = "Event Participant";

                    if(notifTable.Notif_Type == "A")
                    {
                        isAdmin = true;
                        userType = "Event Admin";
                    }

                    //Create Notif object
                    NotificationObject objNotif = new NotificationObject()
                    {
                        notifMessage = notifTable.Notif_Message,
                        eventType = notifTable.EventEdit.EventTable.EventType.ET_Name,
                        startLoc = startLoc,
                        stopLoc = stopLoc,
                        startDT = notifTable.EventEdit.EventTable.Evnt_Start_DateTime.ToString(),
                        stopDT = notifTable.EventEdit.EventTable.Evnt_End_DateTime.ToString(),
                        isEventAdmin = isAdmin,
                        youAre = userType
                    };

                    //add object to list
                    listOfNotif.Add(objNotif);
                }
            }

            // Build notifications for user
            NotificationCenterModel notifModel = new NotificationCenterModel()
            {
                listOfNotif = listOfNotif
            };


            return View(notifModel);
        }
    }
}