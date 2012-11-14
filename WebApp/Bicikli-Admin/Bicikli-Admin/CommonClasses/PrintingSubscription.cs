using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using Bicikli_Admin.ApiControllers;
using Bicikli_Admin.ApiModels;
using Bicikli_Admin.EntityFramework;
using Bicikli_Admin.EntityFramework.linq;
using Bicikli_Admin.Models;

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
            var s = DataRepository.GetSession(session.id);
            var l = DataRepository.GetLender(lender.id);

            return sendInvoice(s, l);
        }

        public static bool sendInvoice(SessionModel session, LenderModel lender)
        {
            if ((session.endTime == null) && (lender.printer_ip != null))
            {
                return false;
            }

            var invoice = new InvoiceModel()
            {
                address = session.address,
                name = session.name,
                lender_name = lender.name,
                lender_address = lender.address,
                total_balance = (int)Math.Round((double)session.totalBalance),
                bike_name = session.bikeModel.name,
                items = new List<InvoiceItemModel>()
            };

            invoice.items.Add(new InvoiceItemModel()
            {
                title = "Normál zónában töltött idő",
                vat = 27,
                base_unit_price = ReportController.normalPricePerMinutes,
                amount = (int) Math.Round((double)session.normalTimeMinutes)
            });

            invoice.items.Add(new InvoiceItemModel()
            {
                title = "Veszélyes zónában töltött idő",
                vat = 27,
                base_unit_price = ReportController.dangerPricePerMinites,
                amount = (int)Math.Round((double)session.dangerousZoneTimeMinutes)
            });

            try
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(lender.printer_ip), 6060);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    socket.Connect(remoteEP);
                    var stream = new StreamWriter(new NetworkStream(socket));
                    var serializer = new DataContractJsonSerializer(typeof(InvoiceModel));
                    using (MemoryStream memory = new MemoryStream())
                    {
                        serializer.WriteObject(memory, invoice);
                        stream.Write(Encoding.UTF8.GetString(memory.ToArray()));
                    }
                    stream.Flush();
                }
                catch
                {
                    try
                    {
                        var db = new BicikliDataClassesDataContext();
                        db.Lenders.Single(l => l.id == lender.id).printer_ip = null;
                        db.SubmitChanges();
                    }
                    catch { }

                    return false;
                }

                try
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch { }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}