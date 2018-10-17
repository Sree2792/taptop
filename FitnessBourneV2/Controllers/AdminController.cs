using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FitnessBourneV2.Models;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace FitnessBourneV2.Controllers
{
    public class AdminController : Controller
    {
        private fbmodelContainer db = new fbmodelContainer();
        private ApplicationDbContext dbDef = new ApplicationDbContext();
        // GET: Admin
        public ActionResult Index()
        {
            List<memberObject> objList = new List<memberObject>();
            foreach(MemberTable tableObj in db.MemberTables.ToList())
            {
                memberObject obj = new memberObject()
                {
                    mem_EmailID = tableObj.Mem_Email_Id,
                    mem_ContactNum = tableObj.Mem_Contact_No,
                    mem_id = tableObj.Mem_Id
                };
                objList.Add(obj);
            }

            AdminDemo modelToPass = new AdminDemo()
            {
                memberList = objList
            };

            return View(modelToPass);
        }

        public ActionResult deleteMember(Int32 memIDVal)
        {
            MemberTable memDeleteObj = db.MemberTables.Find(memIDVal);

            //From Asp.net db
            ApplicationUser userObj = new ApplicationUser();
            foreach(ApplicationUser obj in dbDef.Users.ToList())
            {
                if(obj.Email == memDeleteObj.Mem_Email_Id)
                {
                    userObj = obj;
                    break;
                }
            }

            //Nullify all foreign keys
            //memObj.AddressTable = null;
            //memObj.EventMembers = null;
            //memObj.EventEdited = null;
            //memObj.EventTables = null;
            //memObj.NotificationActionTables = null;

            // get all events created by Member
            List<EventTable> memCreatedEvents = memDeleteObj.EventTables.ToList();

            // get all event members who joined a event
            List<EventMembers> eveMembers = memDeleteObj.EventMembers.ToList();

            // drop eventmembers - member joined events
            dropEventMembersInList(eveMembers, false, memDeleteObj);


            // Drop memberevents that joined the event created by member
            foreach (EventTable tble in memCreatedEvents)
            {
                //Get all members joined in event
                List<EventMembers> eveMembersCurrent = tble.EventMembers.ToList();

                // drop eventmembers - member joined events
                dropEventMembersInList(eveMembersCurrent, true, memDeleteObj);

                //drop locations of event
                foreach(LocationTable locObj in tble.LocationTables.ToList())
                {
                    // drop location table
                    LocationTable dropLoc = db.LocationTables.Find(locObj.Loc_Id);

                    db.LocationTables.Remove(dropLoc);
                }
                //drop event
                EventTable dropObj = db.EventTables.Find(tble.Evnt_Id);
                db.EventTables.Remove(dropObj);
                db.SaveChanges();
            }


            //Delete member from our db
            db.MemberTables.Remove(memDeleteObj);
            db.SaveChanges();

            //Delete member from local db
            dbDef.Users.Remove(userObj);
            dbDef.SaveChanges();

            return RedirectToAction("Index", "Admin");
        }

        public void dropEventMembersInList(List<EventMembers> eveMemList, bool isFullEvent, MemberTable memDeleteObj)
        {
            // drop member joined events
            foreach (EventMembers obj in eveMemList)
            {
                EventMembers memObj = db.EventMembers.Find(obj.EvMem_Id);
                EventTable eventDet = memObj.EventTable;
                List<EventMembers> eventMembers = db.EventMembers.ToList();


                if (memObj.EvMem_IsConfirmed && !isFullEvent)
                {
                    // check if event capacity
                    if (Convert.ToInt32(eventDet.Evnt_Capacity) < eventDet.EventMembers.ToList().Count)
                    {
                        // capaciy exceeded case
                        foreach (EventMembers objInside in eventMembers)
                        {
                            if (objInside.EventTable.Evnt_Id == eventDet.Evnt_Id)
                            {
                                // first non confirmed member in list && member not the one that is deleted
                                if (!objInside.EvMem_IsConfirmed && objInside.MemberTable.Mem_Id != memDeleteObj.Mem_Id)
                                {
                                    // send mail
                                    string subject = "Event status changed to Confirmation";
                                    string plainTextContent = "Your event from " + eventDet.Evnt_Start_DateTime.ToString() + " to " + eventDet.Evnt_End_DateTime.ToString() + "has been confirmed.";
                                    changeStatusTo(subject, plainTextContent, objInside.MemberTable.Mem_Email_Id.ToString());

                                    //change his status to confirmed after sending mail
                                    EventMembers changeStatus = db.EventMembers.Find(objInside.EvMem_Id);
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

                // delete event member
                db.EventMembers.Remove(memObj);
                db.SaveChanges();
            }
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