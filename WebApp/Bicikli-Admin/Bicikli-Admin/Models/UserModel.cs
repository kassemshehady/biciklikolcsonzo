using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Bicikli_Admin.Models
{
    public class UserModel
    {
        public Guid guid { get; set; }

        [Display(Name="Felhasználónév")]
        public string username { get; set; }

        [Display(Name="E-Mail")]
        public string email { get; set; }

        [Display(Name="Kölcsönzők #")]
        public int countOfLenders { get; set; }

        [Display(Name="Utolsó belépés")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public DateTime lastLogin { get; set; }

        [Display(Name="Site Admin?")]
        public bool isSiteAdmin { get; set; }

        [Display(Name="Jóváhagyva?")]
        public bool isApproved { get; set; }

        [Display(Name="Kitiltva?")]
        public bool isLockedOut { get; set; }
    }
}
