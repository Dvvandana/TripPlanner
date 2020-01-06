using Microsoft.EntityFrameworkCore;
 
namespace TripPlanner.Models
{
    public class MyContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users{get;set;}
        public DbSet<Tourist> Tourists{get;set;}
        public DbSet<Trip> Trips{get;set;}
    }
}