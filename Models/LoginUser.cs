using System.ComponentModel.DataAnnotations;


namespace TripPlanner.Models{
    public class LoginUser{
        [Required]
        [EmailAddress]
        public string Email{get;set;}

        [Required]
        [DataType(DataType.Password)]
        public string Password{get;set;}
    }
}