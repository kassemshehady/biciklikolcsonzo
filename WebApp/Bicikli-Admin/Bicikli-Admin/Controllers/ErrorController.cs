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

        //
        // GET: /Error/NotFound

        public ViewResult NotFound()
        {
            return View();
        }

        //
        // GET: /Error/Forbidden

        public ViewResult Forbidden()
        {
            return View();
        }

        //
        // GET: /Error/ValidationError

        public ViewResult ValidationError()
        {
            return View();
        }

        //
        // GET: /Error/BadRequest

        public ViewResult BadRequest()
        {
            return View();
        }

        //
        // GET: /Error/ServiceUnavailable

        public ViewResult ServiceUnavailable()
        {
            return View();
        }
    }
}
