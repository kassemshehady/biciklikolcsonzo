using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Security;

namespace Bicikli_Admin.Models
{

    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Jelenlegi jelszó")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Az új jelszónak legalább {2} hosszúnak kell lennie.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Új jelszó")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Új jelszó megerősítése")]
        [Compare("NewPassword", ErrorMessage = "Az új jelszó és annak megerősítése nem egyeznek meg.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "Felhasználónév")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Jelszó")]
        public string Password { get; set; }

        [Display(Name = "Belépés megjegyzése?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "Felhasználónév")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[0-9a-zA-Z\.-]+@([0-9a-zA-Z-]+\.)+[a-zA-Z]{2,}$", ErrorMessage = "Az E-Mail cím formátuma nem megfelelő.")]
        [Display(Name = "E-Mail cím")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "A jelszónak legalább {2} hosszúnak kell lennie.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Jelszó")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Jelszó megerősítése")]
        [Compare("Password", ErrorMessage = "A jelszó és annak megerősítése nem egyeznek meg.")]
        public string ConfirmPassword { get; set; }
    }
}
