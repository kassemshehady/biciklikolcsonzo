using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bicikli_Admin.ApiModels
{
    public class LenderModel
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public String description { get; set; }
    }

    public class LenderListItemModel
    {
        public int id { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string name { get; set; }
        public string address { get; set; }
    }
}