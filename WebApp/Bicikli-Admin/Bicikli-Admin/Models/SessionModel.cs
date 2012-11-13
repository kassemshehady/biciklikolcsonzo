using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Bicikli_Admin.ApiControllers;

namespace Bicikli_Admin.Models
{
    public class SessionModel
    {
        [Display(Name = "#")]
        public int? id { get; set; }

        [Display(Name = "Kölcsönzés kezdete")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public DateTime startTime { get; set; }

        [Display(Name = "Kölcsönzés vége")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public DateTime? endTime { get; set; }

        [Display(Name = "Bicikli")]
        [Required(ErrorMessage = "A következő mező kitöltése kötelező: {0}")]
        public int bike_id { get; set; }

        public BikeModel bikeModel { get; set; }

        [Display(Name = "Utolsó bejelentés")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public DateTime? lastReport { get; set; }

        [Display(Name = "Veszélyes zóna")]
        public int? dangerousZoneId { get; set; }

        [Display(Name = "Veszélyes zónában töltött idő")]
        public int? dangerousZoneTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.0}")]
        public double? dangerousZoneTimeMinutes { get { return dangerousZoneTime / 60.0; } }

        [Display(Name = "Zónán kívül töltött idő")]
        public int? normalTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.0}")]
        public double? normalTimeMinutes { get { return normalTime / 60.0; } }

        [DisplayFormat(DataFormatString = "{0:0.0}")]
        [Display(Name = "Kölcsönzési idő")]
        public double? totalTimeMinutes { get { return normalTimeMinutes + dangerousZoneTimeMinutes; } }

        [DisplayFormat(DataFormatString = "{0:0}")]
        [Display(Name = "Egyenleg")]
        public double? totalBalance
        {
            get
            {
                return (normalTimeMinutes * ReportController.normalPricePerMinutes) + (dangerousZoneTimeMinutes * ReportController.dangerPricePerMinites);
            }
        }

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