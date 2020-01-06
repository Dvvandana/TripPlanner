using System;
using System.Collections.Generic;

namespace TripPlanner.Models{
    public class DashboardView{
        public User loggedUser{get;set;}
        public List<Trip> OthersTrips{get;set;}
        public List<Trip> TripsTaking{get;set;}
    }
    
}