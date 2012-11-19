using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Bicikli_Admin.CommonClasses;
using Bicikli_Admin.EntityFramework;
using Bicikli_Admin.EntityFramework.linq;

namespace Bicikli_Admin.ApiControllers
{
    public class LendersController : ApiController
    {
        // GET api/Lenders
        public IEnumerable<ApiModels.LenderListItemModel> Get()
        {
            try
            {
                return DataRepository.GetLendersForApi();
            }
            catch
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
            }
        }

        // GET api/Lenders/5
        public ApiModels.LenderModel Get(int id)
        {
            try
            {
                return DataRepository.GetLenderForApi(id);
            }
            catch
            {
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            }
        }
    }
}
