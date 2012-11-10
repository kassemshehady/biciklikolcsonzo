using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bicikli_Admin.Models
{
    public class ZoneModel
    {
        [Display(Name = "#")]
        public int? id { get; set; }

        [Required(ErrorMessage = "A következő mező kitöltése kötelező: {0}")]
        [Display(Name = "Zóna neve")]
        public string name { get; set; }

        [Display(Name = "Zóna leírása")]
        [DataType(DataType.MultilineText)]
        public String description { get; set; }

        [Required(ErrorMessage = "A következő mező kitöltése kötelező: {0}")]
        [Display(Name = "Földrajzi szélesség")]
        public double latitude { get; set; }

        [Required(ErrorMessage = "A következő mező kitöltése kötelező: {0}")]
        [Display(Name = "Földrajzi hosszúság")]
        public double longitude { get; set; }

        [Required(ErrorMessage = "A következő mező kitöltése kötelező: {0}")]
        [Display(Name = "Körzet sugara (méterben)")]
        public double radius { get; set; }

        [Display(Name = "Biciklik a zónában")]
        public int bikesInThisZone { get; set; }
    }
}