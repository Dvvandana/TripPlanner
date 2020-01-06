using System;
using System.ComponentModel.DataAnnotations;

namespace TripPlanner.Models{
    public class Tourist{
        [Key]
        public int TouristId{get;set;}

        public DateTime CreatedAt{get;set;} = DateTime.Now;
        public DateTime UpdatedAt{get;set;} = DateTime.Now;
        
        public int TravellerId{get;set;}
        public User Traveller{get;set;}

        public int JoiningTripId{get;set;}
        public Trip JoiningTrip{get;set;}
    }
}