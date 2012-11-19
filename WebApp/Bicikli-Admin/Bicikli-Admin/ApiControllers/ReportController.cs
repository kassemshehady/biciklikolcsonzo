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
using Bicikli_Admin.Models;

namespace Bicikli_Admin.ApiControllers
{
    public class ReportController : ApiController
    {
        //
        // POST api/Report

        public ReportResponseModel Post(ReportRequestModel requestModel)
        {
            try
            {
                var responseModel = new ReportResponseModel();
                var currentTime = DateTime.Now;

                #region STEP 1: Check if this Report is valid (Session exists and has not ended)

                SessionModel session;

                try
                {
                    session = DataRepository.GetSession(requestModel.session_id);
                    if (session.endTime != null)
                    {
                        throw new Exception();
                    }
                }
                catch
                {
                    throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
                }

                #endregion

                #region STEP 2: Check if this Report has came from a DangerousZone

                bool tooMuchAfterLastReport = ((session.lastReport != null) && (currentTime - (DateTime)session.lastReport).TotalMinutes > 1);
                var nearestZone = DataRepository.GetNearestDangerousZone(requestModel.latitude, requestModel.longitude);

                // If this Report came from DangerousZone or too much time elapsed since the last report -> mark as DangerousZone
                if ((nearestZone != null) || tooMuchAfterLastReport)
                {
                    responseModel.status = ReportResponseStatus.OK_DANGER;
                }
                else
                {
                    responseModel.status = ReportResponseStatus.OK_NORMAL;
                }

                #endregion

                #region STEP 3: Calculate time and price

                // Calculate time spent since last report
                int elapsedSeconds = (int)Math.Floor((currentTime - (DateTime)(session.lastReport ?? session.startTime)).TotalSeconds);

                // Calculate time spent in danger time until now
                responseModel.danger_time = (int)(session.dangerousZoneTime ?? 0);

                // Calculate time spent in normal time until now
                responseModel.normal_time = (int)(session.normalTime ?? 0);

                // If the last report was in DangerousZone then add elapsed time to that counter
                if ((session.dangerousZoneId != null) || tooMuchAfterLastReport)
                {
                    responseModel.danger_time += elapsedSeconds;
                }
                else
                {
                    responseModel.normal_time += elapsedSeconds;
                }

                // Calculate total balance
                responseModel.total_balance = (int)Math.Round(responseModel.normal_time * (session.normal_price / 60.0));
                responseModel.total_balance += (int)Math.Round(responseModel.danger_time * (session.danger_price / 60.0));

                #endregion

                #region STEP 4: Update Session with Report data

                session.lastReport = currentTime;
                session.latitude = requestModel.latitude;
                session.longitude = requestModel.longitude;
                session.dangerousZoneTime = responseModel.danger_time;
                session.normalTime = responseModel.normal_time;

                if (responseModel.status == ReportResponseStatus.OK_DANGER)
                {
                    if (nearestZone != null)
                    {
                        session.dangerousZoneId = nearestZone.id;
                    }
                }
                else
                {
                    session.dangerousZoneId = null;
                }

                #endregion

                #region STEP 5: Check if the Report was created in a Lender after a while -> end of session

                var nearestLender = DataRepository.GetNearestLenderInRadius(requestModel.latitude, requestModel.longitude, 10);

                if (((currentTime - (DateTime)session.startTime).TotalMinutes > 10) && (nearestLender != null))
                {
                    session.endTime = currentTime;
                    session.bikeModel.currentLenderId = nearestLender.id;
                    responseModel.status = ReportResponseStatus.END_OF_SESSION;
                    DataRepository.UpdateSession(session);
                    DataRepository.UpdateBike(session.bikeModel);

                    // Send Invoice
                    var lender = DataRepository.GetLender((int)nearestLender.id);
                    PrintingSubscription.sendInvoice(session, lender);
                }
                else
                {
                    DataRepository.UpdateSession(session);
                }

                #endregion

                // STEP 6: Send response
                return responseModel;
            }
            catch (HttpResponseException e)
            {
                throw e;
            }
            catch
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest));
            }
        }
    }
}
