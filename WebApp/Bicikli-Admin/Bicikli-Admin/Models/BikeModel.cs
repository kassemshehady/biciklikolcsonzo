using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bicikli_Admin.Models
{
    public class BikeModel
    {
        public BikeModel() { }

        public BikeModel(BikeModel m)
        {
            id = m.id;
            name = m.name;
            description = m.description;
            imageUrl = m.imageUrl;
            currentLenderId = m.currentLenderId;
            isActive = m.isActive;
            isInDangerousZone = m.isInDangerousZone;
            lastLendingDate = m.lastLendingDate;
            session = m.session;
            lastSession = m.lastSession;
            lender = m.lender;
        }

        [Display(Name = "#")]
        public int? id { get; set; }

        [Display(Name = "Megnevezés")]
        [Required(ErrorMessage = "A következő mező kitöltése kötelező: {0}")]
        public string name { get; set; }

        [Display(Name = "Leírás")]
        [DataType(DataType.MultilineText)]
        public String description { get; set; }

        [Display(Name = "Kép")]
        public String imageUrl { get; set; }

        [Display(Name = "Aktuális kölcsönző")]
        public int? currentLenderId { get; set; }

        [Display(Name = "Aktív?")]
        public bool isActive { get; set; }

        // foreign...
        [Display(Name = "Veszélyes zónában van?")]
        public bool isInDangerousZone { get; set; }

        // foreign...
        [Display(Name = "Utoljára kölcsönözve")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}")]
        public DateTime? lastLendingDate { get; set; }

        // foreign...
        public SessionModel session { get; set; }
        public SessionModel lastSession { get; set; }
        public LenderModel lender { get; set; }
    }

    public class LastLendingOfBike
    {
        public DateTime? lastLendingDate { get; set; }
        public SessionModel lastSession { get; set; }
    }
}