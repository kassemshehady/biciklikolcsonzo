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
        public ReportResponseStatus status;
        public int normal_time;
        public int danger_time;
        public int total_balance;
    }

    public enum ReportResponseStatus
    {
        OK_NORMAL, OK_DANGER, ERROR, END_OF_SESSION
    }
}