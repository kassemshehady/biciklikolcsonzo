using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Bicikli_Admin.ApiModels;
using Bicikli_Admin.CommonClasses;
using Bicikli_Admin.EntityFramework;
using Bicikli_Admin.EntityFramework.linq;

namespace Bicikli_Admin.ApiControllers
{
    public class ReportController : ApiController
    {
        // POST api/Report
        public ReportResponseModel Post(ReportRequestModel requestModel)
        {
            var dc = new BicikliDataClassesDataContext();
            var responseModel = new ReportResponseModel();
            var currentTime = DateTime.Now;

            const int normalPricePerMinutes = 60;
            const int dangerPricePerMinites = 200;

            // STEP 1: Check if this Report is valid (Session exists and has not ended)
            Session session = dc.Sessions.FirstOrDefault(s => ((s.end_time == null) && (s.id == requestModel.session_id)));
            if (session == null)
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }

            // STEP 2: Check if this Report has came from a DangerousZone
            var inDangerousZones = dc.GetDangerousZonesByDistance(requestModel.latitude, requestModel.longitude);
            bool tooMuchAfterLastReport = ((session.last_report != null) && (currentTime - (DateTime)session.last_report).TotalMinutes > 1);
            
            // If this Report is in DangerousZone or too much time elapsed since the last report -> mark as DangerousZone
            if ((inDangerousZones.Count() > 0) || tooMuchAfterLastReport)
            {
                responseModel.status = ReportResponseStatus.OK_DANGER;
            }
            else
            {
                responseModel.status = ReportResponseStatus.OK_NORMAL;
            }

            // STEP 3: Calculate time and price

            // Calculate time spent since last report
            int elapsedSeconds = (int) Math.Floor((currentTime - (DateTime)session.start_time).TotalSeconds);
            if (session.last_report != null)
            {
                elapsedSeconds = (int) Math.Floor((currentTime - (DateTime)session.last_report).TotalSeconds);
            }

            // Calculate time spent in danger time until now
            responseModel.danger_time = 0;
            if (session.dz_time != null)
            {
                responseModel.danger_time = (int) session.dz_time;
            }

            // Calculate time spent in normal time until now
            responseModel.normal_time = 0;
            if (session.normal_time != null)
            {
                responseModel.normal_time = (int)session.normal_time;
            }

            // If the last report was in DangerousZone then add elapsed time to that counter
            if ((session.dz_id != null) || (tooMuchAfterLastReport))
            {
                responseModel.danger_time += elapsedSeconds;
            }
            else
            {
                responseModel.normal_time += elapsedSeconds;
            }

            // Calculate total balance
            responseModel.total_balance = (int) Math.Round(responseModel.normal_time * (normalPricePerMinutes/60.0));
            responseModel.total_balance += (int) Math.Round(responseModel.danger_time * (dangerPricePerMinites/60.0));

            // STEP 4: Update Session with Report data
            session.last_report = currentTime;
            session.latitude = requestModel.latitude;
            session.longitude = requestModel.longitude;
            session.dz_time = responseModel.danger_time;
            session.normal_time = responseModel.normal_time;
            
            if (responseModel.status == ReportResponseStatus.OK_DANGER)
            {
                inDangerousZones = dc.GetDangerousZonesByDistance(requestModel.latitude, requestModel.longitude);
                var dz = inDangerousZones.FirstOrDefault();

                if (dz != null)
                {
                    session.dz_id = dz.id;
                }
            }
            else
            {
                session.dz_id = null;
            }

            // STEP 5: Check if the Report was created in a Lender after a while -> end of session
            var lenderNearby = dc.GetLendersByDistance(requestModel.latitude, requestModel.longitude, 10).FirstOrDefault();

            if (((currentTime - (DateTime)session.start_time).TotalMinutes > 10) && (lenderNearby != null))
            {
                session.end_time = currentTime;
                session.Bike.current_lender_id = lenderNearby.id;
                responseModel.status = ReportResponseStatus.END_OF_SESSION;
                dc.SubmitChanges();

                // Send Invoice
                Lender lender = dc.Lenders.First(l => (l.id == lenderNearby.id));
                PrintingSubscription.sendInvoice(session, lender);
            }
            else
            {
                dc.SubmitChanges();
            }

            // STEP 6: Send response
            return responseModel;
        }
    }
}
