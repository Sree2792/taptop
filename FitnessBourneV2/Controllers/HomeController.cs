﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FitnessBourneV2.Models;
using System.Net.Http;
using System.Net.Mail;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace FitnessBourneV2.Controllers
{
    public class HomeController : Controller
    {
        private fbmodelContainer db = new fbmodelContainer();
        public ActionResult Index()
        {
            if(db.EventTypes.ToList().Count == 0)
            {
                // event type cycling
                EventType typeOne = new EventType()
                {
                    ET_Name = "cycling"
                };
                db.EventTypes.Add(typeOne);
                db.SaveChanges();

                //event type walking
                EventType typeTwo = new EventType()
                {
                    ET_Name = "walking"
                };
                db.EventTypes.Add(typeTwo);
                db.SaveChanges();

                // event type Running
                EventType typeThree = new EventType()
                {
                    ET_Name = "running"
                };
                db.EventTypes.Add(typeThree);
                db.SaveChanges();
            }
            if(db.FitnessClubs.ToList().Count == 0)
            {
                //Club 1
                FitnessClub fClub = new FitnessClub()
                {
                    FC_Ref_Name = "Club Malvern"
                };

                AddressTable adrTable = new AddressTable()
                {
                    Adr_Unit_No = "",
                    Adr_House_No = "93",
                    Adr_Street_Name = "Argyll",
                    Adr_Suburb_Name = "Malvern East",
                    Adr_City_Name = "Melbourne",
                    Adr_State_Name = "Victoria",
                    Adr_Zipcode = "3145",
                    Adr_Lat = -37.874,
                    Adr_Long = 145.08,
                    Adr_FullAddress = ""
                };

                fClub.AddressTable = adrTable;
                db.FitnessClubs.Add(fClub);

                db.SaveChanges();

                //Club 2
                FitnessClub fClub2 = new FitnessClub()
                {
                    FC_Ref_Name = "Club Yarra"
                };

                AddressTable adrTable2 = new AddressTable()
                {
                    Adr_Unit_No = "",
                    Adr_House_No = "23",
                    Adr_Street_Name = "Nicholson Street",
                    Adr_Suburb_Name = "South Yarra",
                    Adr_City_Name = "Melbourne",
                    Adr_State_Name = "Victoria",
                    Adr_Zipcode = "3141",
                    Adr_Lat = -37.8424,
                    Adr_Long = 144.9874,
                    Adr_FullAddress = ""
                };

                fClub2.AddressTable = adrTable2;
                db.FitnessClubs.Add(fClub2);
                db.SaveChanges();

                //Club 3
                FitnessClub fClub3 = new FitnessClub()
                {
                    FC_Ref_Name = "Club Port"
                };

                AddressTable adrTable3 = new AddressTable()
                {
                    Adr_Unit_No = "",
                    Adr_House_No = "30",
                    Adr_Street_Name = "Albert Street",
                    Adr_Suburb_Name = "Port Melbourne",
                    Adr_City_Name = "Melbourne",
                    Adr_State_Name = "Victoria",
                    Adr_Zipcode = "3207",
                    Adr_Lat = -37.8368,
                    Adr_Long = 144.9337,
                    Adr_FullAddress = ""
                };

                fClub3.AddressTable = adrTable3;
                db.FitnessClubs.Add(fClub3);
                db.SaveChanges();

                //Club 4
                FitnessClub fClub4 = new FitnessClub()
                {
                    FC_Ref_Name = "Club William"
                };

                AddressTable adrTable4 = new AddressTable()
                {
                    Adr_Unit_No = "",
                    Adr_House_No = "23",
                    Adr_Street_Name = "Park Cres",
                    Adr_Suburb_Name = "Williamstown North",
                    Adr_City_Name = "Melbourne",
                    Adr_State_Name = "Victoria",
                    Adr_Zipcode = "3106",
                    Adr_Lat = -37.8561,
                    Adr_Long = 144.883,
                    Adr_FullAddress = ""
                };

                fClub4.AddressTable = adrTable4;
                db.FitnessClubs.Add(fClub4);
                db.SaveChanges();

                //Club 5
                FitnessClub fClub5 = new FitnessClub()
                {
                    FC_Ref_Name = "Club Flemingo"
                };

                AddressTable adrTable5 = new AddressTable()
                {
                    Adr_Unit_No = "",
                    Adr_House_No = "1",
                    Adr_Street_Name = "Norwood Street",
                    Adr_Suburb_Name = "Flemington",
                    Adr_City_Name = "Melbourne",
                    Adr_State_Name = "Victoria",
                    Adr_Zipcode = "3031",
                    Adr_Lat = -37.7867,
                    Adr_Long = 144.9296,
                    Adr_FullAddress = ""
                };

                fClub5.AddressTable = adrTable5;
                db.FitnessClubs.Add(fClub5);
                db.SaveChanges();

                //Club 6
                FitnessClub fClub6 = new FitnessClub()
                {
                    FC_Ref_Name = "Club Beach"
                };

                AddressTable adrTable6 = new AddressTable()
                {
                    Adr_Unit_No = "",
                    Adr_House_No = "2",
                    Adr_Street_Name = "St Kilda Road",
                    Adr_Suburb_Name = "St Kilda",
                    Adr_City_Name = "Melbourne",
                    Adr_State_Name = "Victoria",
                    Adr_Zipcode = "3182",
                    Adr_Lat = -37.8566,
                    Adr_Long = 144.9834,
                    Adr_FullAddress = ""
                };

                fClub6.AddressTable = adrTable6;
                db.FitnessClubs.Add(fClub6);
                db.SaveChanges();

                //Club 7
                FitnessClub fClub7 = new FitnessClub()
                {
                    FC_Ref_Name = "Club Don"
                };

                AddressTable adrTable7 = new AddressTable()
                {
                    Adr_Unit_No = "",
                    Adr_House_No = "3",
                    Adr_Street_Name = "Doncaster Road",
                    Adr_Suburb_Name = "Balwyn North",
                    Adr_City_Name = "Melbourne",
                    Adr_State_Name = "Victoria",
                    Adr_Zipcode = "3104",
                    Adr_Lat = -37.7934,
                    Adr_Long = 145.06413,
                    Adr_FullAddress = ""
                };

                fClub7.AddressTable = adrTable7;
                db.FitnessClubs.Add(fClub7);
                db.SaveChanges();
            }
            
            if (Request.IsAuthenticated)
            {

                if(Session["AlertMessage"] != null)
                {
                    // set viewbag message
                    ViewBag.Message = Session["AlertMessage"].ToString();
                    Session["AlertMessage"] = null;
                }

                Session["UserLoggedIn"] = User.Identity.Name;

                return View();
            }
            return RedirectToAction("Login", "Account");
        }

        public void BtnAction(HomeModel recieved)
        {

            string screenName = Session["ScreenNavName"].ToString();

            if (screenName == "AddEvent")
            {
                //Add Event
                RedirectToAction("EventAdd", "Event");
            }
        }

        [HttpPost]
        public void setSession(string ItemRows)
        {
            Session["ScreenNavName"] = ItemRows;
        }

        [HttpPost]
        public void contactUs(string anchorname)
        {
            //To install package-: Install-Package SendGrid
            //var apiKey = Environment.GetEnvironmentVariable("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");
            var client = new SendGridClient("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");

            var from = new EmailAddress(User.Identity.Name.ToString(), "User");

            var subject = "Trouble shooting message from user";

            var to = new EmailAddress("sreejith92pf@gmail.com", "Admin");

            var plainTextContent = anchorname;

            var htmlContent = "<strong>" + anchorname  + "</strong>";

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = client.SendEmailAsync(msg);
        }
    }
}