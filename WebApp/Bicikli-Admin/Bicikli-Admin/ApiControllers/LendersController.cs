using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Bicikli_Admin.EntityFramework;

namespace Bicikli_Admin.ApiControllers
{
    public class LendersController : ApiController
    {
        // GET api/Lenders
        public IEnumerable<GetLendersListResult> Get()
        {
            var dc = new BicikliDataClassesDataContext();
            return dc.GetLendersList();
        }

        // GET api/Lenders/5
        public GetLenderDetailsResult Get(int id)
        {
            var dc = new BicikliDataClassesDataContext();
            return dc.GetLenderDetails(id).SingleOrDefault();
        }
    }
}
