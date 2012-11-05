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

        [Required(ErrorMessage="A következő mező kitöltése kötelező: {0}")]
        [Display(Name = "Földrajzi szélesség")]
        public double latitude { get; set; }

        [Required(ErrorMessage = "A következő mező kitöltése kötelező: {0}")]
        [Display(Name = "Földrajzi hosszúság")]
        public double longitude { get; set; }

        [Required(ErrorMessage = "A következő mező kitöltése kötelező: {0}")]
        [Display(Name = "Név")]
        public string name { get; set; }

        [Required(ErrorMessage = "A következő mező kitöltése kötelező: {0}")]
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