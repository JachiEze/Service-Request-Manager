using System;
using System.ComponentModel.DataAnnotations;

namespace ServiceRequestForm.Models
{

    public class FutureDateAttribute : ValidationAttribute
    {
        private readonly int _daysAhead;

        public FutureDateAttribute(int daysAhead)
        {
            _daysAhead = daysAhead;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date >= DateTime.Today.AddDays(_daysAhead))
                {
                    return ValidationResult.Success!;
                }
                return new ValidationResult($"Date must be at least {_daysAhead} days in the future.");
            }

            return new ValidationResult("Invalid date format.");
        }
    }

    public class ServiceRequest
    {
        public int Id { get; set; }

        
        public string? RequestTitle { get; set; }
        public string? Entity { get; set; }
        public string? Department { get; set; }
        public int NumberOfPersonnel { get; set; }
        public string? ServiceDescription { get; set; }
        public string? PersonnelType { get; set; }
        public string? Location { get; set; }
        public string? ServiceScheme { get; set; }
        public string? User1Comment { get; set; }
        public string? CostCentre { get; set; }
        public string? GlAccount { get; set; }
        public string? BudgetOwner { get; set; }
        public string? JobImpact { get; set; }
        public string? User2Comment { get; set; }
        public string? Status { get; set; }
        public string? RequestedBy { get; set; }
        [FutureDate(7)]
        public DateTime ProposedServiceStartDate { get; set; } = DateTime.Today;
    }
}

