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

            foreach (NotificationActionTable actTable in db.NotificationActionTables.ToList())
            {
                // Add all notif for the login user
                if (actTable.MemberTable.Mem_Id == loginUser.Mem_Id)
                {
                    // Notification table
                    NotificationTable notifTable = actTable.NotificationTable;

                    // check if event already deleted
                    if (actTable.EventTable == null)
                    {
                        //yes
                        //Create Notif object
                        NotificationObject objNotif = new NotificationObject()
                        {
                            notifMessage = notifTable.Notif_Message,
                            eventType = "",
                            startLoc = "",
                            stopLoc = "",
                            startDT = "",
                            stopDT = "",
                            isEventAdmin = false,
                            youAre = "Event Participant",
                            notifTbleId = notifTable.Notif_Id,
                            isEventDeleted = true
                        };

                        //add object to list
                        listOfNotif.Add(objNotif);
                    }
                    else
                    {
                        // No
                        //Admin Notification
                        //Setting location feeds
                        List<LocationTable> locList = actTable.EventTable.LocationTables.ToList();
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

                        if (notifTable.Notif_Type == "A")
                        {
                            isAdmin = true;
                            userType = "Event Admin";
                        }

                        //Create Notif object
                        NotificationObject objNotif = new NotificationObject()
                        {
                            notifMessage = notifTable.Notif_Message,
                            eventType = actTable.EventTable.EventType.ET_Name,
                            startLoc = startLoc,
                            stopLoc = stopLoc,
                            startDT = actTable.EventTable.Evnt_Start_DateTime.ToString(),
                            stopDT = actTable.EventTable.Evnt_End_DateTime.ToString(),
                            isEventAdmin = isAdmin,
                            youAre = userType,
                            notifTbleId = notifTable.Notif_Id,
                            isEventDeleted = false
                        };

                        //add object to list
                        listOfNotif.Add(objNotif);
                    }

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
                if (Session["NotifList"] != null)
                {
                    Session["AlertMessage"] = "All notifications are cleared!!!";
                }
                else
                {
                    Session["AlertMessage"] = "You do not have any notification!!!";
                }
                
                return RedirectToAction("Index", "Home");
            }

        }

        public ActionResult notifResult(NotificationCenterModel notifModel)
        {
            // get selected notificatio
            NotificationObject objNotif = (NotificationObject)Session["SelectedNotification"];

            //Confirm the edit of event and raise notification for all participants
            NotificationTable notifObj = db.NotificationTables.Find(objNotif.notifTbleId);

            if (Session["NotificationStatus"].ToString() == "confirm")
            {
                // Event edit
                EventEdit editObj = notifObj.EventEdit;

                // Event table to update
                EventTable eventTbleOrginal = db.EventTables.Find(editObj.EE_EventIdToEdit);

                //edited event
                EventTable editedEventTbleObj = editObj.EventTable;

                //Create new notifications for participant of events
                if (eventTbleOrginal.EventMembers.Count > 0)
                {
                    //there are members
                    List<LocationTable> localList = new List<LocationTable>();

                    //Setting location feeds
                    List<LocationTable> locList = eventTbleOrginal.LocationTables.ToList();
                    int startId = locList[0].Loc_Id;
                    int stopId = 0;
                    string startLoc = "";
                    string stopLoc = "";
                    List<string> checkPointList = new List<string>();

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
                        if (record.ET_Id == eventTbleOrginal.EventTypeET_Id)
                        {
                            eventType = record.ET_Name;
                        }
                    }

                    // Participant notification
                    NotificationTable notifTble = new NotificationTable()
                    {
                        Notif_Type = "P",
                        Notif_Message = "The " + eventType + " event you registered on " + eventTbleOrginal.Evnt_Start_DateTime.ToString() + " from " + startLoc + " to " + stopLoc + " has changed."
                    };

                    db.NotificationTables.Add(notifTble);
                    db.SaveChanges();

                    foreach (EventMembers eveMem in eventTbleOrginal.EventMembers)
                    {
                        // loop through members of event
                        // Notification table for notification
                        NotificationActionTable actTble = new NotificationActionTable()
                        {
                            NA_Decision = "NO",
                            MemberTable = eveMem.MemberTable,
                            NotificationTable = notifTble,
                            EventTable = eventTbleOrginal
                        };

                        db.NotificationActionTables.Add(actTble);
                        db.SaveChanges();
                    }


                }


                // Edit orginal object with respect to proposed edit
                eventTbleOrginal.Evnt_Is_Private = editedEventTbleObj.Evnt_Is_Private;
                eventTbleOrginal.Evnt_Capacity = editedEventTbleObj.Evnt_Capacity;
                eventTbleOrginal.Evnt_Start_DateTime = editedEventTbleObj.Evnt_Start_DateTime;
                eventTbleOrginal.Evnt_End_DateTime = editedEventTbleObj.Evnt_End_DateTime;
                eventTbleOrginal.EventTypeET_Id = editedEventTbleObj.EventTypeET_Id;
                eventTbleOrginal.Evnt_NavigDetails = editedEventTbleObj.Evnt_NavigDetails;

                //Update location
                eventTbleOrginal.LocationTables.Clear();
                eventTbleOrginal.LocationTables = editedEventTbleObj.LocationTables;


                // state modified
                db.Entry(eventTbleOrginal).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();


                // delete notification corresponding to change
                //Notification delete
                db.NotificationTables.Remove(notifObj);


                //Delete Notification Action tables
                foreach (NotificationActionTable tble in db.NotificationActionTables.ToList())
                {
                    if (tble.NotificationTableNotif_Id == notifObj.Notif_Id && tble.MemberTable.Mem_Email_Id == User.Identity.Name)
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

                MemberTable loginUser = new MemberTable();
                foreach (MemberTable record in db.MemberTables.ToList())
                {
                    if (record.Mem_Email_Id == User.Identity.Name)
                    {
                        loginUser = record;
                        break;
                    }
                }

                //Delete Notification action tables for the notification corresponding to the user
                foreach (NotificationActionTable tble in db.NotificationActionTables.ToList())
                {

                    if (tble.NotificationTableNotif_Id == notifObj.Notif_Id && tble.MemberTable.Mem_Id == loginUser.Mem_Id)
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
}