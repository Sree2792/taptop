﻿using System;
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
                        youAre = userType,
                        notifTbleId = notifTable.Notif_Id
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

            // event joined
            if (listOfNotif.Count > 0)
            {
                Session["NotifList"] = notifModel;
                return View(notifModel);
            }
            else
            {
                // Nothing created yet
                Session["AlertMessage"] = "You do not have any notification to approve!!!";
                return RedirectToAction("Index", "Home");
            }
            
        }

        public ActionResult notifResult(NotificationCenterModel notifModel)
        {
            // get selected notificatio
            NotificationObject objNotif = (NotificationObject)Session["SelectedNotification"];

            //Confirm the edit of event and raise notification for all participants
            NotificationTable notifObj = db.NotificationTables.Find(objNotif.notifTbleId);


            // Event edit
            EventEdit editObj = notifObj.EventEdit;

            // Event table to update
            EventTable eventTble = db.EventTables.Find(editObj.EE_EventIdToEdit);

            //edited event
            EventTable editedEventTble = editObj.EventTable;

            if (Session["NotificationStatus"].ToString() == "confirm")
            {
                //Create new notifications for participant of events
                if(eventTble.EventMembers.Count > 0)
                {
                    //there are members

                    // get location details
                    //add location
                    List<String> locationList = (List<String>)Session["locationList"];

                    List<LocationTable> localList = new List<LocationTable>();
                    //Setting location feeds
                    List<LocationTable> locList = eventTble.LocationTables.ToList();
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

                    // Event type linking
                    string eventType = "";
                    foreach (var record in db.EventTypes.ToList())
                    {
                        if (record.ET_Id == eventTble.EventTypeET_Id)
                        {
                            eventType = record.ET_Name;
                        }
                    }

                    // Participant notification
                    NotificationTable notifTble = new NotificationTable()
                    {
                        Notif_Type = "p",
                        Notif_Message = "The " + eventType + " event you registered on " + eventTble.Evnt_Start_DateTime.ToString() + " from " + startLoc + " to " + stopLoc
                    };

                    db.NotificationTables.Add(notifTble);
                    db.SaveChanges();

                    foreach(EventMembers eveMem in eventTble.EventMembers)
                    {
                        // loop through members of event
                        // Notification table for notification
                        NotificationActionTable actTble = new NotificationActionTable()
                        {
                            NA_Decision = "NO",
                            MemberTable = eveMem.MemberTable,
                            NotificationTable = notifTble
                        };

                        db.NotificationActionTables.Add(actTble);
                        db.SaveChanges();
                    }

                    
                }
                
                // edit event
                editedEventTble.Evnt_Id = eventTble.Evnt_Id;

                //confirm edit
                db.Entry(editedEventTble).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                //event edited delete
                db.EventTables.Remove(editObj.EventTable);

                //Eventedit delete
                db.EventEdits.Remove(editObj);


                //Notification delete
                db.NotificationTables.Remove(notifObj);

                //Delete Notification tables
                foreach (NotificationActionTable tble in db.NotificationActionTables.ToList())
                {
                    if (tble.NotificationTable.Notif_Id == notifObj.Notif_Id)
                    {
                        db.NotificationActionTables.Remove(tble);
                        db.SaveChanges();
                    }
                }
            }
            else if (Session["NotificationStatus"].ToString() == "delete")
            {
                //Erase or delete the notification
                //Notification delete
                db.NotificationTables.Remove(notifObj);

                //Delete Notification tables for the member as participant
                foreach (NotificationActionTable tble in db.NotificationActionTables.ToList())
                {
                    if (tble.NotificationTable.Notif_Id == notifObj.Notif_Id && tble.MemberTable.Mem_Email_Id == User.Identity.Name)
                    {
                        db.NotificationActionTables.Remove(tble);
                        db.SaveChanges();
                        break;
                    }
                }
            }
            return RedirectToAction("NotificationCenter", "Notification");
        }

        [WebMethod]
        public void editNotification(string anchorname)
        {
            string[] words = anchorname.Split(';');

            Session["NotificationStatus"] = words[0];

            NotificationCenterModel modelObj = (NotificationCenterModel)Session["NotifList"];
            Session["SelectedNotification"] = modelObj.listOfNotif[Convert.ToInt32(words[1])];
        }
}