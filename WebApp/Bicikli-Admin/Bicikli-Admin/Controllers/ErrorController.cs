using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Bicikli_Admin.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error

        public ViewResult Index()
        {
            return View();
        }

        public ViewResult NotFound()
        {
            return View();
        }

        public ViewResult Forbidden()
        {
            return View();
        }

        public ViewResult ValidationError()
        {
            return View();
        }

    }
}
