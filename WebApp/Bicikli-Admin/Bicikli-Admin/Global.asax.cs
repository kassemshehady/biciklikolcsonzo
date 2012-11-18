using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Bicikli_Admin.CommonClasses;
using Bicikli_Admin.Controllers;

namespace Bicikli_Admin
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            // custom model binders for double and decimal parsing
            ModelBinders.Binders.DefaultBinder = new CustomModelBinder();
            ModelBinders.Binders.Add(typeof(double), new DoubleModelBinder());
            ModelBinders.Binders.Remove(typeof(decimal));
            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Add text/html support to JsonFormatter + pretty printing
            var jsonFormatter = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            jsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            jsonFormatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
        }

        void Application_Error(Object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();

            if (exception is HttpRequestValidationException)
            {
                Response.ClearContent();
                var controller = getErrorController("ValidationError");
                controller.ValidationError().ExecuteResult(controller.ControllerContext);
                Response.End();
            }
            else if (exception is HttpException)
            {
                Response.StatusCode = (exception as HttpException).GetHttpCode();
            }
            else
            {
                Response.ClearContent();
                var controller = getErrorController("Index");
                controller.Index().ExecuteResult(controller.ControllerContext);
                //Response.Write(exception.ToString());
                //Response.Write((exception as HttpException).GetHttpCode().ToString());
                Response.End();
            }
        }

        void Application_EndRequest(object sender, EventArgs e)
        {
            if (((string) Request.RequestContext.RouteData.Values["controller"]) != "Error")
            {
                ErrorController controller;
                switch ((HttpStatusCode)Response.StatusCode)
                {
                    case HttpStatusCode.Forbidden:
                        Response.ClearContent();
                        controller = getErrorController("Forbidden");
                        controller.Forbidden().ExecuteResult(controller.ControllerContext);
                        Response.End();
                        break;
                    case HttpStatusCode.NotFound:
                        Response.ClearContent();
                        controller = getErrorController("NotFound");
                        controller.NotFound().ExecuteResult(controller.ControllerContext);
                        Response.End();
                        break;
                }
            }
        }

        /// <summary>
        /// Returns a preconfigured ErrorController that can be executed
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private ErrorController getErrorController(string action)
        {
            var controller = new ErrorController();
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.RequestContext = Request.RequestContext;
            controller.ControllerContext.RouteData.Values["controller"] = "Error";
            controller.ControllerContext.RouteData.Values["action"] = action;
            return controller;
        }

    }
}