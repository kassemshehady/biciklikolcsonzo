using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Bicikli_Admin.ApiModels;
using Bicikli_Admin.CommonClasses;
using Bicikli_Admin.EntityFramework;

namespace Bicikli_Admin.ApiControllers
{
    public class PrintingSubscriptionController : ApiController
    {
        // PUT api/PrintingSubscription
        public void Put(PrinterModel printerModel)
        {
            if (!PrintingSubscription.setPrinter(printerModel))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }
        }

        // DELETE api/PrintingSubscription/5
        public void Delete(int id)
        {
            if (!PrintingSubscription.removePrinter(id))
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }
        }
    }
}
