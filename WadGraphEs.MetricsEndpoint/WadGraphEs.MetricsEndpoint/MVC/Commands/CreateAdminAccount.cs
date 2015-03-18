using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WadGraphEs.MetricsEndpoint.MVC.Commands {
    public class CreateAdminAccount {
        [Required(ErrorMessage="Please enter a non-empty username")]
        public string Username{get;set;}
        [Required(ErrorMessage="Please enter a non-empty password")]
        public string Password{get;set;}
        [Compare("Password", ErrorMessage="Passwords don't match")]
        public string ConfirmPassword{get;set;}
    }
}