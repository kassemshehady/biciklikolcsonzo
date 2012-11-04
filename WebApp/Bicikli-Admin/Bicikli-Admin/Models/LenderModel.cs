using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bicikli_Admin.Models
{
    public class LenderModel
    {
        [Display(Name = "#")]
        public int? id { get; set; }

        [Required]
        [Display(Name = "Földrajzi szélesség")]
        [RegularExpression(@"[0-9]+(\.([0-9])+)?", ErrorMessage="A következő mező formátuma nem megfelelő: {0}.")]
        public double latitude { get; set; }

        [Required]
        [Display(Name = "Földrajzi hosszúság")]
        [RegularExpression(@"[0-9]+(\.([0-9])+)?", ErrorMessage = "A következő mező formátuma nem megfelelő: {0}.")]
        public double longitude { get; set; }

        [Required]
        [Display(Name = "Név")]
        public string name { get; set; }

        [Required]
        [Display(Name = "Cím")]
        [DataType(DataType.MultilineText)]
        public string address { get; set; }

        [Display(Name = "Leírás")]
        [DataType(DataType.MultilineText)]
        public String description { get; set; }

        [Display(Name = "Nyomtató IP")]
        [RegularExpression(@"([0-9]{1,3}\.){3}[0-9]{1,3}", ErrorMessage="A {0} mező nem tartalmaz érvényes IPv4 címet.")]
        public String printer_ip { get; set; }
    }
}