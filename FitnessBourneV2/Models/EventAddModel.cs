using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FitnessBourneV2.Models
{
    public class EventAddModel
    {
        public int eventID;

        [Display(Name = "Event Type")]
        public string eventTypeName;

        [Display(Name = "Is the event private to your club members?")]
        public bool isPrivate;
        public bool isCheckIn;

        [Display(Name = "Event start time")]
        public DateTime startDateTime;

        [Display(Name = "Event end time")]
        public DateTime endDateTime;

        [Display(Name = "Event capacity")]
        public Int32 mem_capacity;

        [Display(Name = "Club Name")]
        public Int32 clubName;

        public MemberTable eventAdmin;
        public EventEdit editedEvent;

        [Display(Name = "Location One")]
        public string Location1;

        [Display(Name = "Location Two")]
        public string Location2;

        [Display(Name = "Location Three")]
        public string Location3;

        [Display(Name = "Location Four")]
        public string Location4;
        
        public List<FitnessClub> clubList;
        public List<EventType> eventType;

    }
}