using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VerifyEmailForgotPassword.Models
{
    public class UserRegisterRequest
    {
        [Required,EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required,MinLength(6,ErrorMessage ="Please Enter at least 6 characters")]
        public string Password { get; set; } = string.Empty;
        [Required,Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

    }
}
