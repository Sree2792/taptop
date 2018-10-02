using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FitnessBourneV2.Models
{
    public class EventCreated
    {
        public List<CreatedEventDetails> eventList { get; set; }
    }

    public class CreatedEventDetails
    {
        public int eventID { get; set; }
        public string startLoc { get; set; }
        public string stopLoc { get; set; }
        public string checkPoints { get; set; }
        public string totalDistance { get; set; }
        public string navInstructions { get; set; }
        public string eventStartTime { get; set; }
        public string eventEndTime { get; set; }
        public string seatAvailblity { get; set; }
        public string totalCapacity { get; set; }
        public string EventypeInView { get; set; }
        public List<LocationTable> listOfLocations { get; set; }
        public List<string> locationString { get; set; }
        public List<MemberTable> EventMembers { get; set; }
    }
}