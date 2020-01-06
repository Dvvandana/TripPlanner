using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

namespace TripPlanner.Models{
    public class User{
        [Key]
        public int UserId{get;set;}

        [Required]
        [MinLength(2,ErrorMessage="First Name should be atleast 2 characters long")]
        [Display(Name = "First Name:")]
        public string FirstName{get;set;}

        [Required]
        [MinLength(2,ErrorMessage="Last Name should be atleast 2 characters long")]
        [Display(Name = "Last Name:")]
        public string LastName{get;set;}

        [Required]
        [EmailAddress]
        public string Email{get;set;}

        [Required]
        [MinLength(8,ErrorMessage="Password must be 8 characters long")]
        [DataType(DataType.Password)]
        public string Password{get;set;}
        public DateTime CreatedAt{get;set;} = DateTime.Now;
        public DateTime UpdatedAt{get;set;} = DateTime.Now;

        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string Confirm{get;set;}

        public List<Tourist> JoiningTrips{get;set;}
        public List<Trip> createdPlans{get;set;}
    }
}