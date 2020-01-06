using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TripPlanner.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace TripPlanner.Controllers
{
    public class HomeController : Controller
    {
        public MyContext dbContext;
        public HomeController(MyContext context){
            dbContext =context;
        }

        public IActionResult Index()
        {
            return View();
        }
        //RegisterUser
        [HttpPost("register")]
        public IActionResult RegisterUser(RegLoginUser newUser){
            if(ModelState.IsValid){
                if(dbContext.Users.Any(u => u.Email == newUser.RegUser.Email)){
                    ModelState.AddModelError("RegUser.Email","Email Address should be unique");
                    return View("Index",newUser);
                }
                else{
                    PasswordHasher<User> hasher = new PasswordHasher<User>();
                    newUser.RegUser.Password = hasher.HashPassword(newUser.RegUser,newUser.RegUser.Password);
                    dbContext.Add(newUser.RegUser);
                    dbContext.SaveChanges();
                    //Log the user by adding to Session
                    // User userInDb = dbContext.Users.FirstOrDefault(u => u.Email == newUser.RegUser.Email);

                    // int? userID = HttpContext.Session.GetInt32("LoggedUser");
                    // if(userID == null){
                    HttpContext.Session.SetInt32("LoggedUser",newUser.RegUser.UserId);
                    // }

                    return RedirectToAction("Dashboard");
 
                }
            }
            return View("Index",newUser);
        }

        [HttpGet("success")]
        public IActionResult Success(){
            int? logged_id = HttpContext.Session.GetInt32("LoggedUser");
            if(logged_id == null){
                
                return View("Index");
            }
            User logged = dbContext.Users.FirstOrDefault(u => u.UserId == logged_id);
            return View("account",logged);
        }
        [HttpGet("login")]
        public IActionResult Login(){
            return View();
        }
        [HttpGet("logOut")]
        public IActionResult LogOut(){
            int? logged_id = HttpContext.Session.GetInt32("LoggedUser");
            if(logged_id == null){
                
                return RedirectToAction("Index");
            }
            HttpContext.Session.Remove("LoggedUser");
            return RedirectToAction("Index");
        }

        [HttpPost("loginUser")]
        public IActionResult LoginUser(RegLoginUser user){
            if(ModelState.IsValid){
                User userInDb = dbContext.Users.FirstOrDefault(u => u.Email == user.LoginUser.Email);
                if(userInDb == null){
                    ModelState.AddModelError("LoginUser.Email","Invalid Email Addreess");
                    return View("Index",user);
                }
                PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
                PasswordVerificationResult result = hasher.VerifyHashedPassword(user.LoginUser,userInDb.Password,user.LoginUser.Password); 
                if (result == 0){
                    ModelState.AddModelError("LoginUser.Password","Passowrd doesn't match the given Email Addess");
                    return View("Index",user);
                }else{
                    // int? userID = HttpContext.Session.GetInt32("LoggedUser");
                    // if(userID == null){
                    HttpContext.Session.SetInt32("LoggedUser",userInDb.UserId);
                    // }
                    return RedirectToAction("Dashboard");
                }
            }
            return View("Index",user);
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard(){
            int? logged_id = HttpContext.Session.GetInt32("LoggedUser");
            if(logged_id == null){
                
                return RedirectToAction("Index");
            }
            DashboardView dbv =new DashboardView();
            dbv.loggedUser = dbContext.Users.Include(u => u.createdPlans).Include(u => u.JoiningTrips).ThenInclude(jt => jt.JoiningTrip).FirstOrDefault(u => u.UserId == logged_id);
            dbv.OthersTrips = dbContext.Trips.Where(t => !t.Tourists.Any(tourist => tourist.TravellerId == logged_id)).ToList();
            
            return View(dbv);
        }

        [HttpGet("trips/new")]
        public IActionResult NewTrip(){
            int? logged_id = HttpContext.Session.GetInt32("LoggedUser");
            if(logged_id == null){
                
                return RedirectToAction("Index");
            }
            return View();
        }

        //AddTrip
        [HttpPost("trips/addTrip")]
        public IActionResult AddTrip(Trip newTrip){
            int? logged_id = HttpContext.Session.GetInt32("LoggedUser");
            if(logged_id == null){
                
                return RedirectToAction("Index");
            }
            if(ModelState.IsValid){
                newTrip.PlannerId = (int)logged_id;
                dbContext.Add(newTrip);
                dbContext.SaveChanges();
                Tourist tourist = new Tourist();
                tourist.TravellerId = (int)logged_id;
                tourist.JoiningTripId = newTrip.TripId;
                dbContext.Tourists.Add(tourist);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            return View("NewTrip",newTrip);

        }

        ///trips/delete/@tour.JoiningTrip.TripId
        [HttpGet("trips/delete/{tripId}")]
        public IActionResult DeletTrip(int tripId){
            int? logged_id = HttpContext.Session.GetInt32("LoggedUser");
            if(logged_id == null){
                
                return RedirectToAction("Index");
            }
            if(dbContext.Trips.Any(t => t.TripId == tripId && t.PlannerId == (int)logged_id)){
                Trip delTrip = dbContext.Trips.SingleOrDefault(t => t.TripId == tripId);
                dbContext.Trips.Remove(delTrip);
                dbContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            return RedirectToAction("Dashboard");
        }

        //"/trips/edit/@tour.JoiningTrip.TripId"
        [HttpGet("trips/edit/{tripId}")]
        public IActionResult EditTrip(int tripId){
            int? logged_id = HttpContext.Session.GetInt32("LoggedUser");
            if(logged_id == null){
                
                return RedirectToAction("Index");
            }
            if(dbContext.Trips.Any(t => t.TripId == tripId && t.PlannerId == (int)logged_id)){
                Trip editTrip = dbContext.Trips.SingleOrDefault(t => t.TripId == tripId);
                return View(editTrip);
            }
            return RedirectToAction("Dashboard");
        }

        //UpdateTrip
        [HttpPost("trips/edit/{tripId}")]
        public IActionResult EditTrip(int tripId,Trip editTrip){
            int? logged_id = HttpContext.Session.GetInt32("LoggedUser");
            if(logged_id == null){
                
                return RedirectToAction("Index");
            }
            if(ModelState.IsValid){
                 if(dbContext.Trips.Any(t => t.TripId == editTrip.TripId && t.PlannerId == (int)logged_id)){
                    //Trip editTrip = dbContext.Trips.SingleOrDefault(t => t.TripId == tripId);
                    editTrip.PlannerId = (int)logged_id;
                    dbContext.Update(editTrip);
                    dbContext.Entry(editTrip).Property("CreatedAt").IsModified = false;
                    dbContext.SaveChanges();
                    return RedirectToAction("Dashboard");
                }
                return RedirectToAction("Dashboard");
            }
            return View("EditTrip",editTrip);
        }
        ///trips/cancelTrip/@tour.TouristId
        [HttpGet("trips/cancel/{tourId}")]
        public IActionResult cancelTrip(int tourId){
            int? logged_id = HttpContext.Session.GetInt32("LoggedUser");
            if(logged_id == null){
                
                return RedirectToAction("Index");
            }

            Tourist remTour = dbContext.Tourists.FirstOrDefault(t => t.TouristId == tourId );

            if(remTour != null && remTour.TravellerId == (int)logged_id){
                dbContext.Remove(remTour);
                dbContext.SaveChanges();
            }
            return RedirectToAction("Dashboard");
        }
        //trips/join/@trip.TripId
        [HttpGet("trips/join/{tripId}")]
        public IActionResult JoinTrip(int tripId){
            int? logged_id = HttpContext.Session.GetInt32("LoggedUser");
            if(logged_id == null){
                
                return RedirectToAction("Index");
            }
            if(!dbContext.Trips.Any(t => t.TripId == tripId)){
                return RedirectToAction("Dashboard");
            }
            if(!dbContext.Tourists.Any(tour => tour.TravellerId == (int)logged_id && tour.JoiningTripId == tripId)){
                Tourist tour = new Tourist();
                tour.TravellerId = (int)logged_id;
                tour.JoiningTripId = tripId;
                dbContext.Tourists.Add(tour);
                dbContext.SaveChanges();
            }
            return RedirectToAction("Dashboard");
        }

        //trip/detail/@tour.JoiningTripId
        [HttpGet("trip/detail/{tripId}")]
        public IActionResult tripDetail(int tripId){
            int? logged_id = HttpContext.Session.GetInt32("LoggedUser");
            if(logged_id == null){
                
                return RedirectToAction("Index");
            }
            Trip tripToShow = dbContext.Trips
                                        .Include(t => t.Planner)
                                        .Include(t =>  t.Tourists)
                                        .ThenInclude(tour => tour.Traveller)
                                        .FirstOrDefault(t => t.TripId == tripId);
            if(tripToShow!= null){
                    return View(tripToShow);
            }
            return RedirectToAction("Dashboard");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
