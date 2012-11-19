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
        public SessionModel() { }

        public SessionModel(SessionModel m)
        {
            id = m.id;
            startTime = m.startTime;
            endTime = m.endTime;
            bike_id = m.bike_id;
            bikeModel = m.bikeModel;
            lastReport = m.lastReport;
            dangerousZoneId = m.dangerousZoneId;
            dangerousZoneTime = m.dangerousZoneTime;
            normalTime = m.normalTime;
            latitude = m.latitude;
            longitude = m.longitude;
            name = m.name;
            address = m.address;
            paid = m.paid;
            normal_price = m.normal_price;
            normal_vat = m.normal_vat;
            danger_price = m.danger_price;
            danger_vat = m.danger_vat;
        }

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
                return (normalTimeMinutes * normal_price) + (dangerousZoneTimeMinutes * danger_price);
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

        public int normal_price { get; set; }
        public double normal_vat { get; set; }
        public int danger_price { get; set; }
        public double danger_vat { get; set; }
    }
}