using System.ComponentModel.DataAnnotations;

namespace ServiceRequestForm.Models
{
    public class LoginViewModel
    {


        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Username { get; set; }

        public string Password { get; set; }


    }
}

