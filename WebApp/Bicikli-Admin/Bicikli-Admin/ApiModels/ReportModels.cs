using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bicikli_Admin.ApiModels
{
    public class ReportRequestModel
    {
        public int session_id;
        public float latitude;
        public float longitude;
    }

    public class ReportResponseModel
    {
        public string status;
        public int normal_time;
        public int danger_time;
        public int total_balance;
    }

    public class ReportResponseStatus
    {
        public const string OK_NORMAL = "OK_NORMAL";
        public const string OK_DANGER = "OK_DANGER";
        public const string ERROR = "ERROR";
        public const string END_OF_SESSION = "END_OF_SESSION";
    }
}