using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bicikli_Admin.ApiModels;
using Bicikli_Admin.EntityFramework;
using Bicikli_Admin.EntityFramework.linq;

namespace Bicikli_Admin.CommonClasses
{
    public class PrintingSubscription
    {
        /*
         * Sets or Updates a printer subscription.
         */
        public static bool setPrinter(PrinterModel printerModel)
        {
            var dc = new BicikliDataClassesDataContext();
            int result = dc.SetPrinter(printerModel.lender_id, printerModel.printer_ip);

            if (result == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /*
         * Removes the printer subscription from a lender.
         */
        public static bool removePrinter(int lender_id)
        {
            var printerModel = new PrinterModel();
            printerModel.lender_id = lender_id;
            printerModel.printer_ip = null;
            return setPrinter(printerModel);
        }

        /*
         * Sends an Invoice to a remote printer.
         */
        public static bool sendInvoice(Session session, Lender lender)
        {
            // TODO: Some TCP/IP network communication with the Print server...

            return false;   // Not Implemented Yet
        }
    }
}