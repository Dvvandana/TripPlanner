using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
public class FutureDateAttribute:ValidationAttribute{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext){
        
        if((DateTime)value < DateTime.Now){
            return new ValidationResult("Date should be from Future");
        }
        return ValidationResult.Success;
    }
}

public class EndStartDateAttribute:ValidationAttribute{
    public string otherProperty;
    public EndStartDateAttribute(string op){
        this.otherProperty = op;
    }
    protected override ValidationResult IsValid(object value, ValidationContext validationContext){
        var container = validationContext.ObjectInstance.GetType();
        var field = container.GetProperty(this.otherProperty);
        DateTime start = (DateTime)field.GetValue(validationContext.ObjectInstance,null);
        if((DateTime)value < start){
            return new ValidationResult("End Date should be after StartDate");
        }
        return ValidationResult.Success;
    }
}

namespace TripPlanner.Models{
    public class Trip{
        [Key]
        public int TripId{get;set;}

        [Required]
        [MinLength(2)]
        public string Destination{get;set;}
        [Required]
        [DataType(DataType.DateTime),DisplayFormat(DataFormatString="{0: MM dd, yyyy}")]
        [FutureDate]
        public DateTime StartDate{get;set;}

        [Required]
        [DataType(DataType.DateTime),DisplayFormat(DataFormatString="{0: MM dd, yyyy}")]
        [FutureDate]
        [EndStartDate("StartDate")]

        public DateTime EndDate{get;set;}

        [Required]
        [MinLength(10)]
        public string Plan{get;set;}

        public DateTime CreatedAt{get;set;} = DateTime.Now;
        public DateTime UpdatedAt{get;set;} = DateTime.Now;

        public int PlannerId{get;set;}
        public User Planner{get;set;}
        public List<Tourist> Tourists{get;set;}

    }
}