using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bicikli_Admin.Models
{
    public class SessionModel
    {
        [Display(Name = "#")]
        public int? id { get; set; }

        [Display(Name = "Kölcsönzés ideje")]
        [Required(ErrorMessage = "A következő mező kitöltése kötelező: {0}")]
        public DateTime startTime { get; set; }

        [Display(Name = "Kölcsönzés befejezve")]
        public DateTime? endTime { get; set; }

        [Display(Name = "Bicikli")]
        [Required(ErrorMessage = "A következő mező kitöltése kötelező: {0}")]
        public int bike_id { get; set; }

        [Display(Name = "Utolsó bejelentés")]
        public DateTime? lastReport { get; set; }

        [Display(Name = "Veszélyes zóna")]
        public int? dangerousZoneId { get; set; }

        [Display(Name = "Veszélyes zónában töltött idő")]
        public int? dangerousZoneTime { get; set; }

        [Display(Name = "Kölcsönzés időtartama")]
        public int? normalTime { get; set; }

        [Display(Name = "Földrajzi szélesség")]
        public double? latitude { get; set; }

        [Display(Name = "Földrajzi hosszúság")]
        public double? longitude { get; set; }

        [Display(Name = "Név")]
        [Required(ErrorMessage = "A következő mező kitöltése kötelező: {0}")]
        public string name { get; set; }

        [Display(Name = "Cím")]
        [Required(ErrorMessage = "A következő mező kitöltése kötelező: {0}")]
        [DataType(DataType.MultilineText)]
        public string address { get; set; }

        [Display(Name = "Fizetve?")]
        public bool paid { get; set; }
    }
}